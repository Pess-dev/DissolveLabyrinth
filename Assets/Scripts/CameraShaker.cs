using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [Header("Settings")]
    public float magnitude = 0.1f; // Сила тряски (максимальное смещение)
    public float minSpeed = 1f; // Сила тряски (максимальное смещение)
    public float maxSpeed = 5f;      // Скорость изменения позиции

    public float value = 0f;

    private Vector3 initialPosition; // Исходная позиция камеры
    private float seedX, seedY;      // Случайные начальные значения для шума

    private float timer = 0f;

    void Awake()
    {
        // Сохраняем исходную позицию камеры
        initialPosition = transform.localPosition;

        // Генерация случайных начальных значений для шума
        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
    }

    public void SetValue(float value){
        this.value = value;
    }

    void Update()
    {
        // Рассчет смещения с использованием шума Перлина
        float speed = value*(maxSpeed - minSpeed) + minSpeed;
        
        timer += Time.deltaTime*speed;

        float xOffset = (Mathf.PerlinNoise(seedX + timer, 0f) - 0.5f) * magnitude;
        float yOffset = (Mathf.PerlinNoise(0f, seedY + timer) - 0.5f) * magnitude;

        // Применение нового положения
        transform.localPosition = initialPosition + new Vector3(xOffset, yOffset, 0f);
    }
}