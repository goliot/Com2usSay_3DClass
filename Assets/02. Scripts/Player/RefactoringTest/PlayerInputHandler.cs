using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public float Horizontal => Input.GetAxisRaw("Horizontal");
    public float Vertical => Input.GetAxisRaw("Vertical");
    public bool IsJumpPressed => Input.GetKeyDown(KeyCode.Space);
    public bool IsSprintPressed => Input.GetKeyDown(KeyCode.LeftShift);
    public bool IsSprintReleased => Input.GetKeyUp(KeyCode.LeftShift);
    public bool IsRollPressed => Input.GetKeyDown(KeyCode.E);
}
