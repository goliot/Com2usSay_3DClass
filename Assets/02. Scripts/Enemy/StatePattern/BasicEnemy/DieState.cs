using UnityEngine;

[CreateAssetMenu(fileName = "DieState", menuName = "Enemy/States/DieState")]
public class DieState : ScriptableObject, IEnemyState
{
    float _dieTimer;

    public void Enter(Enemy enemy)
    {
        enemy.Collider.enabled = false;
        _dieTimer = 0f;
        enemy.NavAgent.isStopped = true;
        enemy.Animator.SetTrigger("Die");
    }

    public void Execute(Enemy enemy)
    {
        _dieTimer += Time.deltaTime;
        if(_dieTimer >= enemy.DyingTime)
        {
            enemy.StateMachine.ChangeState(EEnemyState.Idle);
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.isStopped = false;
        enemy.NavAgent.ResetPath();
        Debug.Log("Nav 초기화됨");
        enemy.Die();
    }
}