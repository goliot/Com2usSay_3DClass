using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class UI_LoadingScene : MonoBehaviour
{
    [Header("# UI")]
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Slider _slider;

    [Header("# Scripts")]
    [SerializeField] private List<string> _loadingScripts;

    private void Start()
    {
        StartCoroutine(CoLoadNextScene());
    }

    private IEnumerator CoLoadNextScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneLoader.NextSceneIndex);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progressRatio = Mathf.Clamp01(op.progress / 0.9f);
            _slider.value = progressRatio;

            if (_loadingScripts.Count > 0)
            {
                int index = Mathf.Min(Mathf.FloorToInt(progressRatio * _loadingScripts.Count), _loadingScripts.Count - 1);
                _progressText.text = $"{_loadingScripts[index]}...{_slider.value * 100}%";
            }
            else
            {
                _progressText.text = $"Loading...{_slider.value * 100}%";
            }

            if (progressRatio >= 1f)
            {
                yield return new WaitForSeconds(0.5f); // 잠깐 보여주기
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}