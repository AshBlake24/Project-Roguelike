﻿using Roguelike.Audio.Factory;
using Roguelike.Audio.Sounds;
using Roguelike.Enemies;
using Roguelike.Enemies.EnemyStates;
using Roguelike.Infrastructure.Services.Random;
using Roguelike.Infrastructure.Services.StaticData;
using Roguelike.Player;
using Roguelike.StaticData.Enemies;
using Roguelike.UI.Elements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Roguelike.Infrastructure.Factory
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly ILootFactory _lootFactory;
        private readonly IRandomService _randomService;
        private readonly IAudioFactory _audioFactory;

        private EnemyStaticData _enemyData;
        private GameObject _enemyPrefab;
        private Enemy _enemy;

        public EnemyFactory(IStaticDataService staticDataService, ILootFactory lootFactory,
            IRandomService randomService, IAudioFactory audioFactory)
        {
            _staticDataService = staticDataService;
            _lootFactory = lootFactory;
            _randomService = randomService;
            _audioFactory = audioFactory;
        }

        public GameObject CreateMinion(Transform spawnPoint, EnemyId id, PlayerHealth target)
        {
            CreateEnemy(spawnPoint, id, target);

            _enemy.Health.Init(_enemyData);

            return _enemyPrefab;
        }

        public GameObject CreateBoss(Transform spawnPoint, EnemyId id, PlayerHealth target)
        {
            CreateEnemy(spawnPoint, id, target);

            _enemyPrefab.GetComponent<BossRoot>().Init(_enemy);

            _enemy.Health.Init(_enemyData);

            _enemyPrefab.GetComponent<BossStage>().Init(_enemy);

            return _enemyPrefab;
        }

        private void CreateEnemy(Transform spawnPoint, EnemyId id, PlayerHealth target)
        {
            _enemyData = _staticDataService.GetDataById<EnemyId, EnemyStaticData>(id);
            _enemyPrefab = Object.Instantiate(_enemyData.Prefab, spawnPoint);

            _enemy = new Enemy(_enemyData, _enemyPrefab.GetComponentInChildren<EnemyHealth>(), target);

            _enemyPrefab.GetComponentInChildren<EnemyStateMachine>().Init(_enemy);
            EnemyLootSpawner lootSpawner = _enemyPrefab.GetComponentInChildren<EnemyLootSpawner>();
            
            if (lootSpawner != null)
                lootSpawner.Construct(_lootFactory, _randomService);

            if (_enemyPrefab.TryGetComponent(out AudioPlayer audioPlayer))
                audioPlayer.Construct(_audioFactory, _enemyData.Sound);
        }
    }
}