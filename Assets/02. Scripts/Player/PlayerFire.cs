using System;
using System.Collections;
using UnityEngine;
using UniRx;

public class PlayerFire : MonoBehaviour
{
    [Header("# WeaponData")]
    [SerializeField] private WeaponDataSO _weaponDatas;
    public WeaponDataSO WeaponDatas => _weaponDatas;

    [SerializeField] private Transform _firePosition;
    [SerializeField] private EObjectType _bombType;
    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private GameObject _muzzleFlash;

    private Camera _mainCamera;

    [Header("# Events")]
    public static Action<int, int> OnAmmoChanged;
    public static Action<int, int> OnGrandeNumberChanged;
    public static Action<float, float> OnReload;
    public static Action<float, float> OnGranadeCharge;

    [Header("# Current Ammo Infos")]
    private int _currentAmmo;
    private int _currentGranade;

    [Header("# Granade Charge")]
    private bool _isCharging;
    private float _chargePower;
    private float _maxChargePower = 30f;
    private float _chargeSpeed = 10f;

    private Coroutine _coReload;
    private float _timer;

    private void Awake()
    {
        _mainCamera = Camera.main;

        this.ObserveEveryValueChanged(_ => _currentAmmo)
            .DistinctUntilChanged()
            .Subscribe(newAmmo =>
            {
                OnAmmoChanged?.Invoke(newAmmo, _weaponDatas.GetWeapon(EWeaponType.BasicGun).MaxAmmo);
            }).AddTo(this);

        this.ObserveEveryValueChanged(_ => _currentGranade)
            .DistinctUntilChanged()
            .Subscribe(newGranade =>
            {
                OnGrandeNumberChanged?.Invoke(newGranade, _weaponDatas.GetWeapon(EWeaponType.Granade).MaxAmmo);
            }).AddTo(this);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _currentAmmo = _weaponDatas.GetWeapon(EWeaponType.BasicGun).MaxAmmo;
        _currentGranade = _weaponDatas.GetWeapon(EWeaponType.Granade).MaxAmmo;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetMouseButton(0))
        {
            FireGun();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            StartCharging();
        }
        else if (Input.GetMouseButton(1))
        {
            Charging();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ThrowGranade();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    private void FireGun()
    {
        if (_weaponDatas.GetWeapon(EWeaponType.BasicGun).CoolTime < _timer && _currentAmmo > 0)
        {
            if (_coReload != null)
            {
                StopCoroutine(_coReload);
                OnReload?.Invoke(_weaponDatas.GetWeapon(EWeaponType.BasicGun).ReloadInterval, _weaponDatas.GetWeapon(EWeaponType.BasicGun).ReloadInterval);
                _coReload = null;
            }
            _muzzleFlash.SetActive(true);

            GameObject tracerBullet = PoolManager.Instance.GetObject(EObjectType.TracerBullet);
            tracerBullet.SetActive(false);
            tracerBullet.transform.position = _firePosition.transform.position;
            tracerBullet.transform.rotation = Quaternion.identity;
            tracerBullet.SetActive(true);

            Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            RaycastHit hitInfo;

            Vector3 hitPoint;
            bool isHit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Player")));
            if (isHit)
            {
                hitPoint = hitInfo.point;
            }
            else
            {
                // 맞지 않았을 경우 일정 거리 앞으로
                hitPoint = ray.origin + ray.direction * 50f; // 50f는 트레일 길이
            }

            StartCoroutine(CoSpawnTrail(tracerBullet.GetComponent<TrailRenderer>(), hitPoint, isHit ? (Vector3?)hitInfo.normal : null));

            _currentAmmo--;
            _timer = 0f;
        }
    }

    private IEnumerator CoSpawnTrail(TrailRenderer trail, Vector3 targetPoint, Vector3? normal)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, targetPoint, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = targetPoint;

        if (normal.HasValue)
        {
            GameObject impact = PoolManager.Instance.GetObject(EObjectType.BulletImpactEffect);
            impact.transform.position = targetPoint;
            impact.transform.rotation = Quaternion.LookRotation(normal.Value);
        }

        yield return new WaitForSeconds(trail.time);
        PoolManager.Instance.ReturnObject(trail.gameObject, EObjectType.TracerBullet);
    }


    private void StartCharging()
    {
        if (_currentGranade <= 0) return;
        _isCharging = true;
        _chargePower = 0f;
    }

    private void Charging()
    {
        if (!_isCharging) return;

        _chargePower += _chargeSpeed * Time.deltaTime;
        _chargePower = Mathf.Min(_chargePower, _maxChargePower);
        OnGranadeCharge?.Invoke(_chargePower, _maxChargePower);
    }

    private void ThrowGranade()
    {
        if (!_isCharging) return;

        GameObject granade = PoolManager.Instance.GetObject(_bombType);
        granade.transform.position = _firePosition.position;

        Rigidbody granadeRigidbody = granade.GetComponent<Granade>().Rigidbody;
        granadeRigidbody.AddForce(_mainCamera.transform.forward * _chargePower, ForceMode.Impulse);
        granadeRigidbody.AddTorque(Vector3.one * 10f);

        OnGranadeCharge?.Invoke(0, _maxChargePower);
        _currentGranade--;
        _isCharging = false;
    }

    private void Reload()
    {
        if (_coReload == null)
        {
            _coReload = StartCoroutine(CoReload());
        }
    }

    private IEnumerator CoReload()
    {
        int maxAmmo = _weaponDatas.GetWeapon(EWeaponType.BasicGun).MaxAmmo;
        float totalDuration = _weaponDatas.GetWeapon(EWeaponType.BasicGun).ReloadInterval;
        float timer = 0f;

        while(timer < totalDuration)
        {
            timer += Time.deltaTime;
            OnReload?.Invoke(timer, totalDuration);
            yield return null;
        }
        _currentAmmo = maxAmmo;

        _coReload = null;
    }

    private void OnDrawGizmos()
    {
        if (_firePosition == null || _mainCamera == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_firePosition.position, _mainCamera.transform.forward * 100f);
    }
}