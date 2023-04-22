using System;
using Roguelike.Enemies;
using Roguelike.Logic;
using Roguelike.Weapons.Projectiles.Stats;
using UnityEngine;
using UnityEngine.Pool;

namespace Roguelike.Weapons.Projectiles
{
    public class Bullet : Projectile
    {
        private BulletStats _stats;

        protected override ProjectileStats Stats => _stats;

        public override void Construct<TStats>(TStats stats, IObjectPool<Projectile> bulletPool)
        {
            InitializeBulletStats(stats);
            base.Construct(stats, bulletPool);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IHealth health))
                health.TakeDamage(_stats.Damage);

            SpawnVFX(ImpactVFXKey);
            ReturnToPool();
        }
        
        private void InitializeBulletStats<TStats>(TStats stats)
        {
            if (stats is BulletStats bulletStats)
                _stats = bulletStats;
            else
                throw new ArgumentNullException(nameof(stats), $"Expected to get the {typeof(BulletStats)}");
        }
    }
}