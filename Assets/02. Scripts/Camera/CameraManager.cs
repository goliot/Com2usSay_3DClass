using UnityEngine;
using UnityEngine.InputSystem;

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
        Transform targetView = GetTargetView();

        if (_currentView == ViewMode.TPS)
        {
            //_cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetView.position, Time.deltaTime * TransitionSpeed);
            _cameraTransform.position = targetView.position;
            //_cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, targetView.rotation, Time.deltaTime * TransitionSpeed);
        }
        else if(_currentView == ViewMode.Quarter)
        {
            _cameraTransform.position = targetView.position;
            _cameraTransform.rotation = Quaternion.Euler(45f, 0, 0f); // 예시: 위에서 바라보는 고정 각도
        }
        else
        {
            // FPS 모드에서는 카메라가 플레이어의 회전을 그대로 따르도록 설정
            _cameraTransform.position = _fpsView.position;
        }
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