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

    [Header("# WeaponObejct")]
    public List<WeaponObejct> _weaponPrefabs;
    private Dictionary<EWeaponType, GameObject> _weaponObjects = new Dictionary<EWeaponType, GameObject>();
    private EWeaponType _currentWeaponType = EWeaponType.Rifle;

    [Header("# Effects")]
    [SerializeField] private Transform _firePosition;
    public Transform FirePosition => _firePosition;
    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private GameObject _muzzleFlash;
    public GameObject MuzzleFlash => _muzzleFlash;

    [Header("# Camera")]
    public Camera MainCamera { get; private set; }

    [Header("# Events")]
    public static Action<int, int> OnAmmoChanged;
    public static Action<float, float> OnReload;
    public static Action<float, float> OnGranadeCharge;
    public static Action OnMeleeAttack;
    public static Action<EWeaponType> OnWeaponChange;

    [Header("# Zoom Aim")]
    public GameObject UI_Zoom;
    [SerializeField] private float _zoomInSize = 15f;
    [SerializeField] private float _zoomOutSize = 40f;
    private bool _isZoomMode = false;

    [Header("# Components")]
    public Animator Animator { get; private set; }

    [Header("# Current Ammo Infos")]
    public int CurrentAmmo
    {
        get => _weaponDatas.GetWeapon(_currentWeaponType).CurrentAmmo;
        set => _weaponDatas.GetWeapon(_currentWeaponType).CurrentAmmo = value;
    }

    private IWeaponStrategy _currentStrategy;
    private Dictionary<EWeaponType, IWeaponStrategy> _strategies;

    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        MainCamera = Camera.main;
        _weaponDatas.Init(gameObject);

        // 무기 오브젝트 초기화
        foreach (var weaponPrefab in _weaponPrefabs)
        {
            if (weaponPrefab.Prefab != null)
            {
                _weaponObjects[weaponPrefab.Type] = weaponPrefab.Prefab;
            }
        }

        _strategies = new Dictionary<EWeaponType, IWeaponStrategy>
        {
            { EWeaponType.Rifle, new RifleStrategy() },
            { EWeaponType.Granade, new GranadeStrategy() },
            { EWeaponType.Melee, new KatanaStrategy() },
        };

        foreach (var strategy in _strategies)
        {
            var weaponData = _weaponDatas.GetWeapon(strategy.Key);
            Debug.Log($"무기 데이터 설정: {strategy.Key}, 데미지: {weaponData.Damage.Value}");
            strategy.Value.SetWeaponData(weaponData);
        }

        _currentStrategy = _strategies[EWeaponType.Rifle];
        _currentWeaponType = EWeaponType.Rifle;

        this.ObserveEveryValueChanged(_ => CurrentAmmo)
            .DistinctUntilChanged()
            .Subscribe(newAmmo =>
            {
                OnAmmoChanged?.Invoke(newAmmo, _weaponDatas.GetWeapon(_currentWeaponType).MaxAmmo);
            }).AddTo(this);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        ChangeWeapon(EWeaponType.Rifle);
        CurrentAmmo = _weaponDatas.GetWeapon(_currentWeaponType).MaxAmmo;
    }

    private void Update()
    {
        _currentStrategy.Update(this);

        GetWeaponChangeInput();
        GetFireInput();
    }

    private void GetWeaponChangeInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int weaponCount = (int)EWeaponType.Count;
            int currentIndex = (int)_currentWeaponType;

            if (scroll > 0)
                currentIndex = (currentIndex + 1) % weaponCount;
            else if (scroll < 0)
                currentIndex = (currentIndex - 1 + weaponCount) % weaponCount;

            ChangeWeapon((EWeaponType)currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(EWeaponType.Rifle);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(EWeaponType.Melee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(EWeaponType.Granade);
        }

        // 줌 모드 자동 해제
        if (_currentStrategy is not RifleStrategy && _isZoomMode)
        {
            _isZoomMode = false;
            UI_Zoom.SetActive(false);
            MainCamera.fieldOfView = _isZoomMode ? _zoomInSize : _zoomOutSize;
        }

        // 탄약 UI 업데이트
        WeaponData data = _weaponDatas.GetWeapon(_currentWeaponType);
        OnAmmoChanged?.Invoke(data.CurrentAmmo, data.MaxAmmo);
        CurrentAmmo = data.CurrentAmmo;
        Animator.SetFloat("WeaponType", (float)(int)_currentWeaponType / ((int)EWeaponType.Count - 1));
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
        else if(Input.GetMouseButtonDown(1) && _currentStrategy is RifleStrategy)
        {
            _isZoomMode = !_isZoomMode;
            UI_Zoom.SetActive(_isZoomMode);
            MainCamera.fieldOfView = _isZoomMode ? _zoomInSize : _zoomOutSize;
        }
    }

    public void ChangeWeapon(EWeaponType weaponType)
    {
        if (_strategies.TryGetValue(weaponType, out var strategy))
        {
            // 현재 무기 비활성화, 새 무기 활성화
            if (_weaponObjects.TryGetValue(_currentWeaponType, out var currentWeapon) && _weaponObjects.TryGetValue(weaponType, out var newWeapon))
            {
                currentWeapon.SetActive(false);
                newWeapon.SetActive(true);
            }

            _currentStrategy = strategy;
            _currentWeaponType = weaponType;

            OnWeaponChange?.Invoke(_currentWeaponType);
            Debug.Log($"무기 변경: {weaponType}");
        }
    }

    private void OnDrawGizmos()
    {
        if (_firePosition == null || MainCamera == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(MainCamera.transform.position, MainCamera.transform.forward * 10000);
    }
}