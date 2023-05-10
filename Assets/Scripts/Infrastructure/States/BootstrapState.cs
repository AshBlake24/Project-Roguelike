using Roguelike.Infrastructure.AssetManagement;
using Roguelike.Infrastructure.Factory;
using Roguelike.Infrastructure.Services;
using Roguelike.Infrastructure.Services.Environment;
using Roguelike.Infrastructure.Services.Input;
using Roguelike.Infrastructure.Services.Loading;
using Roguelike.Infrastructure.Services.PersistentData;
using Roguelike.Infrastructure.Services.Pools;
using Roguelike.Infrastructure.Services.Random;
using Roguelike.Infrastructure.Services.SaveLoad;
using Roguelike.Infrastructure.Services.StaticData;
using Roguelike.Infrastructure.Services.Windows;

namespace Roguelike.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string InitialScene = "Initial";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _services;
        private readonly ICoroutineRunner _coroutineRunner;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, AllServices services,
            ICoroutineRunner coroutineRunner)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _services = services;
            _coroutineRunner = coroutineRunner;

            RegisterServices();
        }

        public void Enter() =>
            _sceneLoader.Load(InitialScene, onLoaded: EnterLoadLevel);

        public void Exit()
        {
        }

        private void EnterLoadLevel() =>
            _stateMachine.Enter<LoadProgressState>();

        private void RegisterServices()
        {
            RegisterStaticData();
            _services.RegisterSingle<IEnvironmentService>(new EnvironmentService());
            _services.RegisterSingle<IRandomService>(new UnityRandomService());
            _services.RegisterSingle<IInputService>(GetInputService());
            _services.RegisterSingle<IParticlesPoolService>(new ParticlesPoolService());
            _services.RegisterSingle<ISceneLoadingService>(new SceneLoadingService(_stateMachine));
            _services.RegisterSingle<IPersistentDataService>(new PersistentDataService(
                _services.Single<IStaticDataService>()));

            _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(
                _services.Single<IPersistentDataService>()));

            _services.RegisterSingle<IAssetProvider>(new AssetProvider(
                _services.Single<ISaveLoadService>()));

            _services.RegisterSingle<IProjectileFactory>(new ProjectileFactory(
                _services.Single<IStaticDataService>()));

            _services.RegisterSingle<ILootFactory>(new LootFactory(
                _services.Single<IAssetProvider>(),
                _services.Single<IRandomService>(),
                _services.Single<IParticlesPoolService>(),
                _services.Single<IStaticDataService>(),
                _coroutineRunner));

            _services.RegisterSingle<IEnemyFactory>(new EnemyFactory(
                _services.Single<IStaticDataService>(),
                _services.Single<ILootFactory>(),
                _services.Single<IRandomService>()));

            _services.RegisterSingle<ISkillFactory>(new SkillFactory(
                _coroutineRunner,
                _services.Single<IStaticDataService>(),
                _services.Single<IPersistentDataService>()));

            _services.RegisterSingle<IWeaponFactory>(new WeaponFactory(
                _services.Single<IStaticDataService>(),
                _services.Single<ISaveLoadService>(),
                _services.Single<IPersistentDataService>(),
                _services.Single<ILootFactory>()));

            _services.RegisterSingle<IUIFactory>(new UIFactory(
                _services.Single<IAssetProvider>(),
                _services.Single<IStaticDataService>(),
                _services.Single<IPersistentDataService>(),
                _services.Single<ISceneLoadingService>()));

            _services.RegisterSingle<IWindowService>(new WindowService(
                _services.Single<IUIFactory>()));

            _services.RegisterSingle<IGameFactory>(new GameFactory(
                _services.Single<IAssetProvider>(),
                _services.Single<IPersistentDataService>(),
                _services.Single<IStaticDataService>(),
                _services.Single<ISaveLoadService>(),
                _services.Single<IWeaponFactory>(),
                _services.Single<ISkillFactory>(),
                _services.Single<IEnemyFactory>(),
                _services.Single<IWindowService>(),
                _services.Single<IEnvironmentService>(),
                _services.Single<ISceneLoadingService>(),
                _services.Single<ILootFactory>()));
        }

        private void RegisterStaticData()
        {
            IStaticDataService staticDataService = new StaticDataService();
            staticDataService.Load();
            _services.RegisterSingle<IStaticDataService>(staticDataService);
        }

        private IInputService GetInputService()
        {
            EnvironmentType deviceType = _services.Single<IEnvironmentService>().GetDeviceType();

            return deviceType == EnvironmentType.Desktop
                ? new DesktopInputService()
                : new MobileInputService();
        }
    }
}