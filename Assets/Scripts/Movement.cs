using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float rotateSpeed = 10f;


    float minimumSpeed = 0f;

    CharacterController cc;

    Vector3 moveDirection = Vector3.zero;
    Vector3 lookDirection = Vector3.forward;
    Vector3 flatVelocity = Vector3.zero;

    Vector3 oldPosition;
    public Vector3 deltaPosition {get; private set;} = Vector3.zero;
    

    Dictionary<string,float> modifiers = new Dictionary<string, float>();

    public UnityEvent<Vector3> velocityChanged = new UnityEvent<Vector3>(); 

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    public void SetMoveDirection(Vector3 direction){
        moveDirection = Vector3.ClampMagnitude(direction,1);
    }

    public void SetLookDirection(Vector3 direction){
        lookDirection = Vector3.ClampMagnitude(direction,1);
        lookDirection = Vector3.ProjectOnPlane(direction,Vector3.up).normalized;
    }

    public void YRotate(float angle){
        transform.Rotate(Vector3.up*angle);
        lookDirection = transform.forward;
    }
    
    public void SetModifier(string name, float modifier){
        modifiers.TryAdd(name,modifier);
    }

    public void RemoveModifier(string name){
        modifiers.Remove(name);
    }

    public void ResetModifiers(){
        modifiers.Clear();
    }

    public void SetMinimumSpeed(float value){
        minimumSpeed = value;
    }
    public void ResetMinimumSpeed(){
        minimumSpeed = 0f;
    }


    public void AddFlatVelocity(Vector3 velocity){
        flatVelocity += Vector3.ProjectOnPlane(velocity, Vector3.up);
    }

    void Update(){
        Move();
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(lookDirection),rotateSpeed*Time.deltaTime);
    }

    void Move(){
        // Vector3 direction = moveDirection;
        // Vector3 flatVelocity = Vector3.ProjectOnPlane(rb.linearVelocity,Vector3.up);
        // direction = (direction*speed-flatVelocity)/acceleration/Time.fixedDeltaTime;
        // direction = Vector3.ClampMagnitude(direction,1); 
        // rb.AddForce(direction * acceleration, ForceMode.Acceleration);
        // print(direction);
        float modifier = 1;
        if (modifiers.Count>0) modifier = modifiers.Values.Aggregate((acc, value) => acc * value);
        float modifiedAcceleration = acceleration*modifier*modifier;
        float modifiedSpeed = speed*modifier;

        flatVelocity += moveDirection*modifiedAcceleration*Time.deltaTime;
        flatVelocity = Vector3.ClampMagnitude(flatVelocity,modifiedSpeed);
        if (flatVelocity.magnitude<minimumSpeed&&flatVelocity.magnitude!=0) flatVelocity = flatVelocity.normalized*minimumSpeed;
        else 
        if (flatVelocity.magnitude==0&&flatVelocity.magnitude<minimumSpeed) flatVelocity = transform.forward;
        if (moveDirection.magnitude < 0.1f) 
            flatVelocity = Vector3.ClampMagnitude(flatVelocity, Mathf.Clamp(flatVelocity.magnitude-modifiedAcceleration*Time.deltaTime,0,modifiedSpeed));
        
        Vector3 pos = transform.position;
        cc.Move((flatVelocity - Vector3.up*2)*Time.deltaTime);
        Vector3 after = transform.position;
        deltaPosition = transform.position - pos;

        velocityChanged.Invoke(flatVelocity);
        
        //transform.position = Vector3.Project(transform.position, Vector3.up); 
       // print(pos+" "+after+" "+transform.position);
    }
}