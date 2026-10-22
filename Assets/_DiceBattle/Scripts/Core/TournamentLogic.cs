using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.Localization;
using DiceBattle.UI;
using GameSignals;

namespace DiceBattle.Core
{
    /// <summary>
    /// Orchestrates a tournament match: player vs bot, alternating turns,
    /// two rolls each, standard deck (no inventory bonuses).
    /// </summary>
    public class TournamentLogic
    {
        private const int _rollsPerTurn = 2;
        private const float _matchEndPause = 1f;
        private const float _enemyResultPause = 0.6f;
        private const float _playerHitDelay = 0.2f;
        private const float _enemyAttackDelay = 0.4f;
        private const float _enemyHitDelay = 0.65f;
        private const float _nextRoundPause = 0.8f;

        private enum Phase
        {
            PlayerRolling,
            EnemyRolling,
            Resolving,
        }

        private readonly GameConfig _config;
        private readonly TournamentScreen _screen;

        private TournamentFighter _player;
        private TournamentFighter _enemy;

        private Phase _phase;
        private int _playerRollsLeft;
        private int _enemyRollsLeft;
        private bool _matchEnded;
        private bool _isPlayerRolling;
        private int _matchEndTweenId = -1;

        public bool IsMatchEnded => _matchEnded;

        public TournamentLogic(GameConfig config, TournamentScreen screen)
        {
            _config = config;
            _screen = screen;
        }

        public void InitializeMatch()
        {
            _matchEnded = false;
            TournamentBracket.BeginMatch();
            DiceRuleset.SetStandard(_config.DiceStartCount);

            _player = BuildFighter(_config.GetPlayerConfig(GameData.SelectedCharacterClass));
            _enemy = BuildFighter(_config.GetPlayerConfig(TournamentBracket.CurrentOpponent));

            _screen.SetPlayerData(_player.Data);
            _screen.SetEnemyData(_enemy.Data);

            _screen.ResetPlayerDice();
            _screen.ResetEnemyDice();
            _screen.ClearPlayerDicePreview();
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());

            BeginRound();
        }

        private void BeginRound()
        {
            _playerRollsLeft = _rollsPerTurn;
            _enemyRollsLeft = _rollsPerTurn;
            _phase = Phase.PlayerRolling;
            _isPlayerRolling = false;

            _player.Result.Calculate(new List<Dice>());
            _enemy.Result.Calculate(new List<Dice>());

            _screen.EnablePlayerDice();
            _screen.SetContextLabel(LocalizationManager.Localize(LocKeys.GameHits.RollDice));
        }

        public void AbandonMatch()
        {
            _matchEnded = true;
            LeanTween.cancel(_screen.gameObject);
            LeanTween.cancel(_matchEndTweenId);
            DiceRuleset.Reset();
            TournamentBracket.RegisterDefeat();
        }

#if UNITY_EDITOR
        public void DebugEndMatch(bool playerWon)
        {
            if (_matchEnded)
            {
                return;
            }

            LeanTween.cancel(_screen.gameObject);
            EndMatch(playerWon);
        }
#endif

        public void ContextClick()
        {
            if (_phase != Phase.PlayerRolling || _matchEnded || _isPlayerRolling)
            {
                return;
            }

            if (_playerRollsLeft == _rollsPerTurn)
            {
                StartPlayerRoll(_screen.RollPlayer);
            }
            else if (_screen.HavePlayerSelectedDice)
            {
                StartPlayerRoll(_screen.RerollPlayerSelected);
            }
            else
            {
                FinishPlayerTurn();
            }
        }

        private void StartPlayerRoll(System.Action roll)
        {
            _playerRollsLeft--;
            _isPlayerRolling = true;
            _screen.DisablePlayerDice();
            roll();
        }

        public void AllClick()
        {
            if (_phase != Phase.PlayerRolling || _playerRollsLeft == 0 || _matchEnded || _isPlayerRolling)
            {
                return;
            }

            _screen.SetPlayerSelectionStatus(_screen.HavePlayerUnselectedDice);
        }

        public void OnRollCompleted()
        {
            if (_matchEnded)
            {
                return;
            }

            switch (_phase)
            {
                case Phase.PlayerRolling:
                    _isPlayerRolling = false;
                    _player.Result.Calculate(_screen.PlayerDices);
                    _screen.SetPlayerDicePreview(_player.Result.Armor, _player.Result.Damage, _player.Result.Heal);

                    if (_playerRollsLeft == 0)
                    {
                        FinishPlayerTurn();
                    }
                    else
                    {
                        _screen.EnablePlayerDice();
                        _screen.SetContextLabel(LocalizationManager.Localize(LocKeys.GameHits.Finish));
                    }
                    break;

                case Phase.EnemyRolling:
                    _enemy.Result.Calculate(_screen.EnemyDices);
                    ContinueEnemyTurn();
                    break;
            }
        }

