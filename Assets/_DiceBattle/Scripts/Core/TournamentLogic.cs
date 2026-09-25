using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.Localization;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;

namespace DiceBattle.Core
{
    /// <summary>
    /// Orchestrates a tournament match: player vs bot, alternating turns,
    /// two rolls each, standard deck (no inventory bonuses).
    /// </summary>
    public class TournamentLogic
    {
        private const int RollsPerTurn = 2;
        private const float MatchEndPause = 1f;

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
        private TournamentBracket _bracket;

        private Phase _phase;
        private int _playerRollsLeft;
        private int _enemyRollsLeft;
        private bool _matchEnded;
        private bool _isPlayerRolling;
        private int _matchEndTweenId = -1;

        public TournamentLogic(GameConfig config, TournamentScreen screen)
        {
            _config = config;
            _screen = screen;
        }

        public void InitializeMatch()
        {
            _matchEnded = false;
            DiceRuleset.SetStandard(_config.DiceStartCount);

            _bracket = new TournamentBracket(new[] { PickOpponentClass() });

            _player = BuildFighter(_config.GetPlayerConfig(GameData.SelectedCharacterClass));
            _enemy = BuildFighter(_config.GetPlayerConfig(_bracket.Next()));

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
            _playerRollsLeft = RollsPerTurn;
            _enemyRollsLeft = RollsPerTurn;
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
        }

        public void ContextClick()
        {
            if (_phase != Phase.PlayerRolling || _matchEnded || _isPlayerRolling)
            {
                return;
            }

            if (_playerRollsLeft == RollsPerTurn)
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
            _enemyRollsLeft = RollsPerTurn;
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
            ResolveRound();
        }

        private void ResolveRound()
        {
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());

            ApplyDefense(_player, _screen.PlayerTakeHeal);
            ApplyDefense(_enemy, _screen.EnemyTakeHeal);

            ApplyAttack(_player, _screen.EnemyTakeDamage, _screen.EnemyTakeCriticalHit, _screen.EnemyAnimateDamage);

            if (_enemy.Data.CurrentHealth <= 0)
            {
                EndMatch(playerWon: true);
                return;
            }

            ApplyAttack(_enemy, _screen.PlayerTakeDamage, _screen.PlayerTakeCriticalHit, _screen.PlayerAnimateDamage);

            if (_player.Data.CurrentHealth <= 0)
            {
                EndMatch(playerWon: false);
                return;
            }

            _player.Data.Armor = _player.BaseArmor;
            _enemy.Data.Armor = _enemy.BaseArmor;

            _screen.ResetPlayerDice();
            _screen.ResetEnemyDice();

            BeginRound();
        }

        private static void ApplyDefense(TournamentFighter fighter, System.Action<int> heal)
        {
            heal(fighter.Result.Heal);
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
        }

        private void EndMatch(bool playerWon)
        {
            _matchEnded = true;

            Debug.Log($"Турнир: {(playerWon ? "Победа" : "Поражение")}. " +
                      $"Игрок HP {_player.Data.CurrentHealth}/{_player.Data.MaxHealth}, " +
                      $"Бот HP {_enemy.Data.CurrentHealth}/{_enemy.Data.MaxHealth}");

            DiceRuleset.Reset();
            _matchEndTweenId = LeanTween.delayedCall(MatchEndPause,
                () => SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TournamentPyramidScreen))).id;
        }

        private CharacterClass PickOpponentClass()
        {
            var values = (CharacterClass[])System.Enum.GetValues(typeof(CharacterClass));
            return values[Random.Range(0, values.Length)];
        }

        private static TournamentFighter BuildFighter(UnitConfig config)
        {
            var data = new UnitData
            {
                Name = config.Name,
                Portrait = config.Portraits.Length > 0 ? config.Portraits[0] : null,
                MaxHealth = config.StartHealth,
                CurrentHealth = config.StartHealth,
                Damage = config.StartDamage,
                Armor = config.StartArmor,
            };

            return new TournamentFighter
            {
                Data = data,
                BaseDamage = config.StartDamage,
                BaseArmor = config.StartArmor,
            };
        }
    }
}
