using UnityEngine;

[ExecuteAlways]
public class BezierTest : MonoBehaviour
{
    public Transform pointsParent; // Родительский объект, содержащий точки P0, P1, P2, P3

    public void SetPointsParent(Transform newPointsParent)
    {
        pointsParent = newPointsParent;
        InitializePoints(); // Переинициализация точек
    }

    private void InitializePoints()
    {
        if (pointsParent != null && pointsParent.childCount >= 4)
        {
            Debug.Log($"Initialized Bezier curve with points from: {pointsParent.name}");
        }
        else
        {
            Debug.LogError("Points parent is missing or does not have enough points", this);
        }
    }

    private void OnDrawGizmos()
    {
        if (pointsParent == null || pointsParent.childCount < 4)
        {
            return; // Если не хватает точек, ничего не рисуем
        }

        Vector3 prevPoint = pointsParent.GetChild(0).position;
        Gizmos.color = Color.red;
        int segmentsNumber = 20;

        for (int i = 1; i <= segmentsNumber; i++)
        {
            float t = i / (float)segmentsNumber;
            Vector3 point = Bezier.GetPoint(
                pointsParent.GetChild(0).position, 
                pointsParent.GetChild(1).position, 
                pointsParent.GetChild(2).position, 
                pointsParent.GetChild(3).position, t);
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}
