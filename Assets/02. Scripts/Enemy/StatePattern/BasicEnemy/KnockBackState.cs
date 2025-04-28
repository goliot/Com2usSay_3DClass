using UnityEngine;

[CreateAssetMenu(fileName = "KnockBackState", menuName = "Enemy/States/KnockBackState")]
public class KnockBackState : ScriptableObject, IEnemyState
{
    private float _knockBackTimer;
    private Vector3 _knockBackDirection;
    private float _knockBackForce;
    private float _knockBackDuration = 0.1f;

    public void Enter(Enemy enemy)
    {
        enemy.Animator.SetTrigger("Hit");
        _knockBackTimer = 0f;
        // NavAgent의 velocity를 직접 조작하여 넉백 효과 생성
        enemy.NavAgent.velocity = _knockBackDirection * _knockBackForce;
    }

    public void Execute(Enemy enemy)
    {
        _knockBackTimer += Time.deltaTime;

        // 시간이 지날수록 넉백 효과가 감소
        float progress = _knockBackTimer / _knockBackDuration;
        float currentForce = Mathf.Lerp(_knockBackForce, 0f, progress);
        enemy.NavAgent.velocity = _knockBackDirection * currentForce;

        if (_knockBackTimer >= _knockBackDuration)
        {
            enemy.NavAgent.velocity = Vector3.zero;
            enemy.StateMachine.ChangeState(EEnemyState.Damaged);
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.velocity = Vector3.zero;
    }

    public void SetKnockBackInfo(Vector3 direction, float force)
    {
        _knockBackDirection = direction;
        _knockBackForce = force;
    }
} 