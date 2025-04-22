using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject bombEffect = PoolManager.Instance.GetObject(EObjectType.BombEffect);
        bombEffect.transform.position = transform.position;
        PoolManager.Instance.ReturnObject(gameObject, EObjectType.Bomb);
    }
}