using UnityEngine;

public interface IWeaponStrategy
{
    void Fire(PlayerFire playerFire);
    void Reload(PlayerFire playerFire);
    void Update(PlayerFire playerFire);
    void SetWeaponData(WeaponData weaponData);
    WeaponData GetWeaponData();
} 