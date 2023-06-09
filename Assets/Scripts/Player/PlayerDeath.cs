using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Roguelike.Data;
using Roguelike.Infrastructure.Services.PersistentData;
using Roguelike.Infrastructure.Services.SaveLoad;
using Roguelike.Infrastructure.Services.Windows;
using Roguelike.Utilities;
using UnityEngine;

namespace Roguelike.Player
{
    [RequireComponent(typeof(PlayerAnimator), typeof(PlayerHealth))]
    public class PlayerDeath : MonoBehaviour, IProgressWriter
    {
        [SerializeField] private float _delayBeforeResurrectionScreen = 1.5f;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private PlayerAnimator _animator;
        [SerializeField] private ParticleSystem _resurrectionEffect;
        [SerializeField] private MonoBehaviour[] _componentsToDeactivate;

        private IWindowService _windowService;
        private ISaveLoadService _saveLoadService;
        private bool _isDead;

        public event Action Resurrected;
        public event Action Died;

        public void Construct(IWindowService windowService, ISaveLoadService saveLoadService)
        {
            _windowService = windowService;
            _saveLoadService = saveLoadService;
        }

        public void WriteProgress(PlayerProgress progress) => 
            progress.State.Dead = _isDead;

        public void ReadProgress(PlayerProgress progress) =>
            _isDead = progress.State.Dead;

        private void Start() =>
            _health.HealthChanged += OnHealthChanged;

        private void OnDestroy() =>
            _health.HealthChanged -= OnHealthChanged;

        private void OnHealthChanged()
        {
            if (_isDead == false && _health.CurrentHealth <= 0)
                Die();
        }

        public void Resurrect()
        {
            SwitchComponents(isOn: true);
            SpawnResurrectionVFX();

            _isDead = false;
            _animator.Restart();

            Resurrected?.Invoke();
            
            _saveLoadService.SaveProgress();
        }

        private void SpawnResurrectionVFX()
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y += 0.1f;
            Instantiate(_resurrectionEffect, spawnPosition, Quaternion.identity);
        }

        private void Die()
        {
            SwitchComponents(isOn: false);
            
            _isDead = true;
            _animator.PlayDeath();
            _saveLoadService.SaveProgress();
            Died?.Invoke();

            StartCoroutine(OpenResurrectionWindowAfterDelay());
        }

        private void SwitchComponents(bool isOn)
        {
            for (int i = 0; i < _componentsToDeactivate.Length; i++) 
                _componentsToDeactivate[i].enabled = isOn;
        }

        private IEnumerator OpenResurrectionWindowAfterDelay()
        {
            yield return Helpers.GetTime(_delayBeforeResurrectionScreen);

            _windowService.OpenResurrectionWindow(this);
        }
    }
}