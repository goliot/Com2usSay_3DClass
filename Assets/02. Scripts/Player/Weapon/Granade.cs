using UnityEngine;

public class Granade : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

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
        GameObject bombEffect = PoolManager.Instance.GetObject(EObjectType.GranadeExplodeEffect);
        bombEffect.transform.position = transform.position;
        PoolManager.Instance.ReturnObject(gameObject, EObjectType.Granade);
    }
}