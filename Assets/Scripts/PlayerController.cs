using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Movement movement;
    Transform cameraTransform;

    [SerializeField] float cameraMaxAngle = 90f;
    float verticalLookRotation = 0f;

    void Start(){
        movement = GetComponent<Movement>();
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        movement.SetMoveDirection(InputManager.moveDirection);
        
        transform.Rotate(Vector3.up * InputManager.lookDirection.x);
        
        verticalLookRotation -= InputManager.lookDirection.y; 
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -cameraMaxAngle, cameraMaxAngle);
        cameraTransform.localEulerAngles = new Vector3(verticalLookRotation, 0, 0);
    }

    
}