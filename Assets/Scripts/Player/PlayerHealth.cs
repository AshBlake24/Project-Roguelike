using System;
using System.Collections;
using Roguelike.Data;
using Roguelike.Infrastructure.Services.PersistentData;
using Roguelike.Logic;
using Roguelike.Player.Enhancements;
using Roguelike.Utilities;
using UnityEngine;

namespace Roguelike.Player
{
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerHealth : MonoBehaviour, IProgressWriter, IHealth, IEnhanceable<int>
    {
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private PlayerDeath _playerDeath;
        
        private State _state;
        private float _immuneTimeAfterHit;
        private float _immuneTimeAfterResurrect;

        public event Action HealthChanged;

        public bool IsImmune { get; private set; }

        public int CurrentHealth
        {
            get => _state.CurrentHealth;
            private set
            {
                if (_state.CurrentHealth != value)
                {
                    _state.CurrentHealth = value;
                    HealthChanged?.Invoke();
                }
            }
        }

        public int MaxHealth
        {
            get => _state.MaxHealth;
            private set
            {
                if (_state.MaxHealth != value)
                {
                    _state.MaxHealth = value;
                    HealthChanged?.Invoke();
                }
            }
        }
        
#if UNITY_EDITOR
        private void OnGUI()
        {
            if (GUI.Button(new Rect(30, 250, 100, 35), "Take Damage"))
                TakeDamage(1);
            
            if (GUI.Button(new Rect(30, 350, 100, 35), "Heal"))
                Heal(100);
        }
#endif

        private void OnEnable() => 
            _playerDeath.Resurrected += OnResurrected;

        private void OnDisable() => 
            _playerDeath.Resurrected -= OnResurrected;

        public void Construct(float immuneTimeAfterHit, float immuneTimeAfterResurrect)
        {
            _immuneTimeAfterHit = immuneTimeAfterHit;
            _immuneTimeAfterResurrect = immuneTimeAfterResurrect;
            IsImmune = false;
        }

        public void ReadProgress(PlayerProgress progress)
        {
            _state = progress.State;
            HealthChanged?.Invoke();
        }

        public void WriteProgress(PlayerProgress progress)
        {
            progress.State.CurrentHealth = CurrentHealth;
            progress.State.MaxHealth = MaxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (IsImmune || CurrentHealth <= 0)
                return;

            if (IsPositive(damage))
            {
                _playerAnimator.PlayHit();
                CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
                StartCoroutine(ImmuneTimer(_immuneTimeAfterHit));
            }
        }

        public void Heal(int health)
        {
            if (IsPositive(health))
                CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);
        }

        public void Enhance(int value)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be lower than 0");

            MaxHealth += value;
            CurrentHealth += value;
        }

        public void SetImmune(bool isImmune) => 
            IsImmune = isImmune;

        private IEnumerator ImmuneTimer(float time)
        {
            SetImmune(true);

            yield return Helpers.GetTime(time);

            SetImmune(false);
        }

        private static bool IsPositive(int value)
        {
            if (value >= 0)
                return true;

            throw new ArgumentOutOfRangeException(nameof(value), "Value must not be less than 0");
        }

        private void OnResurrected()
        {
            _state.Resurrect();
            StartCoroutine(ImmuneTimer(_immuneTimeAfterResurrect));
            HealthChanged?.Invoke();
        }
    }
}