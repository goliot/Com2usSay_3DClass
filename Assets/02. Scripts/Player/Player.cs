using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] public PlayerStatSO _stat;
    public PlayerStatSO Stat => _stat;

    private float _hp;

    public static Action OnDamaged;

    private void Awake()
    {
        _hp = _stat.MaxHp;
    }

    public void TakeDamage(DamageInfo damage)
    {
        _hp -= damage.Value;
        OnDamaged?.Invoke();

        if(_hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}