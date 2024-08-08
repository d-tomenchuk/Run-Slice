using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    // Включение всех дочерних объектов
    public void Start()
    {
        TurnOff();
    }
    public void TurnOn()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    // Выключение всех дочерних объектов
    public void TurnOff()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
