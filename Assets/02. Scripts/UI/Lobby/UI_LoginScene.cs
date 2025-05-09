using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public struct AccountInputFields
{
    public TMP_InputField IDInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField PasswordCheck;
    public Button ConfirmButton;

    public List<TMP_InputField> Fields { get; private set; }

    public void InitFields()
    {
        Fields = new List<TMP_InputField>(){
            IDInputField,
            PasswordInputField
        };
        if(PasswordCheck != null)
        {
            Fields.Add(PasswordCheck);
        }
    }
}

public class UI_LoginScene : MonoBehaviour
{
    [Header ("# Panel")]
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _registerPanel;
    [SerializeField] private TextMeshProUGUI _resultText;

    [Header("# Login")]
    [SerializeField] private AccountInputFields _loginFields;

    [Header("# Register")]
    [SerializeField] private AccountInputFields _registerFields;

    private AccountInputFields _nowPanel;
    private Vector3 _resultOriginPos;

    private void Start()
    {
        _loginFields.InitFields();
        _registerFields.InitFields();
        _loginPanel.SetActive(true);
        _registerPanel.SetActive(false);
        _nowPanel = _loginFields;
        _resultOriginPos = _resultText.rectTransform.position;
        SetResultText("");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            for(int i=0; i<_nowPanel.Fields.Count; i++)
            {
                if (_nowPanel.Fields[i].isFocused)
                {
                    if (_nowPanel.Fields[i].isFocused)
                    {
                        int nextIndex = (i + 1) % _nowPanel.Fields.Count;
                        EventSystem.current.SetSelectedGameObject(_nowPanel.Fields[nextIndex].gameObject);
                        break;
                    }
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            _nowPanel.ConfirmButton.onClick?.Invoke();
            if(_nowPanel.ConfirmButton.TryGetComponent<UI_TouchBounce>(out var bounce)) 
            {
                bounce.Bounce();
            }
        }
    }

    public void OnClickGoToLoginButton()
    {
        _loginPanel.SetActive(true);
        _registerPanel.SetActive(false);
        _nowPanel = _loginFields;
    }

    public void OnClickGoToRegisterButton()
    {
        _loginPanel.SetActive(false);
        _registerPanel.SetActive(true);
        _nowPanel = _registerFields;
    }

    public void Register()
    {
        string id = _registerFields.IDInputField.text;
        string pw = _registerFields.PasswordInputField.text;
        string pwCheck = _registerFields.PasswordCheck.text;

        if (string.IsNullOrEmpty(id))
        {
            SetResultText("아이디를 입력해주세요.");
            return;
        }
        else if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(pwCheck))
        {   
            SetResultText("비밀번호 또는 확인을 입력해주세요.");
            return;
        }
        else if(pw != pwCheck)
        {
            SetResultText("입력하신 비밀번호와 확인이 일치하지 않습니다.");
            return;
        }
        else if (PlayerPrefs.HasKey(id))
        {
            SetResultText("이미 존재하는 아이디입니다.");
            return;
        }

        PlayerPrefs.SetString(id, AesEncryption.Encrypt(pw));
        SetResultText("회원가입이 완료되었습니다.");
        OnClickGoToLoginButton();
        _loginFields.IDInputField.text = id;
        _loginFields.PasswordInputField.text = "";
    }

    public void Login()
    {
        string id = _loginFields.IDInputField.text;
        string pw = _loginFields.PasswordInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            SetResultText("아이디를 입력해주세요.");
            return;
        }
        else if (string.IsNullOrEmpty(pw))
        {
            SetResultText("비밀번호를 입력해주세요.");
            return;
        }
        else if(!PlayerPrefs.HasKey(id) || AesEncryption.Decrypt(PlayerPrefs.GetString(id, "")) != pw)
        {
            SetResultText("아이디 또는 비밀번호가 일치하지 않거나 존재하지 않는 계정입니다.");
            return;
        }

        //TODO : 로그인 성공, 씬 로드
        SetResultText("로그인 성공!");
        Debug.Log(PlayerPrefs.GetString(id));
    }

    private void SetResultText(string s)
    {
        _resultText.text = s;
        _resultText.DOKill();
        _resultText.rectTransform.position = _resultOriginPos;
        _resultText.rectTransform.DOShakeAnchorPos(0.3f, 30, 100).OnComplete(() => _resultText.rectTransform.position = _resultOriginPos);
    }
}