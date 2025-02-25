using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float acceleration = 5f;


    CharacterController cc;

    Vector3 moveDirection = Vector3.zero;
    Vector3 flatVelocity = Vector3.zero;

    Dictionary<string,float> modifiers = new Dictionary<string, float>();

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    public void SetMoveDirection(Vector3 direction){
        moveDirection = Vector3.ClampMagnitude(direction,1);
    }
    
    public void SetModifier(string name, float modifier){
        modifiers.TryAdd(name,modifier);
    }

    public void RemoveModifier(string name){
        modifiers.Remove(name);
    }

    public void ResetModifier(){
        modifiers.Clear();
    }


    public void AddFlatVelocity(Vector3 velocity){
        flatVelocity += Vector3.ProjectOnPlane(velocity, Vector3.up);
    }

    void Update()
    {
        Move();
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
        if (moveDirection.magnitude < 0.1f) 
            flatVelocity = Vector3.ClampMagnitude(flatVelocity, Mathf.Clamp(flatVelocity.magnitude-modifiedAcceleration*Time.deltaTime,0,modifiedSpeed));
        cc.Move((flatVelocity - Vector3.up*2)*Time.deltaTime);
    }
}