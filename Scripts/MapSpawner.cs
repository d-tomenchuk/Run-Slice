using UnityEngine;
using System.Collections.Generic;

public class MapSpawner : MonoBehaviour
{
    public GameObject[] mapPrefabs; // Префабы карт
    public GameObject conePrefab; // Префаб конуса
    public GameObject alternativeConePrefab; // Префаб альтернативного конуса
    public Vector3 coneOffset = new Vector3(0, 0, 6.5f); // Смещение конусов
    private float mapLength = 220f; // Длина карты
    private float spawnThreshold = 140f; // Порог спавна новой карты
    private float despawnThreshold = 140f; // Порог удаления старой карты
    public Transform cameraTransform; // Трансформ камеры
    private GameObject newMap; // Переменная для новой карты

    private float lastSpawnX; // Последняя координата спавна
    private float lastReplacementX = -1000; // Последняя позиция успешной замены
    private Queue<GameObject> spawnedMaps = new Queue<GameObject>(); // Очередь заспавненных карт
    private System.Random random = new System.Random(); // Рандомизатор
    private List<Vector3> specialConePositions = new List<Vector3>(); // Позиции специальных конусов
    private int currentMapIndex = 0; // Индекс текущей карты

    private CameraMove cameraMove; // Ссылка на скрипт управления камерой

    void Start()
    {
        Debug.Log("MapSpawner Start");
        // Проверка наличия скрипта CameraMove на камере
        cameraMove = cameraTransform.GetComponent<CameraMove>();
        if (cameraMove == null)
        {
            Debug.LogError("CameraMove component is not found on the camera. Please add it or check the assignment.");
            enabled = false;
            return;
        }
        lastSpawnX = 0f;
        SpawnMap(lastSpawnX); // Спавн первой карты
    }

    void Update()
    {
        float cameraX = cameraTransform.position.x;
        // Проверка необходимости спавна новой карты
        if (cameraX >= lastSpawnX + spawnThreshold)
        {
            lastSpawnX += mapLength;
            currentMapIndex++;
            SpawnMap(lastSpawnX);
        }
        RemoveOldMaps(cameraX); // Удаление старых карт
    }

    private void SpawnMap(float spawnX)
    {
        Debug.Log($"Spawning map at {spawnX}");
        if (mapPrefabs.Length == 0)
        {
            Debug.LogError("No map prefabs assigned. Please assign map prefabs in the Inspector.");
            return;
        }

        // Очистка списка специальных конусов перед спавном новой карты
        
        specialConePositions.Clear();

        // Выбор случайного префаба карты
        int randomIndex = random.Next(mapPrefabs.Length);
        GameObject selectedPrefab = mapPrefabs[randomIndex];
        Vector3 spawnPosition = new Vector3(spawnX, 0f, selectedPrefab.transform.position.z);
        newMap = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        spawnedMaps.Enqueue(newMap);

        // Получение текущей скорости камеры
        float currentSpeed = cameraMove.GetCurrentSpeed();
        int minReplacements = 3;
        int maxReplacements = Mathf.CeilToInt(currentSpeed);
        int sequencesCount = Mathf.Clamp(maxReplacements, minReplacements, maxReplacements);

        // Получение всех возможных мест для замены
        List<Vector3> possiblePositions = GetPossibleReplacementPositions(spawnX);

        Debug.Log($"Possible positions for replacements: {possiblePositions.Count}");

        // Гарантия минимального числа замен
        int successfulReplacements = 0;
        float minDistanceBetweenReplacements = 60f; // Мінімальна відстань між замінами зліва та справа

        while (successfulReplacements < sequencesCount && possiblePositions.Count > 0)
        {
            Vector3 replacementPos = possiblePositions[random.Next(possiblePositions.Count)];

            if (replacementPos != Vector3.zero && replacementPos.x > 100.5f && Mathf.Abs(replacementPos.x - lastReplacementX) >= minDistanceBetweenReplacements && TryReplaceConesAtPosition(replacementPos))
            {
                successfulReplacements++;
                lastReplacementX = replacementPos.x; // Обновление последней позиции замены
                RemoveClosePositions(possiblePositions, replacementPos.x); // Переміщення позицій, близких к последней замене
            }
            else
            {
                possiblePositions.Remove(replacementPos);
            }
        }

        if (successfulReplacements < sequencesCount)
        {
            Debug.LogWarning($"Only {successfulReplacements} replacements were made out of {sequencesCount} attempts.");
        }

        SpawnCones(spawnX); // Спавн конусов
    }

