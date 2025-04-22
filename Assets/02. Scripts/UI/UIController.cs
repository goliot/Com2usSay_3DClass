using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    public PlayerMove PlayerMove;

    private void Awake()
    {
        PlayerMove.OnStaminaChange += UpdateStaminaSlider;
    }

    public void UpdateStaminaSlider(float currentPlayerStamina)
    {
        _slider.value = currentPlayerStamina / PlayerMove.PlayerStat.MaxStamina;
    }
}