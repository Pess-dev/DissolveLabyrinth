using UnityEngine;
using UnityEngine.Events;

public class GameSettingsManager : MonoBehaviour
{
    public float mainSensetivity = 3f;

    float sensitivity = 1f;
    float audioVolume = 1f;

    public UnityEvent<float> sensitivityChanged = new UnityEvent<float>();
    public UnityEvent<float> audioVolumeChanged = new UnityEvent<float>();

    void Start()
    {
        // Загрузка сохранённых настроек или установка значений по умолчанию
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
        audioVolume = PlayerPrefs.GetFloat("AudioVolume", 1f);
        
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

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetFloat("AudioVolume", audioVolume);
        PlayerPrefs.Save();
    }
}
