using UnityEngine;

[CreateAssetMenu(fileName = "TraceEnemyTraceState", menuName = "Enemy/States/TraceEnemyTraceState")]
public class TraceEnemyTraceState : ScriptableObject, IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.NavAgent.isStopped = false;
        enemy.Animator.SetTrigger("IdleToMove");
    }

    public void Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.TargetPlayer.transform.position, enemy.transform.position) <= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Attack);
            return;
        }

        if (enemy.NavAgent.isOnNavMesh)
        {
            enemy.NavAgent.SetDestination(enemy.TargetPlayer.transform.position);
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.ResetPath();
    }
}