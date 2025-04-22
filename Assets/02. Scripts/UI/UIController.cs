using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("# Stamina")]
    [SerializeField] private Slider _slider;

    [Header("# Ammo Info")]
    [SerializeField] private TextMeshProUGUI _ammoInfoText;
    [SerializeField] private TextMeshProUGUI _granadeInfoText;
    [SerializeField] private Image _reloadProcessImage;

    private void Awake()
    {
        PlayerMove.OnStaminaChange += UpdateStaminaSlider;
        PlayerFire.OnAmmoChanged += UpdateAmmoInfo;
        PlayerFire.OnGrandeNumberChanged += UpdateGranadeInfo;
        PlayerFire.OnReload += UpdateReloadProcess;
    }

    private void UpdateStaminaSlider(float currentPlayerStamina, float maxStamina)
    {
        _slider.value = currentPlayerStamina / maxStamina;
    }

    private void UpdateAmmoInfo(int value, int maxValue)
    {
        _ammoInfoText.text = $"{value} / {maxValue}";
    }

    private void UpdateGranadeInfo(int value, int maxValue)
    {
        _granadeInfoText.text = $"{value} / {maxValue}";
    }

    private void UpdateReloadProcess(float curTime, float maxTime)
    {
        _reloadProcessImage.fillAmount = curTime / maxTime;
    }
}