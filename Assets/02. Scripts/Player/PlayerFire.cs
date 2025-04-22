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
            Gun();
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

    private void Gun()
    {
        if (_weaponDatas.GetWeapon(EWeaponType.BasicGun).CoolTime < _timer && _currentAmmo > 0)
        {
            if(_coReload != null)
            {
                StopCoroutine(_coReload);
                OnReload?.Invoke(_weaponDatas.GetWeapon(EWeaponType.BasicGun).ReloadInterval, _weaponDatas.GetWeapon(EWeaponType.BasicGun).ReloadInterval);
                _coReload = null;
            }
            Ray ray = new Ray(_firePosition.position, _mainCamera.transform.forward);
            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                GameObject bulletImpact = PoolManager.Instance.GetObject(EObjectType.BulletImpactEffect);
                bulletImpact.transform.position = hitInfo.point;
                bulletImpact.transform.forward = hitInfo.normal; // 법선 벡터(수직 벡터)
            }

            _currentAmmo--;
            _timer = 0f;
        }
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
}