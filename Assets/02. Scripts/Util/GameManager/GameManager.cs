using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] TextMeshProUGUI _gameStartTimerText;
    [SerializeField] GameObject _crossHair;
    [SerializeField] private float _gameStartTime;
    private float _timer;

    private void Start()
    {
        StartCoroutine(CoGameStartRoutine());
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

}