        private void FinishPlayerTurn()
        {
            _player.Result.Calculate(_screen.PlayerDices);
            _screen.DisablePlayerDice();
            _screen.ClearPlayerDicePreview();

            _phase = Phase.EnemyRolling;
            _enemyRollsLeft = _rollsPerTurn;
            SignalSystem.Raise<IHintHandler>(handler => handler.Show(LocalizationManager.Localize(LocKeys.GameHits.EnemyTurn)));

            LeanTween.delayedCall(_screen.gameObject, 0.5f, StartEnemyTurn);
        }

        private void StartEnemyTurn()
        {
            _enemyRollsLeft--;
            _screen.RollEnemy();
        }

        private void ContinueEnemyTurn()
        {
            List<Dice> rerollTargets = _enemy.SelectRerollTargets(_screen.EnemyDices);

            if (_enemyRollsLeft > 0 && rerollTargets.Count > 0)
            {
                _enemyRollsLeft--;
                _screen.SelectEnemyDice(rerollTargets);
                LeanTween.delayedCall(_screen.gameObject, 0.4f, _screen.RerollEnemySelected);
            }
            else
            {
                FinishEnemyTurn();
            }
        }

        private void FinishEnemyTurn()
        {
            _enemy.Result.Calculate(_screen.EnemyDices);

            _phase = Phase.Resolving;
            LeanTween.delayedCall(_screen.gameObject, _enemyResultPause, ResolveRound);
        }

        // Paced like a campaign turn: the bot's dice stay readable, then the player's blow, then the answer.
        private void ResolveRound()
        {
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());

            ApplyDefense(_player, _screen.PlayerTakeHeal, _screen.PlayerAnimateHeal);
            ApplyDefense(_enemy, _screen.EnemyTakeHeal, _screen.EnemyAnimateHeal);
            _screen.UpdateStats();

            PlaySound(_player.Data.AttackSound);
            LeanTween.delayedCall(_screen.gameObject, _playerHitDelay, CompletePlayerAttack);
        }

        private void CompletePlayerAttack()
        {
            ApplyAttack(_player, _screen.EnemyTakeDamage, _screen.EnemyTakeCriticalHit, _screen.EnemyAnimateDamage);

            if (_enemy.Data.CurrentHealth <= 0)
            {
                EndMatch(playerWon: true);
                return;
            }

            SoundType attackSound = _enemy.Data.AttackSound;
            LeanTween.delayedCall(_screen.gameObject, _enemyAttackDelay, () => PlaySound(attackSound));
            LeanTween.delayedCall(_screen.gameObject, _enemyHitDelay, CompleteEnemyAttack);
        }

        private void CompleteEnemyAttack()
        {
            ApplyAttack(_enemy, _screen.PlayerTakeDamage, _screen.PlayerTakeCriticalHit, _screen.PlayerAnimateDamage);

            if (_player.Data.CurrentHealth <= 0)
            {
                EndMatch(playerWon: false);
                return;
            }

            _player.Data.Armor = _player.BaseArmor;
            _enemy.Data.Armor = _enemy.BaseArmor;
            _screen.UpdateStats();

            LeanTween.delayedCall(_screen.gameObject, _nextRoundPause, StartNextRound);
        }

        private void StartNextRound()
        {
            _screen.ResetPlayerDice();
            _screen.ResetEnemyDice();

            BeginRound();
        }

        private static void ApplyDefense(TournamentFighter fighter, System.Action<int> heal, System.Action animateHeal)
        {
            int healthBefore = fighter.Data.CurrentHealth;
            heal(fighter.Result.Heal);

            if (fighter.Data.CurrentHealth > healthBefore)
            {
                animateHeal();
            }

            fighter.Data.Armor = fighter.BaseArmor + fighter.Result.Armor;
        }

        private static void ApplyAttack(TournamentFighter attacker,
            System.Action<int> dealDamage, System.Action criticalHit, System.Action animateDamage)
        {
            if (attacker.Result.IsCritical)
            {
                criticalHit();
            }
            else
            {
                dealDamage(attacker.BaseDamage + attacker.Result.Damage);
            }

            animateDamage();
            PlaySound(attacker.Data.ImpactSound);
        }

        private static void PlaySound(SoundType soundType)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(soundType));
        }

        private void EndMatch(bool playerWon)
        {
            _matchEnded = true;

            if (playerWon)
            {
                TournamentBracket.RegisterWin();
            }
            else
            {
                TournamentBracket.RegisterDefeat();
            }

            DiceRuleset.Reset();
            _matchEndTweenId = LeanTween.delayedCall(_matchEndPause,
                () => SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TournamentPyramidScreen))).id;
        }

        private static TournamentFighter BuildFighter(UnitConfig config)
        {
            return new TournamentFighter
            {
                Data = config.CreateUnitData(),
                BaseDamage = config.StartDamage,
                BaseArmor = config.StartArmor,
            };
        }
    }
}
