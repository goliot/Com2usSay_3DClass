public interface IWeaponStrategy
{
    void Fire(PlayerFire playerFire); // 발사
    void Reload(PlayerFire playerFire);
    void Update(PlayerFire playerFire); // Update문에서 실행할 것
    void SetWeaponData(WeaponData weaponData); //초기 데이터 설정
    WeaponData GetWeaponData();
} 