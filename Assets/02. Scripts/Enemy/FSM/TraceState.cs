using UnityEngine;

public class TraceState : IEnemyState
{

    void IEnemyState.Enter(Enemy enemy)
    {
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
            return;
        }
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) >= enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(enemy.ReturnState);
            return;
        }

        Vector3 direction = (enemy.Player.transform.position - enemy.transform.position).normalized;
        direction.y = enemy.YVelocity;
        enemy.CharacterController.Move(enemy.Stat.MoveSpeed * Time.deltaTime * direction);
    }
    void IEnemyState.Exit(Enemy enemy)
    {

    }
}