using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyStatsSO EnemyStats;
    private const float GRAVITY = -9.8f;

    [Header("# Stats")]
    [SerializeField] private EObjectType _type;
    private float _health = 100f;
    public EnemyStat Stat { get; private set; }
    public float DyingTime { get; private set; } = 2f;


    [Header("# Components")]
    public CharacterController CharacterController { get; private set; }
    public NavMeshAgent NavAgent { get; private set; }

    public GameObject Player { get; private set; }
    public Vector3 StartPosition { get; private set; }
    public float YVelocity { get; set; }


    [Header("# StateMachine")]
    public EnemyStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        AwakeInitCommon();
        AwakeInit();
    }

    private void AwakeInitCommon()
    {
        CharacterController = GetComponent<CharacterController>();
        NavAgent = GetComponent<NavMeshAgent>();
        StartPosition = transform.position;
        Stat = EnemyStats.GetData(_type);
        Stat.Damage.From = gameObject;
        NavAgent.speed = Stat.MoveSpeed;
    }

    protected virtual void AwakeInit()
    {
        Dictionary<EEnemyState, IEnemyState> dict = new Dictionary<EEnemyState, IEnemyState>
        {
            { EEnemyState.Idle, new IdleState() },
            { EEnemyState.Trace, new TraceState() },
            { EEnemyState.Return, new ReturnState() },
            { EEnemyState.Attack, new AttackState() },
            { EEnemyState.Damaged, new DamagedState() },
            { EEnemyState.Die, new DieState() },
            { EEnemyState.Patrol, new PatrolState() },
        };
        StateMachine = new EnemyStateMachine(this, dict);
    }

    private void OnEnable()
    {
        _health = Stat.MaxHealth;
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        ApplyGravity();
        StateMachine.Update();
    }

    private void ApplyGravity()
    {
        YVelocity += GRAVITY * Time.deltaTime;
    }

    void IDamageable.TakeDamage(DamageInfo damage)
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
            StateMachine.ChangeState(EEnemyState.Die);
        }
        else
        {
            StateMachine.ChangeState(EEnemyState.Damaged);
        }
    }
    
    private void KnockBack(float amount, GameObject from)
    {
        Vector3 knockbackDirection = transform.position - from.transform.position;  // _lastHitPosition은 마지막으로 공격 받은 지점
        knockbackDirection.y = 0; 

        knockbackDirection.Normalize();

        CharacterController.Move(knockbackDirection * amount);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}