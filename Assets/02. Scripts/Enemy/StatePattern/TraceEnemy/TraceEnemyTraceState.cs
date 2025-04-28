using UnityEngine;

[CreateAssetMenu(fileName = "TraceEnemyTraceState", menuName = "Enemy/States/TraceEnemyTraceState")]
public class TraceEnemyTraceState : ScriptableObject, IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.NavAgent.isStopped = false;
    }

    public void Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Attack);
            return;
        }

        enemy.NavAgent.SetDestination(enemy.Player.transform.position);
    }

    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.ResetPath();
    }
}