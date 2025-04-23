using UnityEngine;

public class ReturnState : IEnemyState
{
    void IEnemyState.Enter(Enemy enemy)
    {
    }

    void IEnemyState.Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.StartPosition) <= enemy.Stat.ReturnDistance)
        {
            enemy.transform.position = enemy.StartPosition;
            enemy.StateMachine.ChangeState(enemy.IdleState);
            return;
        }
        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) <= enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(enemy.TraceState);
            return;
        }

        Vector3 direction = (enemy.StartPosition - enemy.transform.position).normalized;
        direction.y = enemy.YVelocity;
        enemy.CharacterController.Move(direction * enemy.Stat.MoveSpeed * Time.deltaTime);
    }
    void IEnemyState.Exit(Enemy enemy)
    {

    }
}