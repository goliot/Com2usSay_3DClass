using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 150f;
    private float _rotationX = 0f;

    private void Update()
    {
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
        _rotationX += mouseX;

        // 플레이어의 회전 적용
        transform.eulerAngles = new Vector3(0, _rotationX, 0);
    }
}