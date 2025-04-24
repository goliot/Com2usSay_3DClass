using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _yOffset = 10f;
    private Camera _camera;

    private void Awake()
    {
        if (_target == null)
        {
            _target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Vector3 newPosition = _target.position;
        newPosition.y += _yOffset;

        transform.position = newPosition;

        Vector3 newEulerAngles = _target.eulerAngles;
        newEulerAngles.x = 90;
        newEulerAngles.z = 0;
        transform.eulerAngles = newEulerAngles;
    }

    public void OnClickMinimapZoomIn()
    {
        _camera.orthographicSize = Mathf.Max(1, _camera.orthographicSize -1);
    }

    public void OnClickMinimapZoomOut()
    {
        _camera.orthographicSize = Mathf.Min(20, _camera.orthographicSize + 1);
    }
}