using System;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    //CRUD : Create Read Update Delete
    //무기 레벨업 기능 생각하기
    [Header ("# WeaponData")]
    [SerializeField] private WeaponDataSO _weaponDatas;
    public WeaponDataSO WeaponDatas => _weaponDatas;

    private void Awake()
    {
        _weaponDatas.Init(Player.Instance.gameObject);
    }

    public WeaponData GetWeaponData(EWeaponType type)
    {
        return _weaponDatas.GetWeapon(type);
    }

    public bool TryShot(EWeaponType type)
    {
        if(GetWeaponData(type).CurrentAmmo > 0)
        {
            GetWeaponData(type).CurrentAmmo--;
            return true;
        }
        return false;
    }
}