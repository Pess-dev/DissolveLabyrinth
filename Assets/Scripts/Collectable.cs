using UnityEngine;
using UnityEngine.Events;

public class Collectable : MonoBehaviour
{
    [SerializeField] float timeToCollect = 5f;
    [SerializeField] float reverseSpeed = 0.5f;
    [SerializeField] float distanceToPlayer = 2f;

    [SerializeField] Animation _animation;
    
    float timer = 0;

    bool isCollecting = false;
    bool isCollected = false;
        
    public UnityEvent collected = new UnityEvent();
    public UnityEvent<float> collectingProgress = new UnityEvent<float>();

    private void Update() {
        if (isCollected) 
            return;

        CheckPlayer();

        if (isCollecting)
            timer += Time.deltaTime;
        else 
            timer = Mathf.Clamp(timer - Time.deltaTime*reverseSpeed,0, timer);

        collectingProgress.Invoke(timer/timeToCollect);

        if (timer>=timeToCollect) {
            isCollected = true;
            CollectableManager.instance.AddCollectable();
            _animation.Play();
            collected.Invoke();
        }
    }

    void CheckPlayer(){
        if (FlatDistance(transform.position, PlayerController.position)<distanceToPlayer) {
            isCollecting = true;
        }
        else {
            isCollecting = false;
        }
    }

    protected float FlatDistance(Vector3 point1, Vector3 point2){
        return Vector3.ProjectOnPlane(point1-point2,Vector3.up).magnitude;
    }
}
