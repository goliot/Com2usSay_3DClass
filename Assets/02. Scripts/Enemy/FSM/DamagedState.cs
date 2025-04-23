using UnityEngine;

public class DamagedState : IEnemyState
{
    private float _damagedTimer;

    void IEnemyState.Enter(Enemy enemy)
    {
        _damagedTimer = 0f;
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        _damagedTimer += Time.deltaTime;

        if(_damagedTimer >= enemy.Stat.DamagedTime)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Trace);
        }
    }
    void IEnemyState.Exit(Enemy enemy)
    {

    }
}