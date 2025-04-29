using NUnit.Framework;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{
    [Header("# Hp")]
    [SerializeField] private Slider _playerHpSlider;

    [Header("# Stamina")]
    [SerializeField] private Slider _staminaSlider;

    [Header("# Ammo Info")]
    [SerializeField] private TextMeshProUGUI _ammoInfoText;
    [SerializeField] private Image _reloadProcessImage;

    [Header("# Damage Effect")]
    [SerializeField] private Image _damageEffect;
    private Coroutine _damageCoroutine;
    private Color _originColor = new Color(1, 1, 1, 0.8f);
    [SerializeField] private float _fadeDuration = 3f;

    [Header("# Granade Charge")]
    [SerializeField] private Slider _granadeChargeSlider;

    [Header("# Weapon Image")]
    [SerializeField] private Image _weaponImage;
    [SerializeField] private WeaponImageSO _weaponImageData;

    private void Awake()
    {
        PlayerMove.OnStaminaChange += UpdateStaminaSlider;
        PlayerFire.OnAmmoChanged += UpdateAmmoInfo;
        PlayerFire.OnReload += UpdateReloadProcess;
        PlayerFire.OnGranadeCharge += UpdateChargeSlider;
        PlayerFire.OnWeaponChange += UpdateWeaponImage;
        Player.OnDamaged += DamageEffect;
        Player.OnHpChanged += UpdateHpSlider;
    }

    private void UpdateStaminaSlider(float currentPlayerStamina, float maxStamina)
    {
        _staminaSlider.value = currentPlayerStamina / maxStamina;
    }

    private void UpdateAmmoInfo(int value, int maxValue)
    {
        _ammoInfoText.text = $"{value} / {maxValue}";
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

    private void UpdateHpSlider(float curHp, float maxHp)
    {
        _playerHpSlider.value = curHp / maxHp;
    }

    private void UpdateWeaponImage(EWeaponType type)
    {
        _weaponImage.sprite = _weaponImageData.GetWeaponImageData(type).UIImage;
    }

    private void DamageEffect()
    {
        _damageEffect.gameObject.SetActive(true);
        _damageEffect.color = _originColor;
        if (_damageCoroutine != null)
        {
            StopCoroutine(_damageCoroutine);
        }
        _damageCoroutine = StartCoroutine(CoDamageEffect());
    }

    private IEnumerator CoDamageEffect()
    {
        Color color = _damageEffect.color;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsed / _fadeDuration));
            _damageEffect.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // 보정
        _damageEffect.color = new Color(color.r, color.g, color.b, 0f);

        _damageEffect.gameObject.SetActive(false);
        _damageCoroutine = null;
    }
}