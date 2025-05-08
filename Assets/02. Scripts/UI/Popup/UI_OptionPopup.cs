using UnityEngine;
using DG.Tweening;

public class UI_OptionPopup : UI_Popup
{
    public void OnClickResumeButton()
    {
        UI_PopupManager.Instance.CloseUI();    
    }

    public void OnClickRestartButton()
    {
        GameManager.Instance.Restart();
        gameObject.SetActive(false);
    }

    public void OnClickQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickCreditButton()
    {
        UI_PopupManager.Instance.OpenUI(EPopupType.UICreditPopup);
    }
}