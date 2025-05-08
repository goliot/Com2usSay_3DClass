using DG.Tweening;
using System;
using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    private Action _closeCallback;

    public void Open(EPopupType type, Action closeCallback = null)
    {
        _closeCallback = closeCallback;

        gameObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack).SetUpdate(true);
    }

    public void Close()
    {
        _closeCallback?.Invoke();
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false)).SetUpdate(true);
    }
}