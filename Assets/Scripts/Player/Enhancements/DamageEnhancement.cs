using Roguelike.StaticData.Enhancements;

namespace Roguelike.Player.Enhancements
{
    public sealed class DamageEnhancement : Enhancement
    {
        private readonly PlayerShooter _playerShooter;

        public DamageEnhancement(EnhancementStaticData enhancementStaticData, PlayerShooter playerShooter) : 
            base(enhancementStaticData)
        {
            _playerShooter = playerShooter;
            _playerShooter.WeaponChanged += OnWeaponChanged;
        }

        public override void Apply()
        {
            if (_playerShooter.CurrentWeapon is IEnhanceable<int> weapon)
                weapon.Enhance(Data.ValuesOnTiers[CurrentTier - 1]);
        }

        public override void Cleanup() => 
            _playerShooter.WeaponChanged -= OnWeaponChanged;

        private void OnWeaponChanged() => 
            Apply();
    }
}