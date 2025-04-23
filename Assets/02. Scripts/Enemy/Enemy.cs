using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatsSO EnemyStats;
    private const float GRAVITY = -9.8f;

    [Header("# Stats")]
    [SerializeField] private EEnemyType _type;
    private float _health = 100f;
    public EnemyStat Stat { get; private set; }
    public float DyingTime { get; private set; } = 2f;


    [Header("# Components")]
    public CharacterController CharacterController { get; private set; }

    public GameObject Player { get; private set; }
    public Vector3 StartPosition { get; private set; }
    public float YVelocity { get; set; }


    [Header("# StateMachine")]
    public EnemyStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        StateMachine = new EnemyStateMachine(this, _idleState);
        CharacterController = GetComponent<CharacterController>();
        StartPosition = transform.position;
        Stat = EnemyStats.GetData(_type);
        Stat.Damage.From = gameObject;
    }

    private void OnEnable()
    {
        _health = Stat.MaxHealth;
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    #region States
    protected virtual IEnemyState _idleState => new IdleState();
    protected virtual IEnemyState _traceState => new TraceState();
    protected virtual IEnemyState _attackState => new AttackState();
    protected virtual IEnemyState _returnState => new ReturnState();
    protected virtual IEnemyState _damagedState => new DamagedState();
    protected virtual IEnemyState _dieState => new DieState();
    protected virtual IEnemyState _patrolState => new PatrolState();

    public virtual IEnemyState IdleState => _idleState;
    public virtual IEnemyState TraceState => _traceState;
    public virtual IEnemyState AttackState => _attackState;
    public virtual IEnemyState ReturnState => _returnState;
    public virtual IEnemyState DamagedState => _damagedState;
    public virtual IEnemyState DieState => _dieState;
    public virtual IEnemyState PatrolState => _patrolState;
    #endregion

    private void Update()
    {
        ApplyGravity();
        StateMachine.Update();
    }

    private void ApplyGravity()
    {
        YVelocity += GRAVITY * Time.deltaTime;
    }

    public void TakeDamage(DamageInfo damage)
    {
        if (StateMachine.CurrentState.GetType() == typeof(DieState))
        {
            return;
        }

        Debug.Log($"적 피격 : {damage.Value}");
        _health -= damage.Value;
        KnockBack(damage.KnockBackAmount, damage.From);

        if (_health <= 0)
        {
            Debug.Log("상태 전환: Damaged -> Die");
            StateMachine.ChangeState(DieState);
        }
        else
        {
            StateMachine.ChangeState(DamagedState);
        }
    }
    
    private void KnockBack(float amount, GameObject from)
    {
        // 넉백 방향 계산 (예시: 충돌한 적의 위치에서 현재 위치를 빼서 넉백 방향을 계산)
        Vector3 knockbackDirection = transform.position - from.transform.position;  // _lastHitPosition은 마지막으로 공격 받은 지점
        knockbackDirection.y = 0; // y축을 0으로 고정하여 수평으로만 밀림

        // 넉백 방향을 정규화
        knockbackDirection.Normalize();

        // 넉백 강도만큼 캐릭터를 밀어냄
        CharacterController.Move(knockbackDirection * amount);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}