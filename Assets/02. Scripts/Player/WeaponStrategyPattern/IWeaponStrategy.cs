public interface IWeaponStrategy
{
    public void Fire(PlayerFire playerFire); // 발사
    public void Reload(PlayerFire playerFire);
    public void Update(PlayerFire playerFire); // Update문에서 실행할 것
}