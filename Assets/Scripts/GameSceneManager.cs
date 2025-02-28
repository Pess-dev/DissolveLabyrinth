using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] int menuNumber = 0;
    [SerializeField] int levelCount = 3;

    static int _menuNumber = 0;
    void Start(){
        
    }

    public static void LoadNextLevel(){
        
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        currentLevel++;
        if(SceneManager.GetSceneByBuildIndex(currentLevel)==null)
        {
            currentLevel = 0;
        }
        LoadScene(currentLevel);
    }
//
    public static void LoadScene(int index){
        SceneManager.LoadScene(index);
    }

    public static void LoadMenu(){
        LoadScene(_menuNumber);
    }
}
