using System;
using Roguelike.Infrastructure.Services.StaticData;
using Roguelike.Localization;
using UnityEngine;

namespace Roguelike.StaticData.Enhancements
{
    [CreateAssetMenu(
        fileName = "New Enhancement", 
        menuName = "Static Data/Player/Enhancements/New Enhancement", 
        order = 0)]
    public class EnhancementStaticData : ScriptableObject, IStaticData
    {
        [Header("Stats")]
        public EnhancementId Id;
        public TierInfo[] Tiers;

        [Header("Info")] 
        public Sprite Icon;
        public LocalizedString Name;
        public LocalizedString Description;

        public Enum Key => Id;
    }
}