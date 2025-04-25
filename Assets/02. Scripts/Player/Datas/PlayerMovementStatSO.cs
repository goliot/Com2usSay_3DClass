using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementStatSO", menuName = "Scriptable Objects/PlayerMovementStatSO")]
public class PlayerMovementStatSO : ScriptableObject
{
    public float MaxStamina = 100f;
    public float StaminaRegen = 5f;

    [Header("# Move Stats")]
    public float WalkSpeed = 7f;
    public float SprintSpeed = 12f;
    public float SprintStanmina = 10f;
    public float JumpPower = 3f;
    public float RollSpeed = 50f;
    public float RollDuration = 0.2f;  // 대시 지속 시간
    public float RollStamina = 30f;

    [Header("# Climbing")]
    public LayerMask WallLayer;
    public float WallCheckDistance = 1f;
    public float ClimbSpeed = 5f;
    public float ClimbingStamina = 5f;
}