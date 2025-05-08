using UnityEngine;

public class GranadeStrategy : IWeaponStrategy
{
    private bool _isCharging;
    private float _chargePower;
    private float _minChargePower = 10f;
    private float _maxChargePower = 50f;
    private float _chargeDuration = 1f;
    private float _chargeSpeed;
    private EWeaponType _type;
    PlayerFire _playerFire;

    public GranadeStrategy(EWeaponType type)
    {
        _type = type;
        _chargeSpeed = (_maxChargePower - _minChargePower) / _chargeDuration;
        ThrowGranadeEvent.OnThrowAction += ThrowGranade;
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
        if (playerFire.CurrentAmmo <= 0) return;
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
        playerFire.CurrentAmmo = WeaponManager.Instance.GetWeaponData(_type).MaxAmmo;
    }

    public void Update(PlayerFire playerFire)
    {
        if (_isCharging && Input.GetMouseButtonUp(0))
        {
            //ThrowGranade(playerFire);
            _playerFire = playerFire;
            playerFire.Animator.SetTrigger("GranadeShot");
        }
    }

    public void ThrowGranade()
    {
        GameObject granade = CommonPoolManager.Instance.GetObject(EObjectType.Granade, _playerFire.FirePosition.position);
        Granade granadeComponent = granade.GetComponent<Granade>();
        granadeComponent.SetDamage(WeaponManager.Instance.GetWeaponData(_type).Damage, WeaponManager.Instance.GetWeaponData(_type).ExplodeRange);

        Rigidbody granadeRigidbody = granadeComponent.Rigidbody;

        float actualPower = Mathf.Lerp(_minChargePower, _maxChargePower, _chargePower / _maxChargePower);
        granadeRigidbody.AddForce(_playerFire.MainCamera.transform.forward * actualPower, ForceMode.Impulse);
        granadeRigidbody.AddTorque(Vector3.one * 10f);

        PlayerFire.OnGranadeCharge?.Invoke(0, _maxChargePower);
        _playerFire.CurrentAmmo--;
        _isCharging = false;
    }
}