using UnityEngine;

public class IdleState : IEnemyState
{
    private float _timer;

    void IEnemyState.Enter(Enemy enemy)
    {
        _timer = 0f;
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        _timer += Time.deltaTime;
        if(_timer >= enemy.Stat.IdleToPatrolTime)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Patrol);
        }

        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) < enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Trace);
            return;
        }
    }
    void IEnemyState.Exit(Enemy enemy)
    {

    }
}