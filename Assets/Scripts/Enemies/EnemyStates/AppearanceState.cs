﻿using Cinemachine;
using Roguelike.Roguelike.Enemies.Animators;
using Roguelike.UI.Elements;
using System;
using UnityEngine;

namespace Roguelike.Enemies.EnemyStates
{
    public class AppearanceState : EnemyState
    {
        [SerializeField] CinemachineVirtualCamera _bossCamera;
        [SerializeField] ActorUI _bossUI;
        [SerializeField] Transform _cameraPoint;

        private CinemachineVirtualCamera _currentCamera;
        private Transform _previousCameraFollower;

        public override void Enter(Enemy curentEnemy, EnemyAnimator enemyAnimator)
        {
            base.Enter(curentEnemy, enemyAnimator);

            _currentCamera = Instantiate(_bossCamera);

            if (_currentCamera != null)
            {
                _currentCamera.Follow = _cameraPoint;
                _currentCamera.LookAt = _cameraPoint;
            }
            else
            {
                throw new ArgumentNullException(nameof(CinemachineVirtualCamera));
            }
        }

        public override void Exit(EnemyState nextState)
        {
            ReturnCamera();

            _bossUI.gameObject.SetActive(true);

            base.Exit(nextState);
        }

        public void SetUI(ActorUI bossUI)
        {
            _bossUI= bossUI;
        }

        private void ReturnCamera()
        {
            _currentCamera.gameObject.SetActive(false);
        }
    }
}