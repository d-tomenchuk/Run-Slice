using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float minSpeed = 10f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float startPositionX = 0f;
    [SerializeField] private float endPositionX = 3000f;
    [SerializeField] private float stoppingTime = 0.7f;
    [SerializeField] private float nearStopThreshold = 1f; // Скорость, ниже которой считается, что камера почти остановилась

    private float currentSpeed;
    private bool isStopping = false;
    private float stopTimer = 0f;
    private bool isPermanentlyStopped = false; // Флаг для полной остановки камеры

    public delegate void NearStopAction();
    public event NearStopAction OnNearStop; // Событие, которое вызывается при почти полной остановке

    void Update()
    {
        if (isPermanentlyStopped) {
            currentSpeed = 0; // Обеспечение того, что скорость остаётся нулевой, если камера окончательно остановлена
            return; // Прекращение дальнейшей обработки
        }

        Vector3 position = transform.position;
        float lerpFactor = Mathf.Clamp((position.x - startPositionX) / (endPositionX - startPositionX), 0, 1);
        currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, lerpFactor);

        if (isStopping) {
            stopTimer += Time.deltaTime;
            currentSpeed = Mathf.Lerp(currentSpeed, 0, stopTimer / stoppingTime);
            if (stopTimer >= stoppingTime) {
                StopCamera();
                isStopping = false;
            }
        }

        position.x += currentSpeed * Time.deltaTime;
        transform.position = position;

        // Проверка, достаточно ли близко скорость к нулю для срабатывания события "почти остановка"
        if (currentSpeed <= nearStopThreshold && OnNearStop != null)
        {
            OnNearStop.Invoke();
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void BeginStopping()
    {
        isStopping = true;
        stopTimer = 0f;
    }

    public void StopCamera()
    {
        isPermanentlyStopped = true;
        currentSpeed = 0f;
    }

    public void ResumeCamera()
    {
        isPermanentlyStopped = false;
        isStopping = false;
        stopTimer = 0f;
        currentSpeed = minSpeed; // Сброс до минимальной скорости или другой логики начальной скорости
    }
}
