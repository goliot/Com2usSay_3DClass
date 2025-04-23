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
        if(_damagedTimer >= enemy.Stat.DamagedTime)
        {
            enemy.ChangeState(enemy.TraceState);
        }
    }
}