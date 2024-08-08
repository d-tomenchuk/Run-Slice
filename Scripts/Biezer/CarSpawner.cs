using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs; // Массив префабов автомобилей

    public GameObject SpawnCarWithPhysics(Transform spawnPoint)
    {
        if (carPrefabs.Length == 0)
        {
            Debug.LogError("No car prefabs are assigned!");
            return null;
        }

        int index = Random.Range(0, carPrefabs.Length);
        GameObject selectedCarPrefab = carPrefabs[index];
        GameObject carInstance = Instantiate(selectedCarPrefab, spawnPoint.position, spawnPoint.rotation);
        return carInstance;
    }
}
