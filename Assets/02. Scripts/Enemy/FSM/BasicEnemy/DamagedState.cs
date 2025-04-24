using UnityEngine;

public class DamagedState : IEnemyState
{
    private float _damagedTimer;

    public void Enter(Enemy enemy)
    {
        _damagedTimer = 0f;
    }

    public void Execute(Enemy enemy)
    {
        _damagedTimer += Time.deltaTime;

        if(_damagedTimer >= enemy.Stat.DamagedTime)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Trace);
            return;
        }
    }

    public void Exit(Enemy enemy)
    {

    }
}