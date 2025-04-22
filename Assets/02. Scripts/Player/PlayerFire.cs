using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [SerializeField] private Transform _firePosition;
    [SerializeField] private EObjectType _bombType;

    [SerializeField] private float _throwPower = 15f;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Gun();
        Bomb();
    }

    private void Gun()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //레이저를 생성하고 발사 위치와 진행 방향을 설정
            Ray ray = new Ray(_firePosition.position, _mainCamera.transform.forward);

            //레이저와 부딪힌 물체의 정보를 저장할 변수
            RaycastHit hitInfo = new RaycastHit();

            //레이저를 발사한 다음, 변수에 데이터가 있다면 피격 이펙트 생성
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                GameObject bulletImpact = PoolManager.Instance.GetObject(EObjectType.BulletImpact);
                bulletImpact.transform.position = hitInfo.point;
                bulletImpact.transform.forward = hitInfo.normal; // 법선 벡터(수직 벡터)
            }

            //Ray: 레이저
            //RayCast: 레이저 발사
            //RayCastHit : 레이저 충돌시 정보 저장 구조체
        }
    }

    private void Bomb()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject bomb = PoolManager.Instance.GetObject(_bombType);
            bomb.transform.position = _firePosition.position;

            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
            bombRigidbody.AddForce(_mainCamera.transform.forward * _throwPower, ForceMode.Impulse);
            bombRigidbody.AddTorque(Vector3.one);
        }
    }
}
