using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static float sensitivity = 1f;
    public static Vector2 move{get; private set;} = Vector2.zero;
    public static Vector3 moveDirection {get; private set;} = Vector3.zero;
    public static Vector2 look {get; private set;} = Vector2.zero;

    public static UnityEvent<bool> interact {get; private set;} = new UnityEvent<bool>();
    public static UnityEvent<bool> ability {get; private set;} = new UnityEvent<bool>();
    public static UnityEvent escape {get; private set;} = new UnityEvent();

    void OnMove(InputValue value) => move = value.Get<Vector2>();
    void OnLook(InputValue value) => look = value.Get<Vector2>()/5f*sensitivity;
    void OnFire(InputValue value) => interact.Invoke(value.isPressed);
    void OnInteract(InputValue value) => interact.Invoke(value.isPressed);
    void OnJump(InputValue value) => ability.Invoke(value.isPressed);
    void OnEscape(InputValue value) => escape.Invoke();

    void Update()
    {
        moveDirection = ApplyCameraRotation(move);
        if(Input.GetKeyDown(KeyCode.P)){
            GameSceneManager.instance.LoadNextLevel();
        }
        if(Input.GetKeyDown(KeyCode.O)){
            GameSceneManager.instance.LoadCurrentLevel();
        }
        if(Input.GetKeyDown(KeyCode.I)){
            CollectableManager.instance.collectedAll.Invoke();
        }
    }

    Vector3 ApplyCameraRotation(Vector3 vector){
        vector = new Vector3(vector.x, 0, vector.y);
        return Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * vector;
    }
}
