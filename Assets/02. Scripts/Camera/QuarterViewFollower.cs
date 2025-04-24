using UnityEngine;

public class QuarterViewFollower : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _smoothTime = 0.2f;  // 감속 시간
    private Vector3 _velocity = Vector3.zero;
    [SerializeField] private Vector3 _offset;

    void LateUpdate()
    {
        Vector3 targetPosition = _player.transform.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
    }
}