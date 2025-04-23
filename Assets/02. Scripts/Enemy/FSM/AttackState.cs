using UnityEngine;

public class AttackState : IEnemyState
{
    float _timer;

    void IEnemyState.Enter(Enemy enemy)
    {
        _timer = 0f;
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        _timer += Time.deltaTime;

        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) >= enemy.Stat.AttackDistance)
        {
            enemy.ChangeState(enemy.TraceState);
            return;
        }

        if (_timer >= enemy.Stat.AttackCoolTime)
        {
            Debug.Log("공격!");
            _timer = 0f;
        }
    }
}