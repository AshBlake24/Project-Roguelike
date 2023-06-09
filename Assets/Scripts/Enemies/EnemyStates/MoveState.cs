using Roguelike.Roguelike.Enemies.Animators;
using UnityEngine.AI;

namespace Roguelike.Enemies.EnemyStates
{
    public class MoveState : EnemyState
    {
        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            _agent.SetDestination(enemy.Target.transform.position);
        }

        public override void Enter(Enemy curentEnemy, EnemyAnimator enemyAnimator)
        {
            base.Enter(curentEnemy, enemyAnimator);

            _agent.SetDestination(enemy.Target.transform.position);
            _agent.speed = enemy.Speed;
            _agent.isStopped = false;

            animator.Move(_agent.speed, _agent.isStopped);
        }

        public override void Exit(EnemyState nextState)
        {
            _agent.isStopped = true;

            animator.Move(0, _agent.isStopped);

            base.Exit(nextState);
        }
    }
}
