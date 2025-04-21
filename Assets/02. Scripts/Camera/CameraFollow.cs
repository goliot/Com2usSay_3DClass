using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _targetTransform;

    private void Update()
    {
        transform.position = _targetTransform.position;
    }
}