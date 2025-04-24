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
        _damage.From = gameObject;
    }

    private void OnEnable()
    {
        _hp = _maxHp;
        _isDead = false;
        _rigidbody.isKinematic = true;
    }

    public void TakeDamage(DamageInfo damage)
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
        Collider[] hits = Physics.OverlapSphere(transform.position, _explodeRange);
        foreach(var hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out var damageable))
            {
                _rigidbody.isKinematic = false;
                damageable.TakeDamage(_damage);
                StartCoroutine(CoExplode());
            }
        }
    }

    private IEnumerator CoExplode()
    {
        GameObject effect = CommonPoolManager.Instance.GetObject(EObjectType.GranadeExplodeEffect, transform.position);

        Vector3 randomDir = (Vector3.one + Random.insideUnitSphere * 0.5f).normalized;
        _rigidbody.AddForce(randomDir * _force, ForceMode.Impulse);
        _rigidbody.AddTorque(Random.onUnitSphere * _force * 0.5f, ForceMode.Impulse);

        yield return new WaitForSeconds(4f);

        Destroy(gameObject);
    }
}
