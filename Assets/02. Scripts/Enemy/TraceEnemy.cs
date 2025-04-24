public class TraceEnemy : Enemy
{
    //임시
    IEnemyState RangeAttackState = new AttackState();

    protected override void AwakeInit()
    {
        base.AwakeInit();

        StateMachine.ModifyState(EEnemyState.Attack, RangeAttackState);
    }
}