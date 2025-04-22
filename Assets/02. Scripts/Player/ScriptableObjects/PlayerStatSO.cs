using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Scriptable Objects/PlayerStat")]
public class PlayerStatSO : ScriptableObject
{
    public float MaxStamina;
    public float StaminaRegen;

    [Header("# Move Stats")]
    public float WalkSpeed;
    public float SprintSpeed;
    public float SprintStanmina;
    public float JumpPower = 3f;
    public float RollSpeed;
    public float RollDuration = 0.2f;  // 대시 지속 시간
    public float RollStamina = 30f;
    public float ClimbSpeed;
}
