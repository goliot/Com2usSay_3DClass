using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "Enemy/States/Idle")]
public class IdleState : ScriptableObject, IEnemyState
{
    private float _timer;

    public void Enter(Enemy enemy)
    {
        _timer = 0f;
    }

    public void Execute(Enemy enemy)
    {
        _timer += Time.deltaTime;
        if(_timer >= enemy.Stat.IdleToPatrolTime)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Patrol);
        }

        if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) < enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Trace);
            return;
        }
    }

    public void Exit(Enemy enemy)
    {

    }
}