using UnityEngine;

public class PlayerStaminaController : MonoBehaviour
{
    public float CurrentStamina { get; set; }
    private PlayerStatSO _playerStat;

    public PlayerStaminaController(PlayerStatSO playerStat)
    {
        _playerStat = playerStat;
        CurrentStamina = playerStat.MaxStamina;
    }

    public void UpdateStamina(EPlayerState currentState, float horizontalInput, float verticalInput)
    {
        if (currentState == EPlayerState.Sprinting)
        {
            CurrentStamina = Mathf.Max(0, CurrentStamina - _playerStat.SprintStanmina * Time.deltaTime);
        }
        else if (currentState == EPlayerState.Climbing && (horizontalInput != 0 || verticalInput != 0))
        {
            CurrentStamina = Mathf.Max(0, CurrentStamina - _playerStat.ClimbingStamina * Time.deltaTime);
        }
        else
        {
            CurrentStamina = Mathf.Min(_playerStat.MaxStamina, CurrentStamina + _playerStat.StaminaRegen * Time.deltaTime);
        }
    }
}
