using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UnityEvent changedToMenu = new UnityEvent();
    public UnityEvent changedToPlay = new UnityEvent();
    public UnityEvent changedToPause = new UnityEvent();
    public UnityEvent disableControls = new UnityEvent();
    public UnityEvent enableControls = new UnityEvent();
    public UnityEvent changedToLoading = new UnityEvent();
    public UnityEvent<Transform> changedToKillcam = new UnityEvent<Transform>();

    public GameState gameState = GameState.Menu;

    public bool controllable = true;

    public bool invincible = false;

    public enum GameState{
        Menu,
        Pause,
        Play,
        Killcam
    }

    void Awake(){
        if (instance!=null)
            Destroy(gameObject);
        else 
            instance = this;
            
        DontDestroyOnLoad(gameObject);
        InputManager.escape.AddListener(Pause);
    }

    void Start()
    {
        if (!PlayerController.instance)
            OnMenu();
        else
            OnPlay();
    }

    public void StartPlay(){
        GameSceneManager.instance.LoadScene(1);
        OnPlay();
    }

    public void ExitToMenu(){
        GameSceneManager.instance.LoadMenu();
        OnMenu();
    }

    public void Resume(){
        OnPlay();
    }
    public void Pause(){
        if (gameState == GameState.Pause)
            OnPlay();
        else
        if (gameState == GameState.Play)
            OnPause();
    }

    public void Restart(){
        OnPlay();
        GameSceneManager.instance.LoadCurrentLevel();
    }

    public void KillPlayerByEnemy(Transform enemy){
        if (invincible) return;
        if (gameState == GameState.Play|| gameState == GameState.Pause)
            StartKillCamera(enemy);
    }

    public void OnMenu(){
        gameState = GameState.Menu;
        //Time.timeScale = 1f;
        SetCursorVisibility(true);
        changedToMenu.Invoke();
    }

    public void OnPlay(){
        enableControls.Invoke();
        gameState = GameState.Play;
       // Time.timeScale = 1f;
        SetCursorVisibility(false);
        changedToPlay.Invoke();

        //GameSceneManager.instance.onEndLoading.RemoveListener(OnPlay);
    }

    public void OnPause(){
        if (gameState == GameState.Menu)
            return;
            
        disableControls.Invoke();
        gameState = GameState.Pause;
        //Time.timeScale = 0f;
        SetCursorVisibility(true);
        changedToPause.Invoke();
    }

    public void StartKillCamera(Transform killerPosition){
        //print("СМЕРТЬ");
        gameState = GameState.Killcam;
        disableControls.Invoke();
        changedToKillcam.Invoke(killerPosition);
    }

    public void OnLoading(){
        changedToLoading.Invoke();
    }

   
    public static void SetCursorVisibility(bool visible){
        if (!visible) Cursor.lockState = CursorLockMode.Confined;
        else 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = visible;
    }

    public void Exit(){
        Application.Quit();
    }
}
