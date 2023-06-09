﻿using Roguelike.Infrastructure.Services;
using Roguelike.Player;
using Roguelike.StaticData.Enemies;
using Roguelike.UI.Elements;
using UnityEngine;

namespace Roguelike.Infrastructure.Factory
{
    public interface IEnemyFactory : IService
    {
        public GameObject CreateMinion(Transform spawnPoint, EnemyId id, PlayerHealth target);

        public GameObject CreateBoss(Transform spawnPoint, EnemyId id, PlayerHealth target);
    }
}
