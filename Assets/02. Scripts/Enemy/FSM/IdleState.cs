using UnityEngine;

public class IdleState : IEnemyState
{
    void IEnemyState.Enter(Enemy enemy)
    {
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) < enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(enemy.TraceState);
            return;
        }
    }
    void IEnemyState.Exit(Enemy enemy)
    {

    }
}