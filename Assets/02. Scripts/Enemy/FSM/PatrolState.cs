using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private List<Vector3> _patrolPoints = new List<Vector3>(3);
    private Vector3 _nextPoint;

    private float _changePointTimer;

    void IEnemyState.Enter(Enemy enemy)
    {
        _changePointTimer = 0;

        if (_patrolPoints.Count != 3)
        {
            _patrolPoints.Clear();
            for (int i = 0; i < 3; i++)
            {
                Vector3 nextPoint = Vector3.one * Random.Range(-5, 5);
                nextPoint.y = 0;
                _patrolPoints.Add(enemy.transform.position + nextPoint);
            }
        }
        _nextPoint = _patrolPoints[0];
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        _changePointTimer += Time.deltaTime;

        if(Vector3.Distance(_nextPoint, enemy.transform.position) <= enemy.Stat.ReturnDistance || _changePointTimer >= 3f)
        {
            _nextPoint = _patrolPoints[Random.Range(0, _patrolPoints.Count)];
            _changePointTimer = 0;
        }
        if(Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) <= enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Trace);
        }
        Vector3 direction = (_nextPoint - enemy.transform.position).normalized;
        direction.y = enemy.YVelocity;
        enemy.CharacterController.Move(enemy.Stat.MoveSpeed * Time.deltaTime * direction);
    }

    void IEnemyState.Exit(Enemy enemy)
    {

    }
}