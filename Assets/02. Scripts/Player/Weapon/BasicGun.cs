using System.Collections;
using UnityEngine;

public class BasicGun : MonoBehaviour
{
    [SerializeField] private bool _addBulletSpread = true;
    [SerializeField] private Vector3 _bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private ParticleSystem _shootingSystem;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private ParticleSystem ImpactParticleSystem;
    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private LayerMask _mask;

    public void Shoot()
    {
        _shootingSystem.Play();

        Vector3 direction = GetDirection();

        if (Physics.Raycast(_bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, _mask))
        {
            TrailRenderer trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);

            StartCoroutine(CoSpawnTrail(trail, hit));
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if(_addBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-_bulletSpreadVariance.x, _bulletSpreadVariance.x),
                Random.Range(-_bulletSpreadVariance.y, _bulletSpreadVariance.y),
                Random.Range(-_bulletSpreadVariance.z, _bulletSpreadVariance.z)
            );
        }

        return direction;
    }

    private IEnumerator CoSpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        PoolManager.Instance.GetObject(EObjectType.BulletImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time); //TODO: 풀링으로 바꿔
    }
}