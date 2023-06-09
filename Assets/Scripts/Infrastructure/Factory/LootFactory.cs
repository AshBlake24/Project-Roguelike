using System;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Audio.Factory;
using Roguelike.Audio.Sounds.Loot;
using Roguelike.Infrastructure.AssetManagement;
using Roguelike.Infrastructure.Services.PersistentData;
using Roguelike.Infrastructure.Services.Pools;
using Roguelike.Infrastructure.Services.Random;
using Roguelike.Infrastructure.Services.StaticData;
using Roguelike.Logic.Interactables;
using Roguelike.Loot.Powerups;
using Roguelike.StaticData.Loot.Powerups;
using Roguelike.StaticData.Loot.Rarity;
using Roguelike.StaticData.Weapons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Roguelike.Infrastructure.Factory
{
    public class LootFactory : ILootFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IRandomService _randomService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IAudioFactory _audioFactory;
        private readonly IPersistentDataService _persistentDataService;
        private readonly IParticlesPoolService _particlesPoolService;
        private readonly IStaticDataService _staticData;
        private readonly Dictionary<WeaponId, int> _weaponsDropWeights = new();
        
        private List<PowerupConfig> _powerupDropTable;
        private int _weaponsTotalWeight;
        private int _powerupsTotalWeight;

        public LootFactory(IAssetProvider assetProvider, IRandomService randomService,
            IParticlesPoolService particlesPoolService,
            IStaticDataService staticData, IAudioFactory audioFactory, IPersistentDataService persistentDataService,
            ICoroutineRunner coroutineRunner)
        {
            _assetProvider = assetProvider;
            _randomService = randomService;
            _coroutineRunner = coroutineRunner;
            _audioFactory = audioFactory;
            _persistentDataService = persistentDataService;
            _particlesPoolService = particlesPoolService;
            _staticData = staticData;
            LoadWeaponsDropWeights();
            LoadPowerupDropTable();
        }

        public void CreateRandomPowerup(Vector3 position) => 
            CreatePowerup(GetDroppedPowerup(), position);

        public void CreateConcretePowerup(PowerupId powerupId, Vector3 position) => 
            CreatePowerup(powerupId, position);

        public GameObject CreateRandomWeapon(Vector3 position, RarityId minimalRarity)
        {
            WeaponId weaponId;
            WeaponStaticData weaponData;
            
            do
            {
                weaponId = GetDroppedWeapon();
                weaponData = _staticData.GetDataById<WeaponId, WeaponStaticData>(weaponId);
            } 
            while (weaponData.Rarity < minimalRarity);

            return CreateWeapon(weaponId, position);
        }

        public GameObject CreateConcreteWeapon(WeaponId weaponId, Vector3 position) => 
            CreateWeapon(weaponId, position);

        private void CreatePowerup(PowerupId powerupId, Vector3 position)
        {
            PowerupStaticData powerupData = _staticData.GetDataById<PowerupId, PowerupStaticData>(powerupId);

            Powerup powerup = Object.Instantiate(powerupData.Prefab, position, Quaternion.identity);
            
            powerup.GetComponent<Powerup>()
                .Construct(_particlesPoolService, powerupId, powerupData.Effect, 
                    _persistentDataService.PlayerProgress.Statistics, powerupData.ActiveVFX);
            
            powerup.GetComponent<PowerUpAudioPlayer>()
                .Construct(_audioFactory, powerupData.AudioClip);

            if (powerupData.Effect is ILastingEffect lastingEffect)
                lastingEffect.Construct(_coroutineRunner);
        }

        private GameObject CreateWeapon(WeaponId weaponId, Vector3 position)
        {
            WeaponStaticData weaponData = _staticData.GetDataById<WeaponId, WeaponStaticData>(weaponId);
            RarityStaticData rarityData = _staticData.GetDataById<RarityId, RarityStaticData>(weaponData.Rarity);

            InteractableWeapon interactableWeapon = _assetProvider
                .Instantiate(AssetPath.InteractableWeaponPath, position)
                .GetComponent<InteractableWeapon>();

            Object.Instantiate(rarityData.RingVFX, interactableWeapon.transform);
            Object.Instantiate(rarityData.GlowVFX, interactableWeapon.transform);
            GameObject model = Object.Instantiate(weaponData.InteractableWeaponPrefab, interactableWeapon.ModelContainer);
            
            interactableWeapon.Construct(weaponId, model.GetComponent<Outline>());
            
            return interactableWeapon.gameObject;
        }

        private PowerupId GetDroppedPowerup()
        {
            int roll = _randomService.Next(0, _powerupsTotalWeight);

            foreach (PowerupConfig powerup in _powerupDropTable)
            {
                roll -= powerup.Weight;

                if (roll < 0)
                    return powerup.Id;
            }

            throw new ArgumentOutOfRangeException(nameof(_powerupDropTable), "Incorrectly placed weights");
        }

        private WeaponId GetDroppedWeapon()
        {
            int roll = _randomService.Next(0, _weaponsTotalWeight);
            
            foreach ((WeaponId weaponId, int weight) in _weaponsDropWeights)
            {
                roll -= weight;

                if (roll < 0)
                    return weaponId;
            }

            throw new ArgumentOutOfRangeException(nameof(_weaponsDropWeights), "Incorrectly placed weights");
        }
        
        private void LoadWeaponsDropWeights()
        {
            int weaponsCount = Enum.GetValues(typeof(WeaponId)).Length - 1;
            
            for (int i = 0; i < weaponsCount; i++)
            {
                WeaponStaticData weaponData = _staticData.GetDataById<WeaponId, WeaponStaticData>((WeaponId) i);
                RarityStaticData rarityData = _staticData.GetDataById<RarityId, RarityStaticData>(weaponData.Rarity);

                _weaponsDropWeights.Add(weaponData.Id, rarityData.Weight);
            }
            
            _weaponsTotalWeight = _weaponsDropWeights.Sum(x => x.Value);
        }
        
        private void LoadPowerupDropTable()
        {
            _powerupDropTable = Resources.Load<PowerupDropTable>(AssetPath.PowerupDropTablePath).PowerupConfigs;
            _powerupsTotalWeight = _powerupDropTable.Sum(x => x.Weight);
        }
    }
}