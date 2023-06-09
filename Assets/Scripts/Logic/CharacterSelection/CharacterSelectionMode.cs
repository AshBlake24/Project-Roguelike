using System;
using System.Collections;
using Cinemachine;
using Roguelike.Infrastructure.Factory;
using Roguelike.Infrastructure.Services.SaveLoad;
using Roguelike.Infrastructure.Services.Windows;
using Roguelike.Tutorials;
using Roguelike.UI.Buttons;
using Roguelike.UI.Windows;
using Roguelike.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Roguelike.Logic.CharacterSelection
{
    public class CharacterSelectionMode : MonoBehaviour
    {
        private const int RayMaxDistance = 100;

        [SerializeField] private CinemachineVirtualCamera _topDownCamera;
        [SerializeField] private CinemachineVirtualCamera _characterSelectionCamera;
        [SerializeField] private Button _characterSelectionButton;
        [SerializeField] private float _delayBeforeShowingTutorial;

        private IWindowService _windowService;
        private IGameFactory _gameFactory;
        private ISaveLoadService _saveLoadService;
        private ITutorialService _tutorialService;
        private RaycastHit _raycastHit;
        private Camera _camera;
        private BaseWindow _selectionWindow;
        private Coroutine _tutorialCoroutine;
        private bool _isActive;
        private bool _characterSelected;

        public void Construct(IGameFactory gameFactory, IWindowService windowService,
            ISaveLoadService saveLoadService, ITutorialService tutorialService, BaseWindow selectionWindow)
        {
            _windowService = windowService;
            _gameFactory = gameFactory;
            _saveLoadService = saveLoadService;
            _selectionWindow = selectionWindow;
            _isActive = true;
            _characterSelected = false;
            _tutorialService = tutorialService;
        }

        private void OnEnable() => 
            LoadingScreen.Hided += OnLoadingScreenHided;

        private void OnDestroy()
        {
            StopAllCoroutines();
            LoadingScreen.Hided -= OnLoadingScreenHided;
        }

        private void Start() => 
            _camera = Camera.main;

        private void Update()
        {
            if (_isActive == false)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out _raycastHit, RayMaxDistance))
                {
                    if (_raycastHit.collider.TryGetComponent(out SelectableCharacter character))
                    {
                        CreateCharacterStatsWindow(character);
                        ZoomIn(_raycastHit.collider.transform);
                    }
                }
            }
        }

        public void OnCharacterSelected()
        {
            Transform spawnPoint = _raycastHit.transform;
            Destroy(_raycastHit.collider.gameObject);

            GameObject player = InitPlayer(spawnPoint);
            InitHud(player);
            InitCamera(player);
            
            _tutorialService.TryShowTutorial(TutorialId.Hub01);
            _saveLoadService.InformProgressReaders();
            _characterSelected = true;
            enabled = false;
        }

        public void ZoomOut()
        {
            StopCoroutine(_tutorialCoroutine);
            
            if (_characterSelected)
                return;

            _isActive = true;
            _selectionWindow.gameObject.SetActive(true);
            _topDownCamera.enabled = true;
        }

        private void ZoomIn(Transform character)
        {
            _isActive = false;
            _selectionWindow.gameObject.SetActive(false);
            _characterSelectionCamera.Follow = character;
            _characterSelectionCamera.LookAt = character;
            _topDownCamera.enabled = false;
            
            if (_tutorialCoroutine != null)
                StopCoroutine(_tutorialCoroutine);
            
            _tutorialCoroutine = StartCoroutine(TryShowTutorial(TutorialId.CharacterStats01));
        }

        private void CreateCharacterStatsWindow(SelectableCharacter character)
        {
            BaseWindow window = _windowService.Open(WindowId.CharacterStats);

            if (window is CharacterStats characterStats)
                characterStats.Init(
                    character.Id,
                    this);
            else
                throw new ArgumentNullException(nameof(window), "The necessary component is missing");
        }

        private void InitCamera(GameObject player)
        {
            _topDownCamera.enabled = false;
            _characterSelectionCamera.enabled = false;
            _gameFactory.CreatePlayerCamera(player);
        }

        private GameObject InitPlayer(Transform spawnPoint) =>
            _gameFactory.CreatePlayer(spawnPoint);

        private void InitHud(GameObject player)
        {
            GameObject hud = _gameFactory.CreateHud(player, createMiniMap: false);
            Button button = Instantiate(_characterSelectionButton, hud.transform);
            
            if (button.TryGetComponent(out OpenWindowButton openWindowButton))
                openWindowButton.Construct(_windowService);
        }

        private IEnumerator TryShowTutorial(TutorialId tutorialId)
        {
            yield return Helpers.GetTime(_delayBeforeShowingTutorial);
            
            _tutorialService.TryShowTutorial(tutorialId);
        }
        
        private void OnLoadingScreenHided() => 
            _tutorialCoroutine = StartCoroutine(TryShowTutorial(TutorialId.Welcome01));
    }
}