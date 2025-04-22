using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private void Awake()
    {
        PlayerMove.OnStaminaChange += UpdateStaminaSlider;
    }

    public void UpdateStaminaSlider(float currentPlayerStamina, float maxStamina)
    {
        _slider.value = currentPlayerStamina / maxStamina;
    }
}