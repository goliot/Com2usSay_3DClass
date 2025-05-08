using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("# GameState")]
    private EGameState _gameState = EGameState.Ready;
    public EGameState GameState => _gameState;

    [Header("# GameControl")]
    [SerializeField] private float _gameStartTime;

    [Header("# UIs")]
    [SerializeField] private TextMeshProUGUI _gameStartTimerText;
    [SerializeField] private GameObject _crossHair;
    [SerializeField] private GameObject _uiOptionPopup;

    private void Start()
    {
        StartCoroutine(CoGameStartRoutine());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    IEnumerator CoGameStartRoutine()
    {
        Time.timeScale = 0f;
        int displayTime = (int)_gameStartTime;

        while (displayTime >= 0)
        {
            _gameStartTimerText.text = $"Ready...{displayTime}";

            // 1초 동안 대기
            float wait = 0f;
            while (wait < 1f)
            {
                wait += Time.unscaledDeltaTime;
                yield return null;
            }

            displayTime--;
        }

        _gameStartTimerText.text = "Go!";
        yield return new WaitForSecondsRealtime(1f);

        _gameStartTimerText.gameObject.SetActive(false);
        _crossHair.SetActive(true);
        Time.timeScale = 1f;
    }

    private void Pause()
    {
        _gameState = EGameState.Pause;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        UI_PopupManager.Instance.OpenUI(EPopupType.UIOptionPopup, closeCallback : Resume);
    }

    public void Resume()
    {
        _gameState = EGameState.Run;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart()
    {
        _gameState = EGameState.Ready;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}