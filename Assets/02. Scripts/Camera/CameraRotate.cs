using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private Transform _playerBody; // TPS 회전을 위해 필요
    [SerializeField] private float _rotationSpeed = 150f;
    [SerializeField] private float _tpsDistance = 3f;

    private float _rotationX = 0f;
    private float _rotationY = 0f;

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;

        switch (CameraManager.Instance.CurrentView)
        {
            case CameraManager.ViewMode.FPS:
                HandleFPSRotation(mouseX, mouseY);
                break;

            case CameraManager.ViewMode.TPS:
                HandleTPSRotation(mouseX, mouseY);
                break;

            case CameraManager.ViewMode.Quarter:
                break;
        }
    }

    private void HandleFPSRotation(float mouseX, float mouseY)
    {
        _rotationX += mouseX;
        _rotationY -= mouseY;
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);
        transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0f);
    }

    private void HandleTPSRotation(float mouseX, float mouseY)
    {
        _rotationX += mouseX;
        _rotationY -= mouseY;
        _rotationY = Mathf.Clamp(_rotationY, -45f, 45f);

        // 카메라 자체를 회전시키는 게 아니라, "타겟(뷰)"의 회전을 바꾼다
        transform.localRotation = Quaternion.Euler(_rotationY, _rotationX, 0);
    }
}