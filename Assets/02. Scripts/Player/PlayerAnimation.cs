using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        PlayerMove.OnMoveChange += ChangeAnimation;
    }

    private void Update()
    {
        
    }

    private void ChangeAnimation(EPlayerState state)
    {
        
    }
}
