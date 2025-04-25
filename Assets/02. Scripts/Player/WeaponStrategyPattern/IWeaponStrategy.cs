public interface IWeaponStrategy
{
    public void Fire(PlayerFire playerFire); // 발사
    public void Reload(PlayerFire playerFire);
    public void Update(PlayerFire playerFire); // Update문에서 실행할 것
    public void SetWeaponData(WeaponData weaponData); //초기 데이터 설정
    public WeaponData GetWeaponData();
} 