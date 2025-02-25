using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static Vector2 move{get; private set;} = Vector2.zero;
    public static Vector3 moveDirection {get; private set;} = Vector3.zero;
    public static Vector2 lookDirection {get; private set;} = Vector2.zero;

    public static UnityEvent<bool> interact {get; private set;} = new UnityEvent<bool>();
    public static UnityEvent<bool> ability {get; private set;} = new UnityEvent<bool>();

    void OnMove(InputValue value) => move = value.Get<Vector2>();
    void OnLook(InputValue value) => lookDirection = value.Get<Vector2>()/5f;
    void OnFire(InputValue value) => interact.Invoke(value.isPressed);
    void OnInteract(InputValue value) => interact.Invoke(value.isPressed);
    void OnJump(InputValue value) => ability.Invoke(value.isPressed);

    void Update()
    {
        moveDirection = ApplyCameraRotation(move);
    }

    Vector3 ApplyCameraRotation(Vector3 vector){
        vector = new Vector3(vector.x, 0, vector.y);
        return Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * vector;
    }
}
