using UnityEngine;
using UnityEngine.Events;

public class GameSettingsManager : MonoBehaviour
{
    public float mainSensetivity = 3f;

    public float sensitivity {get; private set;} = 1f;
    public float audioVolume {get; private set;} = 1f;

    public UnityEvent<float> sensitivityChanged = new UnityEvent<float>();
    public UnityEvent<float> audioVolumeChanged = new UnityEvent<float>();

    public static GameSettingsManager instance;

    void Awake(){
        if (instance) return;
        instance = this;
    }

    void Start()
    {
        // Загрузка сохранённых настроек или установка значений по умолчанию
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
        audioVolume = PlayerPrefs.GetFloat("AudioVolume", 0.5f);

        GameSceneManager.instance.onEndLoading.AddListener(UpdateValues);
        
        // Уведомление других компонентов об изменениях
        SetAudioVolume(audioVolume);
        SetSensitivity(sensitivity);
        Application.targetFrameRate = 90;
    }

    public void SetSensitivity(float value)
    {
        sensitivity = value;
        sensitivityChanged.Invoke(sensitivity);
        InputManager.sensitivity = sensitivity*mainSensetivity;
    }

    public void SetAudioVolume(float value)
    {
        audioVolume = value;
        audioVolumeChanged.Invoke(audioVolume);
        AudioListener.volume = audioVolume;
    }

    void Update(){
    }



    void UpdateValues(){
        AudioListener.volume = audioVolume;
        InputManager.sensitivity = sensitivity*mainSensetivity;
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetFloat("AudioVolume", audioVolume);
        PlayerPrefs.Save();
    }
}
