using Roguelike.Data;
using Roguelike.Infrastructure.Services.StaticData;
using Roguelike.Infrastructure.States;
using Roguelike.StaticData.Levels;
using Roguelike.Utilities;
using UnityEngine;

namespace Roguelike.Infrastructure.Services.PersistentData
{
    public class PersistentDataService : IPersistentDataService
    {
        private readonly IStaticDataService _staticData;
        private readonly GameStateMachine _stateMachine;
        
        public bool IsResetting { get; private set; }

        public PersistentDataService(IStaticDataService staticData, GameStateMachine stateMachine)
        {
            _staticData = staticData;
            _stateMachine = stateMachine;
            IsResetting = false;
        }

        public PlayerProgress PlayerProgress { get; set; }
        
        public void UpdateStatistics()
        {
            LevelId levelId = EnumExtensions.GetCurrentLevelId();
            int coins = PlayerProgress.Balance.DungeonBalance.Coins;
            
            PlayerProgress.Statistics.Favourites.AddWeapons(PlayerProgress.PlayerWeapons.Weapons);
            PlayerProgress.Statistics.KillData.TrySaveOverallKills(levelId);
            PlayerProgress.Statistics.CollectablesData.AddCoins(coins);
            PlayerProgress.Balance.HubBalance.ConvertDungeonToHubCoins(coins);

            Reset();
        }

        public void Reset()
        {
            PlayerProgress.State.Reset();
            PlayerProgress.Balance.DungeonBalance.Reset();
            PlayerProgress.Statistics.KillData.CurrentKillData.Reset();
            PlayerProgress.WorldData = new WorldData(_staticData.GameConfig.StartLevel);
        }

        public void ResetAllProgress()
        {
            IsResetting = true;
            PlayerPrefs.DeleteAll();
            _stateMachine.Enter<LoadProgressState>();
        }

        public void ResetTutorial() => 
            PlayerProgress.TutorialData = new TutorialData();
    }
}