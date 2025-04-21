using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 150f;

    //오일러 각도는 0~360도 -> 0에서 시작한다고 기준을 세운다.
    private float _rotationX = 0f;
    private float _rotationY = 0f;

    private void Update()
    {
        //1. 마우스 입력을 받는다.(커서의 움직임 방향)
        //2. 마우스 입력으로부터 회전시킬 방향을 만든다.
        _rotationX += Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
        _rotationY -= Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);

        //3. 카메라를 그 방향으로 회전시킨다.
        transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0);
    }
}
