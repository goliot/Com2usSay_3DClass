using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class UI_PopupManager : Singleton<UI_PopupManager>
{
    //private List<UI_Popup> _openedUIList = new List<UI_Popup>();
    private Stack<UI_Popup> _openedUIStack = new Stack<UI_Popup>();

    [Header("# UIs")]
    [SerializeField] private List<UI_Popup> _popups;

    private void Update()
    {
        if(_openedUIStack.Count > 0 && Input.GetKeyDown(KeyCode.Backspace))
        {
            CloseUI();
        }
    }

    public void OpenUI(EPopupType type, Action closeCallback = null)
    {
        string popupName = type.ToString();
        foreach (var popup in _popups)
        {
            if (popup.gameObject.name == popupName)
            {
                popup.Open(type, closeCallback);
                //_openedUIList.Add(popup);
                _openedUIStack.Push(popup);
                break;
            }
        }
    }

    public void CloseUI()
    {
        if (_openedUIStack.Count == 0)
            return;

        while (true)
        {
            var popup = _openedUIStack.Pop();
            bool isOpen = popup.isActiveAndEnabled;
            popup.Close();

            if(isOpen || _openedUIStack.Count == 0)
            {
                break;
            }
        }

        //while (_openedUIList.Count > 0)
        //{
        //    bool opened = _openedUIList[_openedUIList.Count - 1].isActiveAndEnabled;
        //    if (opened)
        //    {
        //        _openedUIList[_openedUIList.Count - 1].Close();
        //        _openedUIList.RemoveAt(_openedUIList.Count - 1);
        //        break;
        //    }
        //}
    }
}