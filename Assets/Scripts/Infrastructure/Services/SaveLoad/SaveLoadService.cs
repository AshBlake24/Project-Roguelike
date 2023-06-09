using System;
using System.Collections.Generic;
using Agava.YandexGames;
using Roguelike.Data;
using Roguelike.Infrastructure.Services.PersistentData;
using Roguelike.Leaderboard;
using Roguelike.StaticData.Levels;
using Roguelike.Utilities;
using UnityEngine;

namespace Roguelike.Infrastructure.Services.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string PlayerProgressKey = "PlayerProgress";
        
        private readonly IPersistentDataService _progressService;
        private readonly List<IProgressReader> _progressReaders;
        private readonly List<IProgressWriter> _progressWriters;
        
        public SaveLoadService(IPersistentDataService progressService)
        {
            _progressService = progressService;
            _progressReaders = new List<IProgressReader>();
            _progressWriters = new List<IProgressWriter>();
        }
        
        public IEnumerable<IProgressReader> ProgressReaders => _progressReaders;
        public IEnumerable<IProgressWriter> ProgressWriters => _progressWriters;

        public void SaveProgress()
        {
            foreach (IProgressWriter progressWriter in ProgressWriters)
                progressWriter.WriteProgress(_progressService.PlayerProgress);

            string dataToStore = _progressService.PlayerProgress.ToJson(prettyPrint: false);
            PlayerPrefs.SetString(PlayerProgressKey, dataToStore);
            PlayerPrefs.Save();
            
#if UNITY_WEBGL && !UNITY_EDITOR
            if (PlayerAccount.IsAuthorized)
            {
                PlayerAccount.SetCloudSaveData(dataToStore);
                SetPlayerScore();
            }
#endif
        }

        public void InformProgressReaders()
        {
            foreach (IProgressReader progressReader in ProgressReaders)
                progressReader.ReadProgress(_progressService.PlayerProgress);
        }

        public PlayerProgress LoadProgressFromPrefs() => 
            PlayerPrefs.GetString(PlayerProgressKey)?
                .FromJson<PlayerProgress>();

        public void LoadProgressFromCloud(Action<PlayerProgress> onLoaded) => 
            PlayerAccount.GetCloudSaveData((data) => OnDataLoaded(data, onLoaded));

        public void Cleanup()
        {
            _progressReaders.Clear();
            _progressWriters.Clear();
        }

        public void RegisterProgressWatchers(GameObject gameObject)
        {
            foreach (IProgressReader progressReader in gameObject.GetComponentsInChildren<IProgressReader>())
                Register(progressReader);
        }

        private void Register(IProgressReader progressReader)
        {
            if (progressReader is IProgressWriter progressWriter)
                _progressWriters.Add(progressWriter);
            
            _progressReaders.Add(progressReader);
        }

        private void SetPlayerScore()
        {
            Agava.YandexGames.Leaderboard.GetPlayerEntry(LeaderboardView.LeaderboardName, (result) =>
            {
                int playerScore = _progressService.PlayerProgress.Statistics.PlayerScore;

                if (result == null || result.score < playerScore)
                    Agava.YandexGames.Leaderboard.SetScore(LeaderboardView.LeaderboardName, playerScore);
            });
        }

        private void OnDataLoaded(string data, Action<PlayerProgress> onLoaded) =>
            onLoaded.Invoke(string.IsNullOrEmpty(data)
                ? null
                : data?.FromJson<PlayerProgress>());
    }
}