using UnityEngine;

public class IdleState : IEnemyState
{
    private Vector3 _targetPosition;

    void IEnemyState.Enter(Enemy enemy)
    {
        _targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        throw new System.NotImplementedException();
    }

    void IEnemyState.Exit(Enemy enemy)
    {
        throw new System.NotImplementedException();
    }
}