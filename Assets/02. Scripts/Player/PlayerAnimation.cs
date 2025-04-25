using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        PlayerMove.OnMoveChange += ChangeAnimation;
        PlayerFire.OnMeleeAttack += MeleeAttack;
        _animator.speed = 3;
    }

    private void Update()
    {
        if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
        }
    }

    public void MeleeAttack()
    {
        _animator.SetTrigger("MeleeAttack");
    }

    public void ChangeAnimation(EPlayerState state)
    {
        switch(state)
        {
            case EPlayerState.Idle:
                break;
            case EPlayerState.Walking:
            case EPlayerState.Climbing:
                break;
            case EPlayerState.Sprinting:
                break;
            case EPlayerState.Rolling:
                break;
            case EPlayerState.Jumping:
                break;
        }
    }
}
