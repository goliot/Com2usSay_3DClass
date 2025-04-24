using System.Collections.Generic;

public class TraceEnemy : Enemy
{
    protected override void AwakeInit()
    {
        Dictionary<EEnemyState, IEnemyState> dict = new Dictionary<EEnemyState, IEnemyState>
        {
            { EEnemyState.Idle, new IdleState() },
            { EEnemyState.Trace, new TraceEnemyTraceState() },
            { EEnemyState.Attack, new AttackState() },
            { EEnemyState.Damaged, new DamagedState() },
            { EEnemyState.Die, new DieState() },
        };
        StateMachine = new EnemyStateMachine(this, dict);
    }
}