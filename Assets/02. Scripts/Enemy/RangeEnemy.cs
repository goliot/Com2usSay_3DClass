public class RangeEnemy : Enemy
{
    IEnemyState RangeAttackState = new AttackState();

    protected override void AwakeInit()
    {
        base.AwakeInit();

        StateMachine.ModifyState(EEnemyState.Attack, RangeAttackState);
    }
}