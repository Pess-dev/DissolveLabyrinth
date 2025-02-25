using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float acceleration = 5f;

    CharacterController cc;

    Vector3 moveDirection = Vector3.zero;
    Vector3 flatVelocity = Vector3.zero;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    public void SetMoveDirection(Vector3 direction){
        moveDirection = Vector3.ClampMagnitude(direction,1);
    }
    
    public void AddFlatVelocity(Vector3 velocity){
        flatVelocity += Vector3.ProjectOnPlane(velocity, Vector3.up);
    }

    void Update()
    {
        // Vector3 direction = moveDirection;
        // Vector3 flatVelocity = Vector3.ProjectOnPlane(rb.linearVelocity,Vector3.up);
        // direction = (direction*speed-flatVelocity)/acceleration/Time.fixedDeltaTime;
        // direction = Vector3.ClampMagnitude(direction,1); 
        // rb.AddForce(direction * acceleration, ForceMode.Acceleration);
        // print(direction);

        flatVelocity += moveDirection*acceleration*Time.deltaTime;
        flatVelocity = Vector3.ClampMagnitude(flatVelocity,speed);
        if (moveDirection.magnitude < 0.1f) flatVelocity = Vector3.ClampMagnitude(flatVelocity, Mathf.Clamp(flatVelocity.magnitude-acceleration*Time.deltaTime,0,speed));
        cc.Move((flatVelocity - Vector3.up*2)*Time.deltaTime);
        
    }
}