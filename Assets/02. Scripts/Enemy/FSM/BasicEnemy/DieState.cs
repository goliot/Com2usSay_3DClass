using UnityEngine;

public class DieState : IEnemyState
{
    float _dieTimer;

    void IEnemyState.Enter(Enemy enemy)
    {
        _dieTimer = 0f;
        enemy.NavAgent.isStopped = true;
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        _dieTimer += Time.deltaTime;
        if(_dieTimer >= enemy.DyingTime)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Idle);
        }
    }
    void IEnemyState.Exit(Enemy enemy)
    {
        enemy.NavAgent.isStopped = false;
        enemy.NavAgent.ResetPath();
        Debug.Log("Nav 초기화됨");
        enemy.Die();
    }
}