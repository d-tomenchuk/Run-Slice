using UnityEngine;

public class TimeChange : MonoBehaviour
{
    public float timeChangeSpeed = 0.2f;  // Скорость изменения времени
    public Material material;  // Материал, который будет изменяться
    public Light directionalLight;  // Ссылка на источник света

    public float maxIntensity = 1.0f;  // Максимальная интенсивность света в дневное время
    public float nightIntensityFactor = 0.5f;  // Фактор ночной интенсивности

    public Transform lightTransform;  // Трансформ источника света для вращения

    void Start()
    {   
        // Генерируем случайное начальное смещение для текстуры неба от 0 до 0.2
        float initialOffset = Random.Range(0f, 0.2f);
        material.mainTextureOffset = new Vector2(initialOffset, material.mainTextureOffset.y);

        UpdateLightingAndRotation(material.mainTextureOffset.x);
    }

    void Update()
    {
        float offset = Time.deltaTime * timeChangeSpeed;
        float newOffsetX = Mathf.Repeat(material.mainTextureOffset.x + offset, 1.0f);  // Повторение значения от 0 до 1
        material.mainTextureOffset = new Vector2(newOffsetX, material.mainTextureOffset.y);

        UpdateLightingAndRotation(material.mainTextureOffset.x);
    }

    void UpdateLightingAndRotation(float time)
    {
        float normalizedTime = time - Mathf.Floor(time);

        float cycleValue = Mathf.Cos(2 * Mathf.PI * normalizedTime);
        float intensity = (cycleValue * 0.5f + 0.5f) * (1 - nightIntensityFactor) + nightIntensityFactor;
        directionalLight.intensity = intensity * maxIntensity;
        directionalLight.color = Color.Lerp(Color.gray, Color.white, intensity);

        float angle = 130 + (Mathf.Sin(2 * Mathf.PI * normalizedTime) + 1) / 2 * 50;
        lightTransform.rotation = Quaternion.Euler(new Vector3(angle, -30, 0));
    }
}
