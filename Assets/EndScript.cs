using Unity.Cinemachine;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    [SerializeField] float camera1Delay = 1f;
    [SerializeField] float camera2Delay = 6f;
    [SerializeField] float UIDelay = 7f;

    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] GameObject UI1;
    [SerializeField] GameObject UI2;

    float timer = 0f;
    
    void Start(){
        GameManager.instance.OnEnd();
    }

    void Update()
    {
       // if (GameManager.instance.gameState != GameManager.GameState.End)
       //     GameManager.instance.OnEnd();

        timer += Time.deltaTime;

        if(timer > camera1Delay ) {
            cinemachineCamera.Priority = 1;
        }
        if (timer > camera2Delay ) {
            UI1.SetActive(true);
        }
        if (timer > UIDelay){
            UI2.SetActive(true);
        }
    }
}
