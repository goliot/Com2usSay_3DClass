public class RangeEnemy : Enemy
{
    protected override IEnemyState _attackState => new RangedAttackState();

    public override IEnemyState AttackState => _attackState;
}