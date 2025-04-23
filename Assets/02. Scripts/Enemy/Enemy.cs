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
    public float YVelocity { get; private set; }

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        StartPosition = transform.position;
        Stat = EnemyStats.GetData(_type);
        Stat.Damage.From = gameObject;
    }

    private void OnEnable()
    {
        _currentState = IdleState;
        _health = Stat.MaxHealth;
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    
    #region FSM
    private IEnemyState _currentState;

    [Header("# State Classes")]
    public IdleState IdleState { get; private set; } = new IdleState();
    public TraceState TraceState { get; private set; } = new TraceState();
    public AttackState AttackState { get; private set; } = new AttackState();
    public ReturnState ReturnState { get; private set; } = new ReturnState();
    public DamagedState DamagedState { get; private set; } = new DamagedState();
    public DieState DieState { get; private set; } = new DieState();

    public void ChangeState(IEnemyState newState)
    {
        Debug.Log($"{_currentState} -> {newState}");
        _currentState = newState;
        _currentState.Enter(this);
    }

    private void Update()
    {
        ApplyGravity();
        _currentState?.Execute(this);
    }

    private void ApplyGravity()
    {
        YVelocity += GRAVITY * Time.deltaTime;
    }

    public void TakeDamage(DamageInfo damage)
    {
        if (_currentState == DieState)
        {
            return;
        }

        Debug.Log($"적 피격 : {damage.Value}");
        _health -= damage.Value;

        if (_health <= 0)
        {
            Debug.Log("상태 전환: Damaged -> Die");
            ChangeState(DieState);
        }
        else
        {
            ChangeState(DamagedState);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}