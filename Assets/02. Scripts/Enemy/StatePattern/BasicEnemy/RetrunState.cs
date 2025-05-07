using UnityEngine;

[CreateAssetMenu(fileName = "ReturnState", menuName = "Enemy/States/ReturnState")]
public class ReturnState : ScriptableObject, IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.Animator.SetTrigger("IdleToMove");
    }

    public void Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.StartPosition) <= enemy.Stat.ReturnDistance)
        {
            enemy.transform.position = enemy.StartPosition;
            enemy.StateMachine.ChangeState(EEnemyState.Idle);
            return;
        }
        if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) <= enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Trace);
            return;
        }

        enemy.NavAgent.SetDestination(enemy.StartPosition);
        //Vector3 direction = (enemy.StartPosition - enemy.transform.position).normalized;
        //direction.y = enemy.YVelocity;
        //enemy.CharacterController.Move(direction * enemy.Stat.MoveSpeed * Time.deltaTime);
    }
    void IEnemyState.Exit(Enemy enemy)
    {
        enemy.Animator.SetTrigger("MoveToIdle");
    }
}