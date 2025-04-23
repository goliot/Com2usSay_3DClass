using UnityEngine;

public class DieState : IEnemyState
{
    float _dieTimer;

    void IEnemyState.Enter(Enemy enemy)
    {
        _dieTimer = 0f;
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        _dieTimer += Time.deltaTime;
        if(_dieTimer >= enemy.DyingTime)
        {
            enemy.Die();
        }
    }
}