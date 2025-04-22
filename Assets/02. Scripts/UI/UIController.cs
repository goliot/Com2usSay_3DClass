using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("# Stamina")]
    [SerializeField] private Slider _staminaSlider;

    [Header("# Ammo Info")]
    [SerializeField] private TextMeshProUGUI _ammoInfoText;
    [SerializeField] private TextMeshProUGUI _granadeInfoText;
    [SerializeField] private Image _reloadProcessImage;

    [Header("# Granade Charge")]
    [SerializeField] private Slider _granadeChargeSlider;

    private void Awake()
    {
        PlayerMove.OnStaminaChange += UpdateStaminaSlider;
        PlayerFire.OnAmmoChanged += UpdateAmmoInfo;
        PlayerFire.OnGrandeNumberChanged += UpdateGranadeInfo;
        PlayerFire.OnReload += UpdateReloadProcess;
        PlayerFire.OnGranadeCharge += UpdateChargeSlider;
    }

    private void UpdateStaminaSlider(float currentPlayerStamina, float maxStamina)
    {
        _staminaSlider.value = currentPlayerStamina / maxStamina;
    }

    private void UpdateAmmoInfo(int value, int maxValue)
    {
        _ammoInfoText.text = $"총알 : {value} / {maxValue}";
    }

    private void UpdateGranadeInfo(int value, int maxValue)
    {
        _granadeInfoText.text = $"수류탄: {value} / {maxValue}";
    }

    private void UpdateReloadProcess(float curTime, float maxTime)
    {
        if (!_reloadProcessImage.gameObject.activeSelf)
        {
            _reloadProcessImage.gameObject.SetActive(true);
        }

        _reloadProcessImage.fillAmount = curTime / maxTime;

        if(_reloadProcessImage.fillAmount >= 1)
        {
            _reloadProcessImage.gameObject.SetActive(false);
        }
    }

    private void UpdateChargeSlider(float curPower , float maxPower)
    {
        _granadeChargeSlider.value = curPower / maxPower;
    }
}