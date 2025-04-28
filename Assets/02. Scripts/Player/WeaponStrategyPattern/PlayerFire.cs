using System;
using System.Collections;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

[System.Serializable]
public class WeaponPrefab
{
    public EWeaponType weaponType;
    public GameObject prefab;
}

public class PlayerFire : MonoBehaviour
{
    [Header("# WeaponData")]
    [SerializeField] private WeaponDataSO _weaponDatas;
    public WeaponDataSO WeaponDatas => _weaponDatas;

    [Header("# WeaponObejct")]
    //[SerializeField] private List<WeaponPrefab> _weaponPrefabs = new List<WeaponPrefab>();
    //private Dictionary<EWeaponType, GameObject> _weaponObjects = new Dictionary<EWeaponType, GameObject>();
    private EWeaponType _currentWeaponType = EWeaponType.BasicGun;

    [Header("# Effects")]
    [SerializeField] private Transform _firePosition;
    public Transform FirePosition => _firePosition;
    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private GameObject _muzzleFlash;
    public GameObject MuzzleFlash => _muzzleFlash;

    [Header("# Camera")]
    private Camera _mainCamera;
    public Camera MainCamera => _mainCamera;

    [Header("# Events")]
    public static Action<int, int> OnAmmoChanged;
    public static Action<int, int> OnGrandeNumberChanged;
    public static Action<float, float> OnReload;
    public static Action<float, float> OnGranadeCharge;
    public static Action OnMeleeAttack;
    public static Action OnWeaponChange;

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

    private IWeaponStrategy _currentStrategy;
    private Dictionary<EWeaponType, IWeaponStrategy> _strategies;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _weaponDatas.Init(gameObject);

        //// 무기 오브젝트 초기화
        //foreach (var weaponPrefab in _weaponPrefabs)
        //{
        //    if (weaponPrefab.prefab != null)
        //    {
        //        _weaponObjects[weaponPrefab.weaponType] = weaponPrefab.prefab;
        //    }
        //}

        _strategies = new Dictionary<EWeaponType, IWeaponStrategy>
        {
            { EWeaponType.BasicGun, new BasicGunStrategy() },
            { EWeaponType.Granade, new GranadeStrategy() },
            { EWeaponType.Melee, new HammerStrategy() },
        };

        foreach (var strategy in _strategies)
        {
            var weaponData = _weaponDatas.GetWeapon(strategy.Key);
            Debug.Log($"무기 데이터 설정: {strategy.Key}, 데미지: {weaponData.Damage.Value}");
            strategy.Value.SetWeaponData(weaponData);
        }

        _currentStrategy = _strategies[EWeaponType.BasicGun];
        //_currentWeaponType = EWeaponType.BasicGun;
        ChangeWeapon(EWeaponType.BasicGun);

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
            ChangeWeapon(EWeaponType.Melee);
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
            //// 현재 무기 비활성화
            //if (_weaponObjects.TryGetValue(_currentWeaponType, out var currentWeapon))
            //{
            //    currentWeapon.SetActive(false);
            //}

            //// 새 무기 활성화
            //if (_weaponObjects.TryGetValue(weaponType, out var newWeapon))
            //{
            //    newWeapon.SetActive(true);
            //}

            _currentStrategy = strategy;
            //_currentWeaponType = weaponType;

            OnWeaponChange?.Invoke();
            Debug.Log($"무기 변경: {weaponType}");
        }
    }

    private void OnDrawGizmos()
    {
        if (_firePosition == null || _mainCamera == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(MainCamera.transform.position, MainCamera.transform.forward * 10000);
    }
}