using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerStatSO _playerStat;
    public PlayerStatSO PlayerStat => _playerStat;

    [Header("# States")]
    private EPlayerState _currentState;
    private int _jumpCount = 0;
    private float _currentSpeed;

    [Header("# Climbing")]
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallCheckDistance = 0.6f;

    private PlayerInputHandler _inputHandler;
    private PlayerMovementController _movementController;
    private PlayerStaminaController _staminaController;
    private PlayerClimbingController _climbingController;

    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputHandler = new PlayerInputHandler();
        _movementController = new PlayerMovementController(_characterController);
        _staminaController = new PlayerStaminaController(_playerStat);
        _climbingController = new PlayerClimbingController(_wallLayer, _wallCheckDistance);
    }

    void Update()
    {
        // 입력 처리
        float horizontalInput = _inputHandler.Horizontal;
        float verticalInput = _inputHandler.Vertical;

        // 상태 업데이트
        UpdateState(horizontalInput, verticalInput);

        // 이동 및 중력 업데이트
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;
        _movementController.UpdateGravity(_characterController.isGrounded);
        _movementController.Move(direction, _currentSpeed);

        // 스태미나 업데이트
        _staminaController.UpdateStamina(_currentState, horizontalInput, verticalInput);
    }

    private void UpdateState(float horizontalInput, float verticalInput)
    {
        // 벽타기 감지
        if (_climbingController.CheckWallInFront(transform.position, transform.forward) && verticalInput != 0 && _staminaController.CurrentStamina > 0)
        {
            _currentState = EPlayerState.Climbing;
            _currentSpeed = _playerStat.ClimbSpeed;
        }
        else if (_currentState == EPlayerState.Climbing && !_climbingController.CheckWallInFront(transform.position, transform.forward) || _staminaController.CurrentStamina <= 0)
        {
            _currentState = EPlayerState.Idle;
            _currentSpeed = _playerStat.WalkSpeed;
        }

        // 달리기
        if (_inputHandler.IsSprintPressed && _staminaController.CurrentStamina > 0)
        {
            _currentState = EPlayerState.Sprinting;
            _currentSpeed = _playerStat.SprintSpeed;
        }
        else if (_inputHandler.IsSprintReleased)
        {
            _currentState = EPlayerState.Idle;
            _currentSpeed = _playerStat.WalkSpeed;
        }
        // 구르기
        else if (_inputHandler.IsRollPressed && _staminaController.CurrentStamina >= _playerStat.RollStamina)
        {
            _currentState = EPlayerState.Rolling;
            _currentSpeed = _playerStat.RollSpeed;
            _staminaController.CurrentStamina -= _playerStat.RollStamina;
            StartCoroutine(CoRoll());
        }
        // 점프
        else if (_inputHandler.IsJumpPressed)
        {
            if (_characterController.isGrounded || _jumpCount < 2)
            {
                _currentState = EPlayerState.Idle;
                _movementController.YVelocity = _playerStat.JumpPower;
                _jumpCount++;
            }
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
}