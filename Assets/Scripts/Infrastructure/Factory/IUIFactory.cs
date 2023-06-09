using System;
using Roguelike.Data;
using Roguelike.Infrastructure.Services;
using Roguelike.Infrastructure.Services.Windows;
using Roguelike.Loot.Chest;
using Roguelike.Player;
using Roguelike.StaticData.Weapons;
using Roguelike.UI.Elements;
using Roguelike.UI.Windows;
using Roguelike.UI.Windows.Enhancements;
using UnityEngine;

namespace Roguelike.Infrastructure.Factory
{
    public interface IUIFactory : IService
    {
        BaseWindow CreateWindow<TKey>(IWindowService windowService, TKey windowId, bool isTutorial) where TKey : Enum;
        GameObject CreateWeaponStatsViewer(IWindowService windowService, WeaponId weaponId);
        EnhancementShopWindow CreateEnhancementShop(IWindowService windowService, PlayerEnhancements playerEnhancements);
        void CreateResurrectionWindow(IWindowService windowService, PlayerDeath playerDeath);
        void CreateEnhancementWidget(EnhancementData enhancementProgress, Transform parent, 
            EnhancementTooltip tooltip);

        BaseWindow CreateWeaponChestWindow(WindowService windowService, SalableWeaponChest salableWeaponChest);
        void CreateTutorialRoot();
        void CreateUIRoot();
        void ShowStageName();
    }
}