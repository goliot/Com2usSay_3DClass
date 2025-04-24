using UnityEngine;

public class TraceEnemyTraceState : IEnemyState
{
    void IEnemyState.Enter(Enemy enemy)
    {
        enemy.NavAgent.isStopped = false;
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Attack);
            return;
        }

        enemy.NavAgent.SetDestination(enemy.Player.transform.position);
    }

    void IEnemyState.Exit(Enemy enemy)
    {
        enemy.NavAgent.ResetPath();
    }
}