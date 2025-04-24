using UnityEngine;

public class Granade : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    [SerializeField] private float _explodeRange;
    [SerializeField] private DamageInfo Damage;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        ResetRigidBody();
    }

    private void ResetRigidBody()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.rotation = Quaternion.identity;
        _rigidbody.Sleep();
        _rigidbody.WakeUp();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        PoolManager.Instance.GetObject(EObjectType.GranadeExplodeEffect, transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explodeRange);
        foreach(var hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(Damage);
            }
        }

        PoolManager.Instance.ReturnObject(gameObject, EObjectType.Granade);
    }
}