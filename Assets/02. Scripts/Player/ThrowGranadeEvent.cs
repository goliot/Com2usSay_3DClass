using UnityEngine;
using System;

public class ThrowGranadeEvent : MonoBehaviour
{
    public static Action OnThrowAction;

    public void ThrowGranade()
    {
        OnThrowAction?.Invoke();
    }
}
