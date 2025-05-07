using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyStatsSO EnemyStats;
    private const float GRAVITY = -9.8f;

    [Header("# Stats")]
    [SerializeField] private EEnemyType _type;
    private float _health = 100f;
    public EnemyStat Stat { get; private set; }
    public float DyingTime { get; private set; } = 2f;

    [Header("# Drop Item")]
    [SerializeField] private EnemyDropItemDataSO _dropItemData;

    [Header("# Components")]
    public NavMeshAgent NavAgent { get; private set; }
    public Animator Animator { get; private set; }
    public Collider Collider { get; private set; }

    public GameObject TargetPlayer { get; private set; }
    public Vector3 StartPosition { get; private set; }
    public float YVelocity { get; set; }

    [Header("# StateMachine")]
    public EnemyStateMachine StateMachine { get; protected set; }
    public EnemyStateDataSO TypeState;

    [Header("# Damage Flash")]
    private Renderer[] renderers;
    private Color originalColor;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    private Coroutine CoFlash;
    

    [Header(" Event")]
    public Action<float, float> OnHpChanged;

    private void Awake()
    {
        Collider = GetComponent<Collider>();
        NavAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
        StartPosition = transform.position;
        Stat = EnemyStats.GetData(_type);
        Stat.Damage.From = gameObject;
        NavAgent.speed = Stat.MoveSpeed;
        StateMachine = new EnemyStateMachine(this, TypeState.GetStateDictionary(_type));
        renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
            originalColor = renderers[0].material.color;
    }

    private void OnEnable()
    {
        StateMachine.ChangeState(EEnemyState.Idle);
        _health = Stat.MaxHealth;
        Collider.enabled = true;
        OnHpChanged?.Invoke(_health, Stat.MaxHealth);
    }

    private void Start()
    {
        TargetPlayer = Player.Instance.gameObject;
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

    public void MakeDamage()
    {
        if(_type == EEnemyType.Fat)
        {
            MakeRangeDamage();
        }
        else
        {
            TargetPlayer.GetComponent<IDamageable>().TakeDamage(Stat.Damage);
        }
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
        Flash();

        if (_health <= 0)
        {
            Debug.Log("상태 전환: Damaged -> Die");
            StateMachine.ChangeState(EEnemyState.Die);
        }
    }

    public void Die()
    {
        EnemyDropItemEntry data = _dropItemData.GetEntry(_type);
        for (int i = 0; i < data.Count; i++)
        {
            CommonPoolManager.Instance.GetObject(data.Type, transform.position);
        }
        if(_type == EEnemyType.Fat)
        {
            MakeRangeDamage();
        }

        EnemyPoolManager.Instance.ReturnObject(gameObject, _type);
    }

    private void MakeRangeDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, Stat.ExplodeRange, ~(1 << LayerMask.NameToLayer("Enemy")));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(Stat.Damage);
            }
        }
    }

    public void Flash()
    {
        if(CoFlash != null)
        {
            StopCoroutine(CoFlash);
            foreach (var rend in renderers)
                rend.material.color = originalColor;
            CoFlash = null;
        }
        CoFlash = StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        foreach (var rend in renderers)
            rend.material.color = hitColor;

        yield return new WaitForSeconds(flashDuration);

        foreach (var rend in renderers)
            rend.material.color = originalColor;
    }
}