using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private const float GRAVITY = -9.8f;

    private CharacterController _characterController;
    public float YVelocity { get; set; }

    public PlayerMovementController(CharacterController characterController)
    {
        _characterController = characterController;
    }

    public void Move(Vector3 direction, float speed)
    {
        direction.y = YVelocity;
        _characterController.Move(direction * speed * Time.deltaTime);
    }

    public void UpdateGravity(bool isGrounded)
    {
        if (isGrounded)
        {
            YVelocity = 0f;
        }
        else
        {
            YVelocity += GRAVITY * Time.deltaTime;
        }
    }
}