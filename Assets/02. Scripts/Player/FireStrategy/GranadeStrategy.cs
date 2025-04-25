using UnityEngine;

public class GranadeStrategy : IWeaponStrategy
{
    private bool _isCharging;
    private float _chargePower;
    private float _maxChargePower = 30f;
    private float _chargeSpeed = 10f;
    private WeaponData _weaponData;

    public void SetWeaponData(WeaponData weaponData)
    {
        _weaponData = weaponData;
        Debug.Log($"수류탄 전략 무기 데이터 설정: {_weaponData.Damage.Value}");
    }

    public WeaponData GetWeaponData()
    {
        return _weaponData;
    }

    public void Fire(PlayerFire playerFire)
    {
        if (!_isCharging)
        {
            StartCharging(playerFire);
        }
        else
        {
            Charging(playerFire);
        }
    }

    private void StartCharging(PlayerFire playerFire)
    {
        if (playerFire.CurrentGranade <= 0) return;
        _isCharging = true;
        _chargePower = 0f;
    }

    private void Charging(PlayerFire playerFire)
    {
        if (!_isCharging) return;

        _chargePower += _chargeSpeed * Time.deltaTime;
        _chargePower = Mathf.Min(_chargePower, _maxChargePower);
        PlayerFire.OnGranadeCharge?.Invoke(_chargePower, _maxChargePower);
    }

    public void Reload(PlayerFire playerFire)
    {
        // 수류탄은 재장전이 없음
    }

    public void Update(PlayerFire playerFire)
    {
        if (_isCharging && Input.GetMouseButtonUp(0))
        {
            ThrowGranade(playerFire);
        }
    }

    private void ThrowGranade(PlayerFire playerFire)
    {
        GameObject granade = CommonPoolManager.Instance.GetObject(playerFire.BombType, playerFire.FirePosition.position);
        Granade granadeComponent = granade.GetComponent<Granade>();
        granadeComponent.SetDamage(_weaponData.Damage);

        Rigidbody granadeRigidbody = granadeComponent.Rigidbody;
        granadeRigidbody.AddForce(playerFire.MainCamera.transform.forward * _chargePower, ForceMode.Impulse);
        granadeRigidbody.AddTorque(Vector3.one * 10f);

        PlayerFire.OnGranadeCharge?.Invoke(0, _maxChargePower);
        playerFire.CurrentGranade--;
        _isCharging = false;
    }
} 