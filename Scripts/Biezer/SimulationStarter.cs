using UnityEngine;

public class SimulationStarter : MonoBehaviour
{
    public MapController mapController;

    void Update()
    {
        // Пример вызова StartSimulation при нажатии клавиши пробела
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mapController != null)
            {
                mapController.StartSimulation();
            }
            else
            {
                Debug.LogError("MapController is not assigned!");
            }
        }
    }
}
