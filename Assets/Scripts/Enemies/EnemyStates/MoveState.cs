using UnityEngine;
using UnityEngine.AI;

namespace Roguelike.Enemies.EnemyStates
{
    public class MoveState : EnemyState
    {
        private NavMeshAgent _agent;
        private Enemy _enemy;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            if (_enemy == null)
                _enemy = GetComponent<EnemyStateMachine>().Enemy;
        }

        private void Update()
        {
            _agent.SetDestination(enemy.Target.transform.position);
        }

        public override void Enter(Enemy curentEnemy)
        {
            base.Enter(curentEnemy);

            _agent.SetDestination(enemy.Target.transform.position);
            _agent.speed = _enemy.Speed;
            _agent.isStopped = false;
        }

        public override void Exit(EnemyState nextState)
        {
            _agent.isStopped = true;

            base.Exit(nextState);
        }
    }
}
