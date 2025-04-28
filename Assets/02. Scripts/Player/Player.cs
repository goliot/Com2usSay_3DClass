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

    // Player.cs

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _hp = _stat.MaxHp;
        UpdateAnimatorInjuredLayer();
    }

    public void TakeDamage(DamageInfo damage)
    {
        _hp -= damage.Value;
        _hp = Mathf.Max(_hp, 0); // 체력 음수 방지

        OnDamaged?.Invoke();
        OnHpChanged?.Invoke(_hp, _stat.MaxHp);

        UpdateAnimatorInjuredLayer(); // 여기서 호출

        if (_hp <= 0)
        {
            Die();
        }
    }

    private void UpdateAnimatorInjuredLayer()
    {
        float injuredWeight = 1f - (_hp / _stat.MaxHp);
        _animator.SetLayerWeight(1, injuredWeight);
    }


    private void Die()
    {
        Destroy(gameObject);
    }
}