using Roguelike.Weapons.Stats;
using UnityEngine;

namespace Roguelike.StaticData.Weapons
{
    public abstract class WeaponStaticData : ScriptableObject
    {
        [Header("Stats")]
        public WeaponId Id;
        public WeaponType Type;
        public WeaponSize Size;
        public GameObject WeaponPrefab;
        public Sprite Icon;
        public string Name;
        
        
        [Range(0.05f, 2f)] public float AttackRate;
    }
}