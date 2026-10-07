using Assets.SimpleLocalization.Scripts;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.Localization;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DiceBattle.UI
{
    public class Innkeeper : MonoBehaviour
    {
        private const int RegularVictories = 3;
        private const int FriendVictories = 6;
        private const int HeroVictories = 10;
        private const float RumorChance = 0.35f;
        private const float TipChance = 0.5f;

        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _quest;

        public void ShowMessage(GameConfig config)
        {
            AnimateIn();

            _message.text = LocalizationManager.Localize(PickKey(config));
        }

        private void Start()
        {
            _quest.onClick.AddListener(HandleQuestClicked);
        }

        private void OnDestroy()
        {
            _quest.onClick.RemoveAllListeners();
        }

        private void HandleQuestClicked()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.QuestWindow));
        }

        private void AnimateIn()
        {
            // TODO Add animation
        }

        private static string PickKey(GameConfig config)
        {
            InnkeeperEvent pendingEvent = GameData.PendingInnkeeperEvent;

            if (pendingEvent != InnkeeperEvent.None)
            {
                GameData.PendingInnkeeperEvent = InnkeeperEvent.None;
                return RandomKey(GetEventPrefix(pendingEvent));
            }

            if (GameData.HasEverRolledDice == false)
            {
                return RandomKey(LocKeys.Innkeeper.Newcomer);
            }

            bool isFullClear = GameData.CompletedLevels >= config.Enemies.Count;
            string stagePrefix = GetStagePrefix(isFullClear);

            if (stagePrefix == LocKeys.Innkeeper.Traveler && Random.value < TipChance)
            {
                return RandomKey(LocKeys.Innkeeper.Tip);
            }

            if (isFullClear == false && Random.value < RumorChance)
            {
                string rumorPrefix = LocKeys.Innkeeper.RumorPrefix + config.Enemies[GameData.CompletedLevels].GetFamily();

                if (LocalizationManager.CountIndexedKeys(rumorPrefix) > 0)
                {
                    return RandomKey(rumorPrefix);
                }
            }

            return RandomKey(stagePrefix);
        }

        private static string GetEventPrefix(InnkeeperEvent innkeeperEvent) => innkeeperEvent switch
        {
            InnkeeperEvent.Defeat => LocKeys.Innkeeper.AfterDefeat,
            InnkeeperEvent.CampaignWon => LocKeys.Innkeeper.AfterDragon,
            InnkeeperEvent.TournamentWon => LocKeys.Innkeeper.AfterTournamentWin,
            _ => LocKeys.Innkeeper.AfterTournamentLoss,
        };

        private static string GetStagePrefix(bool isFullClear)
        {
            if (isFullClear || GameData.NewGamePlusCycle > 0)
            {
                return LocKeys.Innkeeper.Legend;
            }

            return GameData.TotalVictories switch
            {
                >= HeroVictories => LocKeys.Innkeeper.Hero,
                >= FriendVictories => LocKeys.Innkeeper.Friend,
                >= RegularVictories => LocKeys.Innkeeper.Regular,
                _ => LocKeys.Innkeeper.Traveler,
            };
        }

        private static string RandomKey(string prefix) =>
            $"{prefix}[{Random.Range(0, LocalizationManager.CountIndexedKeys(prefix))}]";
    }
}
