using Roguelike.Infrastructure.Services.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Roguelike.UI.Buttons
{
    public class OpenWindowButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private WindowId _windowId;
        
        private IWindowService _windowService;

        public void Construct(IWindowService windowService) => 
            _windowService = windowService;

        private void Awake() => 
            _button.onClick.AddListener(Open);

        private void OnDestroy() => 
            _button.onClick.RemoveAllListeners();

        private void Open() => 
            _windowService.Open(_windowId);
    }
}