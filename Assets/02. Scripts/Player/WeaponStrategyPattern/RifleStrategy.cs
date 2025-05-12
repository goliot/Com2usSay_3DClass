using UnityEngine;
using System.Collections;

public class RifleStrategy : IWeaponStrategy
{
    private float _timer;
    private Coroutine _coReload;
    private EWeaponType _type;

    public RifleStrategy(EWeaponType type)
    {
        _type = type;
    }

    public void Fire(PlayerFire playerFire)
    {
        if (_coReload != null)
        {
            PlayerFire.OnReload?.Invoke(0, WeaponManager.Instance.GetWeaponData(_type).ReloadInterval);
            playerFire.StopCoroutine(_coReload);
            _coReload = null;
        }
        if (WeaponManager.Instance.GetWeaponData(_type).CoolTime <= _timer && WeaponManager.Instance.TryShot(WeaponManager.Instance.GetWeaponData(_type).WeaponType))
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
            bool isHit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("MapEffect"))));
            bool isEnemy = false;
            if (isHit)
            {
                isEnemy = hitInfo.collider.CompareTag("Enemy");
                Debug.Log(hitInfo.collider.name);
                hitPoint = hitInfo.point;
                if (hitInfo.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(WeaponManager.Instance.GetWeaponData(_type).Damage);
                }
            }
            else
            {
                hitPoint = ray.origin + ray.direction * 50f;
            }

            playerFire.StartCoroutine(CoSpawnTrail(tracerBullet.GetComponent<TrailRenderer>(), hitPoint, isHit ? hitInfo.normal : null, isEnemy));

            _timer = 0f;
            playerFire.gameObject.GetComponentInChildren<Animator>().SetTrigger("Shot");
        }
    }

    private IEnumerator CoSpawnTrail(TrailRenderer trail, Vector3 targetPoint, Vector3? normal, bool isEnemy)
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
            CommonPoolManager.Instance.GetObject(isEnemy ? EObjectType.ZombieHitBlood : EObjectType.BulletImpactEffect, targetPoint, 
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
        int maxAmmo = WeaponManager.Instance.GetWeaponData(_type).MaxAmmo;
        float totalDuration = WeaponManager.Instance.GetWeaponData(_type).ReloadInterval;
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