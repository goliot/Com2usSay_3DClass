using System;
using System.Collections;
using UnityEngine;
using UniRx;

public class PlayerMove : MonoBehaviour
{
    private const float GRAVITY = -9.8f; //중력
    private float _yVelocity = 0f;       //중력 가속도

    [Header("# Components")]
    private Animator _animator;
    private CharacterController _characterController;
    [SerializeField] private PlayerMovementStatSO _playerStat;
    public PlayerMovementStatSO PlayerStat => _playerStat;

    [Header("# States")]
    private EPlayerState _currentState;
    private float _h;
    private float _v;
    private int _jumpCount = 0;
    private float _currentSpeed;
    private float _currentStamina;
    public float Stamina => _currentStamina;

    [Header("# Climbing")]
    private bool _isClimbingWall = false;

    public static Action<float, float> OnStaminaChange;
    public static Action<EPlayerState> OnMoveChange;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _characterController = GetComponent<CharacterController>();
        _currentState = EPlayerState.Idle;
        _currentSpeed = _playerStat.WalkSpeed;
        _currentStamina = _playerStat.MaxStamina;
    }

    private void Start()
    {
        this.ObserveEveryValueChanged(_ => Mathf.Round(_currentStamina * 100f) / 100f)
            .DistinctUntilChanged()
            .Subscribe(newStamina =>
            {
                OnStaminaChange?.Invoke(newStamina, _playerStat.MaxStamina);
            }).AddTo(this);
    }

    void Update()
    {
        GetInput();
        UpdateState();
        Move();
        UpdateStamina();
    }

    private void GetInput()
    {
        _h = Input.GetAxisRaw("Horizontal");
        _v = Input.GetAxisRaw("Vertical");
    }

    private bool CheckWallInFront()
    {
        Vector3 origin = transform.position + Vector3.down;
        Vector3 direction = transform.forward; // 정면 기준

        return Physics.Raycast(origin, direction, _playerStat.WallCheckDistance, _playerStat.WallLayer);
    }

    private void UpdateState()
    {
        // 벽타기 감지
        if (CheckWallInFront() && Input.GetAxisRaw("Vertical") != 0 && _currentStamina > 0)
        {
            _currentState = EPlayerState.Climbing;
            _currentSpeed = _playerStat.ClimbSpeed;
            _isClimbingWall = true;
            OnMoveChange?.Invoke(_currentState);
        }
        else if ((_isClimbingWall && !CheckWallInFront()) || _currentStamina <= 0)
        {
            _currentState = EPlayerState.Walking;
            _currentSpeed = _playerStat.WalkSpeed;
            _isClimbingWall = false;
            OnMoveChange?.Invoke(_currentState);
        }

        // 달리기
        if (Input.GetKeyDown(KeyCode.LeftShift) && _currentStamina > 0)
        {
            _currentState = EPlayerState.Sprinting;
            _currentSpeed = _playerStat.SprintSpeed;
            OnMoveChange?.Invoke(_currentState);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentState = EPlayerState.Walking;
            _currentSpeed = _playerStat.WalkSpeed;
            OnMoveChange?.Invoke(_currentState);
        }
        // 구르기
        else if (Input.GetKeyDown(KeyCode.E) && _currentStamina >= _playerStat.RollStamina)
        {
            _currentState = EPlayerState.Rolling;
            _currentSpeed = _playerStat.RollSpeed;
            _currentStamina -= _playerStat.RollStamina;
            StartCoroutine(CoRoll());
            OnMoveChange?.Invoke(_currentState);
        }
        // 점프
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_characterController.isGrounded || _jumpCount < 2)
            {
                _currentState = EPlayerState.Jumping;
                _yVelocity = _playerStat.JumpPower;
                _jumpCount++;
                OnMoveChange?.Invoke(EPlayerState.Jumping);
            }
        }
    }


    private void Move()
    {
        Vector3 direction;

        if (_currentState == EPlayerState.Climbing)
        {
            // 위/아래 방향 입력으로만 이동
            direction = new Vector3(_h, _v, 0);
            _characterController.Move(direction * _currentSpeed * Time.deltaTime);
            return;
        }

        direction = new Vector3(_h, 0, _v);
        _animator.SetFloat("MoveAmount", direction.magnitude);
        direction = Camera.main.transform.TransformDirection(Vector3.Normalize(direction));

        if (_currentState == EPlayerState.Climbing)
        {
            _yVelocity = 0f; // 벽 타는 중엔 중력 제거
        }
        else if (_characterController.isGrounded)
        {
            if (_yVelocity < 0)
            {
                _jumpCount = 0;
            }
            _yVelocity = 0f;
        }
        else
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }

        direction.y = _yVelocity;
        _characterController.Move(direction * _currentSpeed * Time.deltaTime);
    }

    private void UpdateStamina()
    {
        if (_currentState == EPlayerState.Sprinting)
        {
            _currentStamina = Mathf.Max(0, _currentStamina - _playerStat.SprintStanmina * Time.deltaTime);
        }
        else if(_currentState == EPlayerState.Climbing && (_h != 0 || _v != 0))
        {
            _currentStamina = Mathf.Max(0, _currentStamina - _playerStat.ClimbingStamina * Time.deltaTime);
        }
        else
        {
            _currentStamina = Mathf.Min(_playerStat.MaxStamina, _currentStamina + _playerStat.StaminaRegen * Time.deltaTime);
        }
    }

    private IEnumerator CoRoll()
    {
        float timer = 0f;
        while (timer < _playerStat.RollDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _currentState = EPlayerState.Idle;
        _currentSpeed = _playerStat.WalkSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.down, transform.forward * _playerStat.WallCheckDistance);
    }
}