using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const float GRAVITY = -9.8f;
    /*private IEnemyState _currentState;

    public void ChangeState(IEnemyState newState)
    {
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }

    private void Update()
    {
        _currentState?.Execute(this);
    }*/
    public EEnemyState CurrentState = EEnemyState.Idle;

    [Header("# Stats")]
    [SerializeField] private EEnemyType _type;
    [SerializeField] private EnemyStatsSO _enemyStats;
    public EnemyStatsSO EnemyStats => _enemyStats;
    private EnemyStat _stat;

    [SerializeField] private float _moveSpeed = 3.3f;
    [SerializeField] private float _attackCoolTime = 1f;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _health = 100f;

    [Header("# Distances")]
    [SerializeField] private float _findDistance = 7f;
    [SerializeField] private float _attackDistance = 2.5f;
    [SerializeField] private float _returnDistance = 0.1f;
    [SerializeField] private float _damagedTime = 0.5f; // 경직 시간

    [Header("# Components")]
    private CharacterController _characterController;

    private GameObject _player;
    private Vector3 _startPosition;
    private float _atkTimer;
    private float _yVelocity;
    private float _damagedTimer;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _startPosition = transform.position;
        _stat = _enemyStats.GetData(_type);
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        _atkTimer += Time.deltaTime;
        ApplyGravity();
        switch (CurrentState)
        {
            case EEnemyState.Idle:
                Idle();
                break;
            case EEnemyState.Trace:
                Trace();
                break;
            case EEnemyState.Return:
                Return();
                break;
            case EEnemyState.Attack:
                Attack();
                break;
            case EEnemyState.Damaged:
                Damaged();
                break;
            case EEnemyState.Die:
                Die();
                break;
        }
    }

    public void TakeDamage(DamageInfo damage)
    {
        Debug.Log($"적 피격 : {damage.Value}");
        _health -= damage.Value;
        _damagedTimer = 0f;
        CurrentState = EEnemyState.Damaged;
    }

    private void ApplyGravity()
    {
        _yVelocity += GRAVITY * Time.deltaTime;
    }

    private void Idle()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < _findDistance)
        {
            Debug.Log("상태 전환 : Idle -> Trace");
            CurrentState = EEnemyState.Trace;
            return;
        }
    }

    private void Trace()
    {
        if(Vector3.Distance(_player.transform.position, transform.position) <= _attackDistance)
        {
            Debug.Log("상태 전환 : Trace -> Attack");
            CurrentState = EEnemyState.Attack;
            return;
        }
        if(Vector3.Distance(_player.transform.position, transform.position) >= _findDistance)
        {
            Debug.Log("상태 전환 : Trace -> Return");
            CurrentState = EEnemyState.Return;
            return;
        }
        if (Vector3.Distance(transform.position, _player.transform.position) <= _attackDistance)
        {
            Debug.Log("상태 전환: Trace -> Attack");
            CurrentState = EEnemyState.Attack;
            return;
        }

        Vector3 direction = (_player.transform.position - transform.position).normalized;
        direction.y = _yVelocity;
        _characterController.Move(direction * _moveSpeed * Time.deltaTime);
    }

    private void Return()
    {
        if(Vector3.Distance(transform.position, _startPosition) <= _returnDistance)
        {
            Debug.Log("상태 전환 : Return -> Idle");
            transform.position = _startPosition;
            CurrentState = EEnemyState.Idle;
            return;
        }
        if(Vector3.Distance(transform.position, _player.transform.position) <= _findDistance)
        {
            Debug.Log("상태 전환 : Return -> Trace");
            CurrentState = EEnemyState.Trace;
            return;
        }

        Vector3 direction = (_startPosition - transform.position).normalized;
        direction.y = _yVelocity;
        _characterController.Move(direction * _moveSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if(Vector3.Distance(transform.position, _player.transform.position) >= _attackDistance)
        {
            Debug.Log("상태 전환: Attack -> Trace");
            CurrentState = EEnemyState.Trace;
            return;
        }

        if (_atkTimer >= _attackCoolTime)
        {
            Debug.Log("공격!");
            _atkTimer = 0f;
        }
    }

    private void Damaged()
    {
        if (_health <= 0)
        {
            Debug.Log("상태 전환: Damaged -> Die");
            CurrentState = EEnemyState.Die;
        }

        _damagedTimer += Time.deltaTime;
        if(_damagedTimer >= _damagedTime)
        {
            _damagedTimer = 0f;
            Debug.Log("상태 전환: Damaged -> Trace");
            CurrentState = EEnemyState.Trace;
        }
    }

    private void Die()
    {

    }
}