    private List<Vector3> GetPossibleReplacementPositions(float spawnX)
    {
        List<Vector3> positions = new List<Vector3>();
        float maxStartX = spawnX + mapLength - 80;
        for (float x = Mathf.Max(spawnX + 10.5f, 100.5f); x < maxStartX; x += 1f)
        {
            Vector3 leftPosition = new Vector3(x, 0, -coneOffset.z);
            Vector3 rightPosition = new Vector3(x, 0, coneOffset.z);
            bool leftValid = !IsInNoSpawnZone(leftPosition);
            bool rightValid = !IsInNoSpawnZone(rightPosition);
            if (leftValid)
            {
                positions.Add(leftPosition);
            }
            if (rightValid)
            {
                positions.Add(rightPosition);
            }
        }
        
        return positions;
    }

    private bool TryReplaceConesAtPosition(Vector3 startPos)
    {
        List<Vector3> tempPositions = new List<Vector3>();
        float minDistance = 30f; // Мінімальна відстань між замінами

        for (int i = 0; i < 8; i++)
        {   
            Vector3 position = new Vector3(startPos.x + i, 0, startPos.z);
            if (!IsInNoSpawnZone(position) && Mathf.Abs(position.x - lastReplacementX) >= minDistance)
            {
                tempPositions.Add(position);
                
            }
            else
            {
                Debug.Log($"Skipping replacement at position: {position} due to no-spawn zone or being too close to last replacement.");
                return false; // Прерывание, если попали в зону запрета или слишком близко к последней замене
            }
        }

        specialConePositions.AddRange(tempPositions);
        return true;
    }

    private void RemoveClosePositions(List<Vector3> positions, float lastX)
    {
        float minDistance = 30f; // Мінімальна відстань між замінами
    
        // Проходимо по всіх позиціях
        positions.RemoveAll(pos => Mathf.Abs(pos.x - lastX) < minDistance); // Видалення позицій, що занадто близько
    }

    private void SpawnCones(float spawnX)
    {
        for (float x = spawnX + 10.5f; x < spawnX + mapLength + 10f; x += 1f) 
        {
            SpawnCone(x, coneOffset.z);
            SpawnCone(x, -coneOffset.z); 
        }
    }

    private void SpawnCone(float x, float zOffset)
    {
        Vector3 position = new Vector3(x, 0, zOffset);
        GameObject prefabToUse = specialConePositions.Contains(position) ? alternativeConePrefab : conePrefab;

        if (!IsInNoSpawnZone(position))
        {
            GameObject conesObj = Instantiate(prefabToUse, position, Quaternion.identity);
            conesObj.transform.SetParent(newMap.transform);
        }
    }

    bool IsInNoSpawnZone(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f); // Проверка на вхождение в зону запрета
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("NoSpawnZone"))
            {
                return true;
            }
        }
        return false;
    }

    private void RemoveOldMaps(float cameraX)
    {
        while (spawnedMaps.Count > 0)
        {
            GameObject oldMap = spawnedMaps.Peek();
            if (oldMap.transform.position.x + mapLength < cameraX - despawnThreshold)
            {
                Destroy(oldMap);
                spawnedMaps.Dequeue();
            }
            else
            {
                break;
            }
        }
    }
}
