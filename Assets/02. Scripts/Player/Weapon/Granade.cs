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

    public void SetDamage(DamageInfo damage)
    {
        _damage = damage;
        _damage.From = gameObject;
        Debug.Log($"수류탄 데미지 설정: {_damage.Value}");
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
        Debug.Log("수류탄 폭발!");
        CommonPoolManager.Instance.GetObject(EObjectType.GranadeExplodeEffect, transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explodeRange);
        Debug.Log($"폭발 범위 내 충돌체 수: {hits.Length}");

        foreach(var hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out var damageable))
            {
                Debug.Log($"데미지 적용 대상: {hit.gameObject.name}, 데미지: {_damage.Value}");
                damageable.TakeDamage(_damage);
            }
        }

        CommonPoolManager.Instance.ReturnObject(gameObject, EObjectType.Granade);
    }
}