using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
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

        private enum Phase
        {
            PlayerRolling,
            EnemyRolling,
            Resolving,
        }

        private readonly GameConfig _config;
        private readonly TournamentScreen _screen;
        private readonly DiceList _standardDeck = new();

        private TournamentFighter _player;
        private TournamentFighter _enemy;
        private TournamentBracket _bracket;

        private Phase _phase;
        private int _playerRollsLeft;
        private int _enemyRollsLeft;
        private bool _matchEnded;

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

            _player.Result.Calculate(new List<Dice>(), _standardDeck);
            _enemy.Result.Calculate(new List<Dice>(), _standardDeck);

            _screen.EnablePlayerDice();
            _screen.SetContextLabel("Бросить кубики"); // TODO Localization
        }

        public void ContextClick()
        {
            if (_phase != Phase.PlayerRolling || _matchEnded)
            {
                return;
            }

            if (_playerRollsLeft == RollsPerTurn)
            {
                _screen.RollPlayer();
                _playerRollsLeft--;
            }
            else if (_playerRollsLeft > 0)
            {
                _playerRollsLeft--;

                if (_screen.HavePlayerSelectedDice)
                {
                    _screen.RerollPlayerSelected();
                }
                else
                {
                    FinishPlayerTurn();
                    return;
                }
            }

            if (_playerRollsLeft == 0)
            {
                _screen.DisablePlayerDice();
                return;
            }

            _screen.SetContextLabel("Закончить"); // TODO Localization
            SignalSystem.Raise<IHintHandler>(handler => handler.Show($"Осталось бросков: {_playerRollsLeft}")); // TODO Localization
        }

        public void AllClick()
        {
            if (_phase != Phase.PlayerRolling || _playerRollsLeft == 0 || _matchEnded)
            {
                return;
            }

            _screen.SetPlayerSelectionStatus(_screen.HavePlayerUnselectedDice);
        }

        public void OnRollCompleted()
        {
            switch (_phase)
            {
                case Phase.PlayerRolling:
                    _player.Result.Calculate(_screen.PlayerDices, _standardDeck);
                    _screen.SetPlayerDicePreview(_player.Result.Armor, _player.Result.Damage, _player.Result.Heal);

                    if (_playerRollsLeft == 0)
                    {
                        FinishPlayerTurn();
                    }
                    break;

                case Phase.EnemyRolling:
                    _enemy.Result.Calculate(_screen.EnemyDices, _standardDeck);
                    ContinueEnemyTurn();
                    break;
            }
        }

        private void FinishPlayerTurn()
        {
            _player.Result.Calculate(_screen.PlayerDices, _standardDeck);
            _screen.DisablePlayerDice();
            _screen.ClearPlayerDicePreview();

            Debug.Log($"Турнир — Игрок: DMG={_player.Result.Damage} ARM={_player.Result.Armor} HEAL={_player.Result.Heal}");

            _phase = Phase.EnemyRolling;
            _enemyRollsLeft = RollsPerTurn;
            SignalSystem.Raise<IHintHandler>(handler => handler.Show("Ход противника")); // TODO Localization

            LeanTween.delayedCall(0.5f, StartEnemyTurn);
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
                LeanTween.delayedCall(0.4f, _screen.RerollEnemySelected);
            }
            else
            {
                FinishEnemyTurn();
            }
        }

        private void FinishEnemyTurn()
        {
            _enemy.Result.Calculate(_screen.EnemyDices, _standardDeck);

            Debug.Log($"Турнир — Бот: DMG={_enemy.Result.Damage} ARM={_enemy.Result.Armor} HEAL={_enemy.Result.Heal}");

            _phase = Phase.Resolving;
            ResolveRound();
        }

        private void ResolveRound()
        {
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());

            ApplySide(_player, _screen.PlayerTakeHeal, _screen.EnemyTakeDamage, _screen.EnemyAnimateDamage);

            if (_enemy.Data.CurrentHealth <= 0)
            {
                EndMatch(playerWon: true);
                return;
            }

            ApplySide(_enemy, _screen.EnemyTakeHeal, _screen.PlayerTakeDamage, _screen.PlayerAnimateDamage);

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

        private static void ApplySide(TournamentFighter attacker,
            System.Action<int> heal, System.Action<int> dealDamage, System.Action animateDamage)
        {
            heal(attacker.Result.Heal);
            attacker.Data.Armor = attacker.BaseArmor + attacker.Result.Armor;
            dealDamage(attacker.BaseDamage + attacker.Result.Damage);
            animateDamage();
        }

        private void EndMatch(bool playerWon)
        {
            _matchEnded = true;

            Debug.Log($"Турнир: {(playerWon ? "Победа" : "Поражение")}. " +
                      $"Игрок HP {_player.Data.CurrentHealth}/{_player.Data.MaxHealth}, " +
                      $"Бот HP {_enemy.Data.CurrentHealth}/{_enemy.Data.MaxHealth}");

            DiceRuleset.Reset();
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TavernScreen));
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
