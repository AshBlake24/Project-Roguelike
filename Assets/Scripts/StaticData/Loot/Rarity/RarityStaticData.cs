using System;
using Roguelike.Infrastructure.Services.StaticData;
using UnityEngine;

namespace Roguelike.StaticData.Loot.Rarity
{
    [CreateAssetMenu(fileName = "New Rarity", menuName = "Static Data/Loot/New Rarity", order = 0)]
    public class RarityStaticData : ScriptableObject, IStaticData
    {
        public RarityId Id;
        public int Weight;
        public Color Color;
        public ParticleSystem GlowVFX;
        public ParticleSystem RingVFX;
        
        public Enum Key => Id;
    }
}