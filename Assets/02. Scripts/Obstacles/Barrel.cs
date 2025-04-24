using System.Collections;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamageable
{
    [SerializeField] private DamageInfo _damage;
    [SerializeField] private float _explodeRange;
    [SerializeField] private float _maxHp;
    [SerializeField] private float _force;
    private float _hp;

    [Header("# Components")]
    private Rigidbody _rigidbody;

    private bool _isDead = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _hp = _maxHp;
        _isDead = false;
    }

    void IDamageable.TakeDamage(DamageInfo damage)
    {
        if(_isDead)
        {
            return;
        }

        _hp -= damage.Value;

        if(_hp <= 0)
        {
            _isDead = true;
            Explode();
        }
    }

    private void Explode()
    {
        GameObject effect = PoolManager.Instance.GetObject(EObjectType.GranadeExplodeEffect, transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explodeRange);
        foreach(var hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(_damage);
                StartCoroutine(CoExplode());
            }
        }
    }

    private IEnumerator CoExplode()
    {
        _rigidbody.AddForce(Random.onUnitSphere * _force, ForceMode.Impulse);
        _rigidbody.AddTorque(Random.onUnitSphere * _force, ForceMode.Impulse);

        yield return new WaitForSeconds(2f);

        PoolManager.Instance.ReturnObject(gameObject, EObjectType.Barrel);
    }
}
