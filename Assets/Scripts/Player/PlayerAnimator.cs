using System;
using Roguelike.Logic.Animations;
using UnityEngine;

namespace Roguelike.Player
{
    public class PlayerAnimator : MonoBehaviour, IAnimationStateReader
    {
        private static readonly int s_isMoving = Animator.StringToHash("IsMoving");
        private static readonly int s_speed = Animator.StringToHash("Speed");
        private static readonly int s_die = Animator.StringToHash("Die");

        private readonly int _walkingStateHash = Animator.StringToHash("Move");
        private readonly int _deathStateHash = Animator.StringToHash("Death");
        private readonly int _idleStateHash = Animator.StringToHash("Idle");

        [SerializeField] private Animator _animator;

        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;

        public AnimatorState State { get; private set; }
        
        public void PlayDeath() => _animator.SetTrigger(s_die);
        public void StopMoving() => _animator.SetBool(s_isMoving, false);

        public void Move(float speed)
        {
            _animator.SetBool(s_isMoving, true);
            _animator.SetFloat(s_speed, speed);
        }

        public void EnteredState(int stateHash)
        {
            State = GetState(stateHash);
            StateEntered?.Invoke(State);
        }

        public void ExitedState(int stateHash)
        {
            AnimatorState state = GetState(stateHash);
            StateExited?.Invoke(state);
        }

        private AnimatorState GetState(int stateHash)
        {
            AnimatorState state;

            if (stateHash == _idleStateHash)
                state = AnimatorState.Idle;
            else if (stateHash == _walkingStateHash)
                state = AnimatorState.Walking;
            else if (stateHash == _deathStateHash)
                state = AnimatorState.Died;
            else
                state = AnimatorState.Unknown;
            
            return state;
        }
    }
}