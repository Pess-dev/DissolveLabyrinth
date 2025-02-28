using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Movement movement;
    [SerializeField] Transform cameraTransform;

    [SerializeField] float cameraMaxAngle = 90f;
    float verticalLookRotation = 0f;

    public static Vector3 deltaPosition {get; private set;} = Vector3.zero;
    public static Vector3 position {get; private set;} = Vector3.zero;
    public static PlayerController instance {get; private set;}

    bool isControllable = true;

    void Awake(){
        instance = this;

    }

    void Start(){
        movement = GetComponent<Movement>();
        //cameraTransform = Camera.main.transform;
        transform.position = Vector3.Project(transform.position, Vector3.up); 
        if (GameManager.gameState == GameManager.GameState.Play)
            EnableControls();
        else 
            DisableControls();
        GameManager.instance.changedToPlay.AddListener(EnableControls);
        GameManager.instance.changedToPause.AddListener(DisableControls);
    }

    void Update()
    {
        if (!isControllable) 
            return;

        deltaPosition = movement.deltaPosition;
        position = transform.position;
        movement.SetMoveDirection(InputManager.moveDirection);
        
        cameraTransform.Rotate(Vector3.up * InputManager.lookDirection.x);
        
        verticalLookRotation -= InputManager.lookDirection.y; 
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -cameraMaxAngle, cameraMaxAngle);
        cameraTransform.localEulerAngles = new Vector3(verticalLookRotation, cameraTransform.localEulerAngles.y, 0);

    }

    void DisableControls(){
        isControllable = false;
    }
    
    void EnableControls(){
        isControllable = true;
    }
}