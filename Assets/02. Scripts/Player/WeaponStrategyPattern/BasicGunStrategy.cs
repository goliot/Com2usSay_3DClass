using UnityEngine;
using System.Collections;

public class BasicGunStrategy : IWeaponStrategy
{
    private float _timer;
    private Coroutine _coReload;
    private WeaponData _weaponData;

    public void SetWeaponData(WeaponData weaponData)
    {
        _weaponData = weaponData;
    }

    public WeaponData GetWeaponData()
    {
        return _weaponData;
    }

    public void Fire(PlayerFire playerFire)
    {
        if (_coReload != null)
        {
            PlayerFire.OnReload?.Invoke(0, _weaponData.ReloadInterval);
            playerFire.StopCoroutine(_coReload);
            _coReload = null;
        }
        if (_weaponData.CoolTime <= _timer && playerFire.CurrentAmmo > 0)
        {
            if (_coReload != null)
            {
                playerFire.StopCoroutine(_coReload);
                _coReload = null;
            }
            playerFire.MuzzleFlash.SetActive(true);

            GameObject tracerBullet = CommonPoolManager.Instance.GetObject(EObjectType.TracerBullet, 
                playerFire.FirePosition.transform.position, Quaternion.identity);

            Ray ray = new Ray(playerFire.MainCamera.transform.position, playerFire.MainCamera.transform.forward);
            RaycastHit hitInfo;

            Vector3 hitPoint;
            bool isHit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Player")));
            if (isHit)
            {
                Debug.Log(hitInfo.collider.name);
                hitPoint = hitInfo.point;
                if (hitInfo.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(_weaponData.Damage);
                }
            }
            else
            {
                hitPoint = ray.origin + ray.direction * 50f;
            }

            playerFire.StartCoroutine(CoSpawnTrail(tracerBullet.GetComponent<TrailRenderer>(), hitPoint, isHit ? hitInfo.normal : null));

            playerFire.CurrentAmmo--;
            _timer = 0f;
            playerFire.gameObject.GetComponentInChildren<Animator>().SetTrigger("Shot");
        }
    }

    private IEnumerator CoSpawnTrail(TrailRenderer trail, Vector3 targetPoint, Vector3? normal)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, targetPoint, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = targetPoint;

        if (normal.HasValue)
        {
            CommonPoolManager.Instance.GetObject(EObjectType.BulletImpactEffect, targetPoint, 
                Quaternion.LookRotation(normal.Value));
        }

        yield return new WaitForSeconds(trail.time);
        CommonPoolManager.Instance.ReturnObject(trail.gameObject, EObjectType.TracerBullet);
    }

    public void Reload(PlayerFire playerFire)
    {
        if (_coReload == null)
        {
            _coReload = playerFire.StartCoroutine(CoReload(playerFire));
        }
    }

    private IEnumerator CoReload(PlayerFire playerFire)
    {
        int maxAmmo = _weaponData.MaxAmmo;
        float totalDuration = _weaponData.ReloadInterval;
        float timer = 0f;

        while(timer < totalDuration)
        {
            timer += Time.deltaTime;
            PlayerFire.OnReload?.Invoke(timer, totalDuration);
            yield return null;
        }
        playerFire.CurrentAmmo = maxAmmo;

        _coReload = null;
    }

    public void Update(PlayerFire playerFire)
    {
        _timer += Time.deltaTime;
    }
} 