using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Killcam : MonoBehaviour
{
    public float offsetAnimationTime = 0.2f;
    public float delayTime = 0.5f;
    [SerializeField] CinemachineCamera playerCamera;
    [SerializeField] CinemachineCamera killCamera;
    [SerializeField] Animation _animation;
    CinemachineBrain brain;
    void Start()
    {
        GameManager.instance.changedToKillcam.AddListener(StartKillcam);
        brain = Camera.main.gameObject.GetComponent<CinemachineBrain>();
    }

    void Update()
    {
        
    }
    void StartKillcam(Vector3 killerPos){
        //
        StartCoroutine(StartingKillCam());
    }

    IEnumerator StartingKillCam(){
        playerCamera.Priority = 2;
        yield return new WaitForSecondsRealtime(delayTime); 
        
        killCamera.Priority = 3;
        
        yield return new WaitForSecondsRealtime(offsetAnimationTime); 
        _animation.Play();
    }

    public void EndKill(){
        _animation.Stop();
        GameManager.instance.Restart();
    }
}