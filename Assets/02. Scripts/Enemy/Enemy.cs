using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyStatsSO EnemyStats;
    private const float GRAVITY = -9.8f;

    [Header("# Stats")]
    [SerializeField] private EEnemyType _type;
    private float _health = 100f;
    public EnemyStat Stat { get; private set; }
    public float DyingTime { get; private set; } = 2f;


    [Header("# Components")]
    public Rigidbody Rigidbody { get; private set; }
    public NavMeshAgent NavAgent { get; private set; }

    public GameObject Player { get; private set; }
    public Vector3 StartPosition { get; private set; }
    public float YVelocity { get; set; }


    [Header("# StateMachine")]
    public EnemyStateMachine StateMachine { get; protected set; }

    [Header(" Event")]
    public Action<float, float> OnHpChanged;

    private void Awake()
    {
        AwakeInitCommon();
        AwakeInit();
    }

    private void AwakeInitCommon()
    {
        //Rigidbody = GetComponent<Rigidbody>();
        NavAgent = GetComponent<NavMeshAgent>();
        StartPosition = transform.position;
        Stat = EnemyStats.GetData(_type);
        Stat.Damage.From = gameObject;
        NavAgent.speed = Stat.MoveSpeed;
    }

    /// <summary>
    /// 행동 정의
    /// </summary>
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
            { EEnemyState.KnockBack, new KnockBackState() },
        };
        StateMachine = new EnemyStateMachine(this, dict);
    }

    private void OnEnable()
    {
        StateMachine.ChangeState(EEnemyState.Idle);
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

    public void TakeDamage(DamageInfo damage)
    {
        if (StateMachine.CurrentState.GetType() == typeof(DieState))
        {
            return;
        }

        Debug.Log($"적 피격 : {damage.Value}");
        _health -= damage.Value;
        OnHpChanged?.Invoke(_health, Stat.MaxHealth);

        // 넉백 상태로 전환
        Vector3 knockbackDirection = transform.position - damage.From.transform.position;
        knockbackDirection.y = 0;
        knockbackDirection.Normalize();

        var knockBackState = StateMachine.GetState(EEnemyState.KnockBack) as KnockBackState;
        knockBackState.SetKnockBackInfo(knockbackDirection, damage.KnockBackAmount);
        StateMachine.ChangeState(EEnemyState.KnockBack);

        if (_health <= 0)
        {
            Debug.Log("상태 전환: Damaged -> Die");
            StateMachine.ChangeState(EEnemyState.Die);
        }
    }

    public void Die()
    {
        EnemyPoolManager.Instance.ReturnObject(gameObject, _type);
    }
}