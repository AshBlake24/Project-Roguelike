using Roguelike.Infrastructure.Services.Windows;
using Roguelike.Player.Skills;
using Roguelike.StaticData.Characters;
using Roguelike.StaticData.Player;
using Roguelike.StaticData.Projectiles;
using Roguelike.StaticData.Skills;
using Roguelike.StaticData.Weapons;
using Roguelike.StaticData.Windows;

namespace Roguelike.Infrastructure.Services.StaticData
{
    public interface IStaticDataService : IService
    {
        PlayerStaticData Player { get; }
        void Load();
        WeaponStaticData GetWeaponData(WeaponId id);
        ProjectileStaticData GetProjectileData(ProjectileId id);
        CharacterStaticData GetCharacterData(CharacterId id);
        SkillStaticData GetSkillStaticData(SkillId id);
        WindowConfig GetWindowConfig(WindowId pauseMenu);
    }
}