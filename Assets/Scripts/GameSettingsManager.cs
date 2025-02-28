using UnityEngine;
using UnityEngine.Events;

public class GameSettingsManager : MonoBehaviour
{
    public float sensitivity = 1f;
    public float audioVolume = 1f;

    public UnityEvent<float> sensitivityChanged = new UnityEvent<float>();
    public UnityEvent<float> audioVolumeChanged = new UnityEvent<float>();

    void Start()
    {
        // Загрузка сохранённых настроек или установка значений по умолчанию
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);
        audioVolume = PlayerPrefs.GetFloat("AudioVolume", 1f);
        
        // Уведомление других компонентов об изменениях
        sensitivityChanged.Invoke(sensitivity);
        audioVolumeChanged.Invoke(audioVolume);
    }

    public void SetSensitivity(float value)
    {
        sensitivity = value;
        sensitivityChanged.Invoke(sensitivity);
    }

    public void SetAudioVolume(float value)
    {
        audioVolume = value;
        audioVolumeChanged.Invoke(audioVolume);
    }

    void OnApplicationQuit()
    {
        // Сохранение настроек при завершении приложения
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetFloat("AudioVolume", audioVolume);
        PlayerPrefs.Save();
    }
}
