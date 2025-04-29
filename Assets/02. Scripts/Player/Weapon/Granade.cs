using UnityEngine;

public class Granade : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    [SerializeField] private float _explodeRange;
    private DamageInfo _damage;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetDamage(DamageInfo damage, float explodeRange)
    {
        _damage = damage;
        _explodeRange = explodeRange;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        CommonPoolManager.Instance.GetObject(EObjectType.GranadeExplodeEffect, transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explodeRange);

        foreach(var hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(_damage);
            }
        }

        CommonPoolManager.Instance.ReturnObject(gameObject, EObjectType.Granade);
    }
}