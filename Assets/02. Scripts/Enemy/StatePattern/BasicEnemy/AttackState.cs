using UnityEngine;

[CreateAssetMenu(fileName = "AttackState", menuName = "Enemy/States/AttackState")]
public class AttackState : ScriptableObject, IEnemyState
{
    float _timer;

    public void Enter(Enemy enemy)
    {
        _timer = enemy.Stat.AttackCoolTime;
    }

    public void Execute(Enemy enemy)
    {
        _timer += Time.deltaTime;

        if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) >= enemy.Stat.AttackDistance)
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
        //if(enemy.Player.TryGetComponent<IDamageable>(out var damageable))
        //{
        //    damageable.TakeDamage(enemy.Stat.Damage);
        //}
        enemy.Animator.SetTrigger("IdleToAttack");
    }

    public void Exit(Enemy enemy)
    {
        enemy.Animator.SetTrigger("AttackToIdle");
    }
}