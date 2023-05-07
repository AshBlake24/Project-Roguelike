using System.Collections.Generic;
using System.Linq;
using Roguelike.Infrastructure.AssetManagement;
using Roguelike.Infrastructure.Services.Windows;
using Roguelike.StaticData;
using Roguelike.StaticData.Characters;
using Roguelike.StaticData.Enemies;
using Roguelike.StaticData.Levels;
using Roguelike.StaticData.Loot;
using Roguelike.StaticData.Player;
using Roguelike.StaticData.Projectiles;
using Roguelike.StaticData.Skills;
using Roguelike.StaticData.Weapons;
using Roguelike.StaticData.Weapons.PickupableWeapons;
using Roguelike.StaticData.Windows;
using UnityEngine;

namespace Roguelike.Infrastructure.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private Dictionary<WeaponId, WeaponStaticData> _weapons;
        private Dictionary<ProjectileId, ProjectileStaticData> _projectiles;
        private Dictionary<CharacterId, CharacterStaticData> _characters;
        private Dictionary<SkillId, SkillStaticData> _skills;
        private Dictionary<WindowId, WindowConfig> _windows;
        private Dictionary<EnemyId, EnemyStaticData> _enemies;
        private Dictionary<StageId, LevelStaticData> _levels;
        private Dictionary<LootId, LootStaticData> _loot;
        private Dictionary<WeaponId, PickupableWeaponsConfig> _pickupableWeapons;

        public PlayerStaticData Player { get; private set; }
        public GameConfig GameConfig { get; private set; }

        public void Load()
        {
            LoadWeapons();
            LoadProjectiles();
            LoadCharacters();
            LoadSkills();
            LoadWindows();
            LoadEnemies();
            LoadLevels();
            LoadPlayer();
            LoadLoot();
            LoadPickupableWeapons();
            LoadGameConfig();
        }

        public WeaponStaticData GetWeaponData(WeaponId id) =>
            _weapons.TryGetValue(id, out WeaponStaticData staticData)
                ? staticData
                : null;

        public ProjectileStaticData GetProjectileData(ProjectileId id) =>
            _projectiles.TryGetValue(id, out ProjectileStaticData staticData)
                ? staticData
                : null;

        public CharacterStaticData GetCharacterData(CharacterId id) =>
            _characters.TryGetValue(id, out CharacterStaticData staticData)
                ? staticData
                : null;

        public SkillStaticData GetSkillStaticData(SkillId id) =>
            _skills.TryGetValue(id, out SkillStaticData staticData)
                ? staticData
                : null;

        public EnemyStaticData GetEnemyStaticData(EnemyId id) =>
            _enemies.TryGetValue(id, out EnemyStaticData staticData)
                ? staticData
                : null;

        public WindowConfig GetWindowConfig(WindowId id) =>
            _windows.TryGetValue(id, out WindowConfig windowConfig)
                ? windowConfig
                : null;

        public LevelStaticData GetLevelStaticData(StageId id) =>
            _levels.TryGetValue(id, out LevelStaticData staticData)
                ? staticData
                : null;

        public LootStaticData GetLootStaticData(LootId id) =>
            _loot.TryGetValue(id, out LootStaticData lootData)
                ? lootData
                : null;

        public PickupableWeaponsConfig GetPickupableWeaponConfig(WeaponId id) =>
            _pickupableWeapons.TryGetValue(id, out PickupableWeaponsConfig pickupableWeaponsConfig)
                ? pickupableWeaponsConfig
                : null;

        private void LoadWeapons() =>
            _weapons = Resources.LoadAll<WeaponStaticData>(AssetPath.WeaponsStaticDataPath)
                .ToDictionary(weapon => weapon.Id);

        private void LoadProjectiles() =>
            _projectiles = Resources.LoadAll<ProjectileStaticData>(AssetPath.ProjectilesStaticDataPath)
                .ToDictionary(projectile => projectile.Id);

        private void LoadCharacters() =>
            _characters = Resources.LoadAll<CharacterStaticData>(AssetPath.CharactersStaticDataPath)
                .ToDictionary(character => character.Id);

        private void LoadSkills() =>
            _skills = Resources.LoadAll<SkillStaticData>(AssetPath.SkillsStaticDataPath)
                .ToDictionary(skill => skill.Id);

        private void LoadEnemies() =>
            _enemies = Resources.LoadAll<EnemyStaticData>(AssetPath.EnemiesPath)
                .ToDictionary(enemy => enemy.Id);

        private void LoadLevels() =>
            _levels = Resources.LoadAll<LevelStaticData>(AssetPath.LevelsPath)
                .ToDictionary(level => level.Id);

        private void LoadWindows() =>
            _windows = Resources.Load<WindowStaticData>(AssetPath.WindowsStaticDataPath)
                .Configs
                .ToDictionary(config => config.WindowId, x => x);

        private void LoadLoot() =>
            _loot = Resources.LoadAll<LootStaticData>(AssetPath.LootPath)
                .ToDictionary(loot => loot.Id);

        private void LoadPickupableWeapons() =>
            _pickupableWeapons = Resources.Load<PickupableWeaponsStaticData>(AssetPath.PickupableWeaponStaticDataPath)
                .Configs
                .ToDictionary(config => config.Id, x => x);

        private void LoadPlayer() =>
            Player = Resources.Load<PlayerStaticData>(AssetPath.PlayerStaticDataPath);

        private void LoadGameConfig() =>
            GameConfig = Resources.Load<GameConfig>(AssetPath.GameConfigPath);
    }
}