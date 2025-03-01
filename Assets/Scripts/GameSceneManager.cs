using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] Volume loadingVolume;
    [SerializeField] float volumeChangeDuration = 0.1f;

    public static GameSceneManager instance;

    public UnityEvent onStartLoading = new UnityEvent();
    public UnityEvent onEndLoading = new UnityEvent();

    void Awake(){
        if (instance) return;
        instance = this;
    }

    void Start(){
        DisableVolume();
    }

    public void LoadNextLevel(){
        
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        currentLevel++;
        if(SceneManager.GetSceneByBuildIndex(currentLevel)==null)
        {
            currentLevel = 0;
        }
        LoadScene(currentLevel);
    }
//
    public void LoadScene(int index){
        StartCoroutine(LoadSceneAsync(index));
    }

    IEnumerator LoadSceneAsync(int index){
        onStartLoading.Invoke();
        EnableVolume();
        yield return new WaitForSecondsRealtime(volumeChangeDuration);
        
        SceneManager.LoadSceneAsync(index);
        DisableVolume();
        
        onEndLoading.Invoke();
    }

    public void LoadCurrentLevel(){
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        LoadScene(currentLevel);
    }

    public void LoadMenu(){
        LoadScene(0);
    }

    void EnableVolume(){
        DOTween.To(GetVolumeWeight,SetVolumeWeight,1,volumeChangeDuration); 
    }

    void DisableVolume(){
        DOTween.To(GetVolumeWeight,SetVolumeWeight,0,volumeChangeDuration); 
    }

    float GetVolumeWeight(){
        return loadingVolume.weight;
    }
    void SetVolumeWeight(float value){
        loadingVolume.weight = value;
    }
}
