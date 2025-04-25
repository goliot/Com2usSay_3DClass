using System;
using System.Collections;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class PlayerFire : MonoBehaviour
{
    [Header("# WeaponData")]
    [SerializeField] private WeaponDataSO _weaponDatas;
    public WeaponDataSO WeaponDatas => _weaponDatas;

    [SerializeField] private Transform _firePosition;
    public Transform FirePosition => _firePosition;

    [SerializeField] private EObjectType _bombType;
    public EObjectType BombType => _bombType;

    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private GameObject _muzzleFlash;
    public GameObject MuzzleFlash => _muzzleFlash;

    private Camera _mainCamera;
    public Camera MainCamera => _mainCamera;

    [Header("# Events")]
    public static Action<int, int> OnAmmoChanged;
    public static Action<int, int> OnGrandeNumberChanged;
    public static Action<float, float> OnReload;
    public static Action<float, float> OnGranadeCharge;

    [Header("# Current Ammo Infos")]
    private int _currentAmmo;
    public int CurrentAmmo
    {
        get => _currentAmmo;
        set => _currentAmmo = value;
    }

    private int _currentGranade;
    public int CurrentGranade
    {
        get => _currentGranade;
        set => _currentGranade = value;
    }

    public Coroutine CoReload { get; set; }

    private IWeaponStrategy _currentStrategy;
    private Dictionary<EWeaponType, IWeaponStrategy> _strategies;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _weaponDatas.Init(gameObject);

        _strategies = new Dictionary<EWeaponType, IWeaponStrategy>
        {
            { EWeaponType.BasicGun, new BasicGunStrategy() },
            { EWeaponType.Granade, new GranadeStrategy() }
        };

        foreach (var strategy in _strategies)
        {
            var weaponData = _weaponDatas.GetWeapon(strategy.Key);
            Debug.Log($"무기 데이터 설정: {strategy.Key}, 데미지: {weaponData.Damage.Value}");
            strategy.Value.SetWeaponData(weaponData);
        }

        _currentStrategy = _strategies[EWeaponType.BasicGun];

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
        _currentStrategy.Update(this);

        GetWeaponChangeInput();
        GetFireInput();
    }

    private void GetWeaponChangeInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(EWeaponType.BasicGun);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //보조 무기
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //근접 무기
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeWeapon(EWeaponType.Granade);
        }
    }

    private void GetFireInput()
    {
        if (Input.GetMouseButton(0))
        {
            _currentStrategy.Fire(this);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            _currentStrategy.Reload(this);
        }
    }

    public void ChangeWeapon(EWeaponType weaponType)
    {
        if (_strategies.TryGetValue(weaponType, out var strategy))
        {
            _currentStrategy = strategy;
            Debug.Log(weaponType);
        }
    }

    private void OnDrawGizmos()
    {
        if (_firePosition == null || _mainCamera == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_firePosition.position, _mainCamera.transform.forward * 100f);
    }
}