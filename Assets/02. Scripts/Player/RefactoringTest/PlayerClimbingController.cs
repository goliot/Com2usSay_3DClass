using UnityEngine;

public class PlayerClimbingController : MonoBehaviour
{
    private LayerMask _wallLayer;
    private float _wallCheckDistance;
    private bool _isClimbingWall;

    public PlayerClimbingController(LayerMask wallLayer, float wallCheckDistance)
    {
        _wallLayer = wallLayer;
        _wallCheckDistance = wallCheckDistance;
    }

    public bool CheckWallInFront(Vector3 position, Vector3 forward)
    {
        Vector3 origin = position + Vector3.down;
        return Physics.Raycast(origin, forward, _wallCheckDistance, _wallLayer);
    }

    public bool IsClimbingWall => _isClimbingWall;
}
