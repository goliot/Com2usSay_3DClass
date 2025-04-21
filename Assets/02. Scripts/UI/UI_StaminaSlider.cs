using UnityEngine;
using UnityEngine.UI;

public class UI_StaminaSlider : MonoBehaviour
{
    public PlayerMove PlayerMove;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    void Update()
    {
        _slider.value = PlayerMove.Stamina / PlayerMove.PlayerStat.MaxStamina;
    }
}