using UnityEngine;

public class Coin : MonoBehaviour
{
    public float bounceForce = 5f;          // 튀어오르는 힘
    public float RotateSpeed = 180f;        // 초당 회전 속도
    public float MoveSpeed = 10f;           // 플레이어 쪽으로 끌리는 속도

    private Rigidbody rb;
    private Transform _targetPlayer;

    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90);
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(new Vector3(Random.value, Random.value, Random.value) * bounceForce, ForceMode.Impulse);

        _targetPlayer = null;
    }

    private void Update()
    {
        transform.Rotate(Vector3.right * RotateSpeed * Time.deltaTime, Space.Self);

        if(_targetPlayer != null)
        {
            Vector3 direction = (_targetPlayer.position - transform.position).normalized;
            rb.linearVelocity = direction * MoveSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _targetPlayer = other.transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //TODO : Effect
            CommonPoolManager.Instance.ReturnObject(gameObject, EObjectType.Coin);
        }
    }
}
