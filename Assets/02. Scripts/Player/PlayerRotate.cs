using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 150f;

    private float _rotationX = 0f;

    private void Update()
    {
        _rotationX += Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0, _rotationX, 0);
    }
}
