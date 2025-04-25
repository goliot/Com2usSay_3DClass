using UnityEngine;

public class AttackState : IEnemyState
{
    float _timer;

    public void Enter(Enemy enemy)
    {
        _timer = 0f;
    }

    public void Execute(Enemy enemy)
    {
        _timer += Time.deltaTime;

        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) >= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Trace);
            return;
        }

        if (_timer >= enemy.Stat.AttackCoolTime)
        {
            PerformAttack(enemy);
            _timer = 0f;
        }
    }

    private void PerformAttack(Enemy enemy)
    {
        Debug.Log("공격!");
        if(enemy.Player.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(enemy.Stat.Damage);
        }
    }

    public void Exit(Enemy enemy)
    {

    }
}