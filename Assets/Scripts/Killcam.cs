using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Killcam : MonoBehaviour
{
    public float offsetAnimationTime = 0.2f;
    public float delayTime = 0.5f;
    [SerializeField] CinemachineCamera playerCamera;
    [SerializeField] CinemachineCamera killCamera;
    [SerializeField] Animation _animation;
    CinemachineBrain brain;

    public UnityEvent onCatch = new UnityEvent();
    public UnityEvent onDeath = new UnityEvent();

    void Start()
    {
        GameManager.instance.changedToKillcam.AddListener(StartKillcam);
        brain = Camera.main.gameObject.GetComponent<CinemachineBrain>();
    }

    void Update()
    {
        
    }

    bool isLooking = false;
    void StartKillcam(Transform killer){
        //
        DissolveController.instance.Deactivate();
        Transform originalPlayerCamera = Camera.main.transform;
        playerCamera.transform.localPosition = killer.InverseTransformPoint(originalPlayerCamera.transform.position);
        playerCamera.transform.localRotation = Quaternion.LookRotation(killer.InverseTransformDirection(originalPlayerCamera.transform.forward), Vector3.up);
        isLooking = Vector3.Dot(playerCamera.transform.forward,Vector3.forward)<0;
        StartCoroutine(StartingKillCam());
    }

    IEnumerator StartingKillCam(){
        playerCamera.Priority = 2;
        
        yield return new WaitForEndOfFrame();
        
        onCatch.Invoke();
        if (!isLooking)
            yield return new WaitForSecondsRealtime(delayTime); 
        
        killCamera.Priority = 3;
        
        yield return new WaitForSecondsRealtime(offsetAnimationTime); 
        _animation.Play();
    }

    public void EndKill(){
        _animation.Stop();
        GameManager.instance.Restart();
    }

    public void Death(){
        onDeath.Invoke();
    }
}