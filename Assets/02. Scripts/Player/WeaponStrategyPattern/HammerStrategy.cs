using UnityEngine;

public class HammerStrategy : IWeaponStrategy
{
    private float _timer;
    private WeaponData _weaponData;

    private float _attackableAngle = 90f;

    public void Fire(PlayerFire playerFire)
    {
        if(_weaponData.CoolTime <= _timer)
        {
            PlayerFire.OnMeleeAttack?.Invoke();

            //원뿔 범위 가져오고
            //걔네들 TakeDamage시키기
            MeleeAttack(playerFire);

            _timer = 0f;
        }
    }

    private void MeleeAttack(PlayerFire playerFire)
    {
        Collider[] cols = Physics.OverlapSphere(playerFire.transform.position, _weaponData.ExplodeRange);

        foreach (var e in cols)
        {
            // 검출한 대상의 방향을 구한다.
            Vector3 direction = (e.transform.position - playerFire.transform.position).normalized;


            // 대상과의 각도가 설정한 각도 이내에 있는지 확인한다.
            // viewAngle 은 부채꼴 전체 각도이기 때문에, 0.5를 곱해준다.
            if (Vector3.Angle(playerFire.transform.forward, direction) < (_attackableAngle * 0.5f))
            {
                if (e.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(_weaponData.Damage);
                }
            }
        }
    }

    public WeaponData GetWeaponData()
    {
        return _weaponData;
    }

    public void Reload(PlayerFire playerFire)
    {
        //근접무기 재장전 없음
    }

    public void SetWeaponData(WeaponData weaponData)
    {
        _weaponData = weaponData;
    }

    public void Update(PlayerFire playerFire)
    {
        _timer += Time.deltaTime;
    }
}