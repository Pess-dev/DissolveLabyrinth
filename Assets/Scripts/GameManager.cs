using Unity.Cinemachine;
using UnityEditor.SearchService;
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
    public UnityEvent<Vector3> changedToKillcam = new UnityEvent<Vector3>();

    public static GameState gameState = GameState.Menu;

    public bool controllable = true;

    public enum GameState{
        Menu,
        Pause,
        Play,
        Killcam,
        Loading
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
        GameSceneManager.instance.LoadCurrentLevel();
        OnPlay();
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

    public void OnKillcam(Vector3 killerPosition){
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
