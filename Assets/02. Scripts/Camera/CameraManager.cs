using DG.Tweening;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public enum ViewMode { FPS, TPS, Quarter }

    [Header("# View Offsets")]
    [SerializeField] private Transform _fpsView;
    [SerializeField] private Transform _tpsView;
    [SerializeField] private Transform _quarterView;

    [Header("# Target")]
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Transform _playerBody;
    private Transform _cameraTransform;
    private Transform _targetView;

    [Header("# Shake")]
    private Tween _shakeTween;

    public float TransitionSpeed = 5f;

    private ViewMode _currentView = ViewMode.FPS;
    public ViewMode CurrentView => _currentView;

    private void Awake()
    {
        _currentView = ViewMode.FPS;
        _cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        HandleViewChange();
        MoveCamera();
    }

    private void HandleViewChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetViewMode(ViewMode.FPS);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SetViewMode(ViewMode.TPS);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SetViewMode(ViewMode.Quarter);
    }

    private void SetViewMode(ViewMode newView)
    {
        _currentView = newView;
        Debug.Log($"Current View: {_currentView}");
    }

    private void MoveCamera()
    {
        if (_shakeTween.IsActive())
        {
            return;
        }
        _targetView = GetTargetView();

        if (_currentView == ViewMode.TPS)
        {
            _cameraTransform.position = _targetView.position;
        }
        else if (_currentView == ViewMode.Quarter)
        {
            _cameraTransform.position = _targetView.position;
            _cameraTransform.rotation = Quaternion.Euler(45f, 0, 0f);
        }
        else
        {
            // FPS 모드에서는 카메라가 플레이어의 회전을 그대로 따르도록 설정
            _cameraTransform.position = _fpsView.position;
        }
    }

    public void ShakeCamera(float duration = 0.5f, float strength = 0.5f, int vibrato = 10, float randomness = 90f)
    {
        if (_shakeTween != null && _shakeTween.IsActive())
        {
            _shakeTween.Kill();
            _cameraTransform.position = GetTargetView().position;
        }

        _shakeTween = _cameraTransform
            .DOShakePosition(duration, strength, vibrato, randomness, false, true)
            .OnComplete(() => _cameraTransform.position = GetTargetView().position); // 원위치 복구
    }

    private Transform GetTargetView()
    {
        return _currentView switch
        {
            ViewMode.FPS => _fpsView,
            ViewMode.TPS => _tpsView,
            ViewMode.Quarter => _quarterView,
            _ => _tpsView,
        };
    }
}