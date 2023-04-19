using Roguelike.Data;
using Roguelike.Infrastructure.Factory;
using Roguelike.Infrastructure.Services;
using Roguelike.Infrastructure.Services.PersistentData;
using Roguelike.Logic;
using Roguelike.Utilities;
using Roguelike.Weapons.Projectiles;
using Roguelike.Weapons.Stats;
using UnityEngine;

namespace Roguelike.Weapons
{
    public class RangedWeapon : Weapon, IProgressWriter
    {
        [SerializeField] private Transform _firePoint;

        private IProjectileFactory _factory;
        private RangedWeaponStats _stats;
        private ObjectPool<Projectile> _projectilesPool;
        private ObjectPool<ParticleSystem> _muzzleFlashPool;
        private ParticleSystem _muzzleVFX;

        public override WeaponStats Stats => _stats;
        public int CurrentAmmo { get; private set; }
        public bool InfinityAmmo { get; private set; }

        private void Awake()
        {
            _factory = AllServices.Container.Single<IProjectileFactory>();
        }

        public void Construct(RangedWeaponStats stats)
        {
            _stats = stats;
            InfinityAmmo = stats.InfinityAmmo;
            CurrentAmmo = stats.MaxAmmo;

            _projectilesPool = new ObjectPool<Projectile>(_stats.ProjectileData.Prefab);
            _muzzleFlashPool = new ObjectPool<ParticleSystem>(_stats.ProjectileData.MuzzleFlashVFX.gameObject);
        }

        public void ReadProgress(PlayerProgress progress)
        {
            AmmoData ammoData = progress.PlayerWeapons.Ammo.Find(weapon => weapon.ID == Stats.ID);

            if (ammoData != null)
            {
                InfinityAmmo = ammoData.InfinityAmmo;
                CurrentAmmo = ammoData.CurrentAmmo;
            }
        }

        public void WriteProgress(PlayerProgress progress) =>
            progress.PlayerWeapons.SaveRangedWeapon(Stats.ID, InfinityAmmo, CurrentAmmo);

        public override bool TryAttack()
        {
            if (InfinityAmmo)
            {
                Shot();
                Debug.Log($"{_stats.Name} shot! Bullets amount: Infinity");

                return true;
            }

            if (CurrentAmmo > 0)
            {
                Shot();
                CurrentAmmo--;
                Debug.Log($"{_stats.Name} shot! Bullets amount: {CurrentAmmo}");

                return true;
            }

            Debug.Log("Not enough ammo");

            return false;
        }

        private void Shot()
        {
            Projectile projectile = _projectilesPool.HasObjects
                ? _projectilesPool.GetInstance()
                : GetProjectile();

            projectile.transform.SetPositionAndRotation(_firePoint.position, _firePoint.rotation);
            projectile.gameObject.SetActive(true);
            projectile.Init();

            SpawnMuzzleVFX();
        }

        private void SpawnMuzzleVFX()
        {
            if (_muzzleFlashPool.HasObjects)
            {
                _muzzleVFX = _muzzleFlashPool.GetInstance();
            } 
            else
            {
                _muzzleVFX = Instantiate(_stats.ProjectileData.MuzzleFlashVFX);
                //_muzzleVFX.Counstruct(_muzzleFlashPool);
            }

            _muzzleVFX.transform.SetPositionAndRotation(_firePoint.position, _firePoint.rotation);
            _muzzleVFX.transform.SetParent(_firePoint);
            _muzzleVFX.gameObject.SetActive(true);
        }

        private Projectile GetProjectile() =>
            _factory.CreateProjectile(_stats.ProjectileData.Id, _projectilesPool);
    }
}