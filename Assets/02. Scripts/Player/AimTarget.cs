using UnityEngine;

public class AimTarget : MonoBehaviour
{
    private Ray _ray;
    private RaycastHit _hitInfo;

    private Vector3 hitPoint;
    private bool _isHit;

    void LateUpdate()
    {
        _ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        _isHit = Physics.Raycast(_ray, out _hitInfo, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Player") | (1 << LayerMask.NameToLayer("MapEffect"))));

        if (_isHit)
        {
            hitPoint = _hitInfo.point;
            transform.position = _hitInfo.point;
        }
        else
        {
            hitPoint = _ray.origin + _ray.direction * 50f;
            transform.position = hitPoint;
        }
    }
}
