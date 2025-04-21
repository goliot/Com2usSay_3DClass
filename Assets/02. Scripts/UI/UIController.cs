using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public PlayerMove PlayerMove;
    [SerializeField] private Slider _slider;

    private void Awake()
    {
        PlayerMove.OnStaminaChange += UpdateStaminaSlider;
    }

    public void UpdateStaminaSlider()
    {
        _slider.value = PlayerMove.Stamina / PlayerMove.PlayerStat.MaxStamina;
    }
}