using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<Transform> spawnPoints; // Список всех возможных точек спавна
    public List<TrafficLightController> trafficLights; // Список всех светофоров
    public CarSpawner carSpawner; // Ссылка на CarSpawner
    public BezierTest bezierTest; // Ссылка на BezierTest

    void Start()
    {
        // Убрал вызов SelectRandomCurveAndDraw из Start, чтобы он не запускался автоматически
    }

    public void StartSimulation()
    {
        SelectRandomCurveAndDraw();
    }

    private void SelectRandomCurveAndDraw()
    {
        if (carSpawner == null)
        {
            Debug.LogError("CarSpawner is not assigned!");
            return;
        }

        if (bezierTest == null)
        {
            Debug.LogError("BezierTest is not assigned!");
            return;
        }

        if (spawnPoints.Count != trafficLights.Count)
        {
            Debug.LogError("The number of spawn points and traffic lights must be the same!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Count);
        Transform selectedSpawnPoint = spawnPoints[randomIndex];

        if (selectedSpawnPoint == null || selectedSpawnPoint.childCount == 0)
        {
            Debug.LogError("Selected spawn point is null or has no children to form a curve!");
            return;
        }

        // Управление светофорами
        UpdateTrafficLights(randomIndex);

        int curveIndex = Random.Range(0, selectedSpawnPoint.childCount);
        Transform selectedCurve = selectedSpawnPoint.GetChild(curveIndex);

        Vector3[] bezierPoints = new Vector3[selectedCurve.childCount];
        for (int i = 0; i < selectedCurve.childCount; i++)
        {
            bezierPoints[i] = selectedCurve.GetChild(i).position;
        }

        // Установим pointsParent для BezierTest для отрисовки кривой
        bezierTest.SetPointsParent(selectedCurve);

        GameObject carInstance = carSpawner.SpawnCarWithPhysics(selectedSpawnPoint);
        if (carInstance != null)
        {
            CarController carController = carInstance.GetComponent<CarController>();
            if (carController != null)
            {
                carController.Initialize(bezierPoints, selectedSpawnPoint.position); // Теперь передаем точку спавна
            }
            else
            {
                Debug.LogError("CarController component not found on the spawned car!");
            }
        }
        else
        {
            Debug.LogError("Failed to spawn a car.");
        }
    }

    private void UpdateTrafficLights(int activeIndex)
    {
        for (int i = 0; i < trafficLights.Count; i++)
        {
            if (i == activeIndex)
            {
                trafficLights[i].TurnOn();
            }
            else
            {
                trafficLights[i].TurnOff();
            }
        }
    }
}
