using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const float GRAVITY = -9.8f; //중력
    private float _yVelocity = 0f;       //중력 가속도

    [SerializeField] private float _speed;
    [SerializeField] private float _stamina;
    public float Stamina => _stamina;

    [Header("# Move Stats")]
    [SerializeField] private PlayerStatSO _playerStat;
    public PlayerStatSO PlayerStat => _playerStat;

    [Header("# Climbing")]
    private bool _canClimb;

    [Header("# States")]
    private float _h;
    private float _v;
    private bool _canDoubleJump = true;
    private bool _isSprinting = false;
    private Coroutine _rollCoroutine = null;

    [Header("# Components")]
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _speed = _playerStat.WalkSpeed;
        _stamina = _playerStat.MaxStamina;
    }

    void Update()
    {
        GetInput();
        WallCheck();
        Move();
        Sprint();
        Jump();
        Roll();
        UpdateStamina();
    }

    //private void WallCheck()
    //{
    //    _canClimb = Physics.SphereCast(transform.position, _sphereCastRadius, transform.forward, out frontWallHit, _detectionLength, _wallLayer);
    //    _wallLookAngle = Vector3.Angle(transform.forward, -frontWallHit.normal);
    //}

    private void WallCheck()
    {
        _canClimb = _characterController.collisionFlags == CollisionFlags.Sides;
    }

    private void GetInput()
    {
        _h = Input.GetAxisRaw("Horizontal");
        _v = Input.GetAxisRaw("Vertical");
    }

    private void UpdateStamina()
    {
        if(_isSprinting || _canClimb)
        {
            _stamina = Mathf.Max(0, _stamina - _playerStat.SprintStanmina * Time.deltaTime);
        }
        else
        {
            _stamina = Mathf.Min(_playerStat.MaxStamina, _stamina + 5 * Time.deltaTime);
        }
    }

    private void Move()
    {
        Vector3 direction = new Vector3(_h, 0, _v).normalized;

        //메인 카메라 기준 방향 변환
        direction = Camera.main.transform.TransformDirection(direction);

        if (_canClimb)
        {
            _yVelocity = _playerStat.ClimbSpeed;
        }
        else
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }
        direction.y = _yVelocity;

        _characterController.Move(direction * _speed * Time.deltaTime);
    }

    private void Sprint()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && _stamina > 0)
        {
            _isSprinting = true;
            _speed = _playerStat.SprintSpeed;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isSprinting = false;
            _speed = _playerStat.WalkSpeed;
        }
    }

    private void Jump()
    {
        if (_characterController.isGrounded)
        {
            _canDoubleJump = true;
        }

        if (Input.GetButtonDown("Jump") && (_characterController.isGrounded || _canDoubleJump))
        {
            _yVelocity = _playerStat.JumpPower;

            // 땅이 아니었다면 더블 점프 소모
            if (!_characterController.isGrounded)
            {
                _canDoubleJump = false;
            }
        }
    }

    private void Roll()
    {
        if (!_characterController.isGrounded)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && _rollCoroutine == null && _stamina >= _playerStat.RollStamina)
        {
            _stamina -= _playerStat.RollStamina;
            _speed = _playerStat.RollSpeed;
            _rollCoroutine = StartCoroutine(CoRoll());
        }
    }
    
    IEnumerator CoRoll()
    {
        float timer = 0f;
        while(timer < _playerStat.RollDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _speed = _isSprinting ? _playerStat.SprintSpeed : _playerStat.WalkSpeed;
        _rollCoroutine = null;
    }
}
