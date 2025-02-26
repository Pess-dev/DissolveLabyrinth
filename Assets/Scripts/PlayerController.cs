using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Movement movement;
    [SerializeField] Transform cameraTransform;

    [SerializeField] float cameraMaxAngle = 90f;
    float verticalLookRotation = 0f;

    public static Vector3 deltaPosition {get; private set;} = Vector3.zero;
    public static Vector3 position {get; private set;} = Vector3.zero;

    
    
    void Awake(){
    }

    void Start(){
        movement = GetComponent<Movement>();
        //cameraTransform = Camera.main.transform;
        transform.position = Vector3.Project(transform.position, Vector3.up); 
    }

    void Update()
    {
        deltaPosition = movement.deltaPosition;
        position = transform.position;
        movement.SetMoveDirection(InputManager.moveDirection);
        
        transform.Rotate(Vector3.up * InputManager.lookDirection.x);
        
        verticalLookRotation -= InputManager.lookDirection.y; 
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -cameraMaxAngle, cameraMaxAngle);
        cameraTransform.localEulerAngles = new Vector3(verticalLookRotation, 0, 0);

    }
}