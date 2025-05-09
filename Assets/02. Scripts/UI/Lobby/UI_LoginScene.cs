using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct AccountInputFields
{
    public TMP_InputField IDInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField PasswordCheck;
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

    private void Start()
    {
        _loginPanel.SetActive(true);
        _registerPanel.SetActive(false);
        SetResultText("");
    }

    public void OnClickGoToLoginButton()
    {
        _loginPanel.SetActive(true);
        _registerPanel.SetActive(false);
    }

    public void OnClickGoToRegisterButton()
    {
        _loginPanel.SetActive(false);
        _registerPanel.SetActive(true);
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
    }

    private void SetResultText(string s)
    {
        _resultText.text = s;
        _resultText.transform.DOShakePosition(0.3f);
    }
}