using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] public PlayerStatSO _stat;
    public PlayerStatSO Stat => _stat;

    private float _hp;
    private Animator _animator;

    public static Action OnDamaged;
    public static Action<float, float> OnHpChanged;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _hp = _stat.MaxHp;
        _animator.SetLayerWeight(2, _hp / _stat.MaxHp);
    }

    public void TakeDamage(DamageInfo damage)
    {
        _hp -= damage.Value;
        OnDamaged?.Invoke();
        OnHpChanged?.Invoke(_hp, _stat.MaxHp);

        if(_hp <= 0)
        {
            Die();
        }

        _animator.SetLayerWeight(2, _hp / _stat.MaxHp);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}