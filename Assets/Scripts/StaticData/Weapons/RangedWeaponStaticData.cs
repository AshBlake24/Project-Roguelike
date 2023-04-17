using System;
using Roguelike.StaticData.Projectiles;
using Roguelike.Weapons.Projectiles;
using UnityEngine;

namespace Roguelike.StaticData.Weapons
{
    [CreateAssetMenu(fileName = "New RangedWeaponData", menuName = "Static Data/Weapons/Ranged Weapon")]
    public class RangedWeaponStaticData : WeaponStaticData
    {
        [Header("Ammo")]
        public ProjectileStaticData Projectile;
        public bool InfinityAmmo;
        public int MaxAmmo;
    }
}