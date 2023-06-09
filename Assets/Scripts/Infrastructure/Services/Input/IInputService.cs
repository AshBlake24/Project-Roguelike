using System;
using UnityEngine;

namespace Roguelike.Infrastructure.Services.Input
{
    public interface IInputService : IService
    {
        Vector2 Axis { get; }
        
        event Action<bool> WeaponChanged;
        event Action SkillUsed;
        event Action Interacted;
        event Action PausePressed;
        
        bool IsAttackButtonUp();
        void ChangeWeapon(bool switchToNext);
        void UseSkill();
        void Interact();
    }
}