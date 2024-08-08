using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f; // Скорость автомобиля
    public float turnSpeed = 120f; // Скорость поворота в градусах в секунду
    private Rigidbody rb;
    private Vector3[] bezierPoints; // Массив точек кривой Безье
    private Vector3 spawnPoint; // Точка спавна автомобиля
    private int currentSegment = 0;
    private bool moveToFirstBezierPoint = true;
    private bool moveAlongCurve = false;
    private bool moveStraight = false;
    private bool isStopped = false; // Флаг для остановки движения
    private float t = 0f; // Параметр t для интерполяции по кривой Безье

    public void Initialize(Vector3[] bezierPoints, Vector3 spawnPosition)
    {
        this.bezierPoints = bezierPoints;
        spawnPoint = spawnPosition; // Запоминаем точку спавна
        transform.position = spawnPosition; // Спавнимся в указанной точке спавна
        currentSegment = 0;
        t = 0f; // Инициализация параметра t
        isStopped = false; // Сбрасываем флаг остановки

        if (bezierPoints.Length > 0)
        {
            SmoothLookAt(bezierPoints[0]); // Направляемся к первой точке кривой
            moveToFirstBezierPoint = true;
            moveAlongCurve = false;
            moveStraight = false;
        }

        Debug.Log("Car initialized. Moving to first Bezier point.");
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isStopped) return; // Если машина остановлена, не выполнять дальнейшие действия

        if (moveToFirstBezierPoint)
        {
            MoveToFirstBezierPoint();
        }
        else if (moveAlongCurve)
        {
            MoveAlongBezierCurve();
        }
        else if (moveStraight)
        {
            MoveStraight();
        }
    }

    void MoveToFirstBezierPoint()
    {
        Vector3 targetPosition = bezierPoints[0];
        float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPosition.x, 0, targetPosition.z));
        Debug.Log("Moving to first Bezier point. Distance: " + distance);

        if (distance < 0.5f)
        {
            moveToFirstBezierPoint = false;
            moveAlongCurve = true; // Начинаем движение по кривой
            Debug.Log("Reached first Bezier point. Starting to move along the curve.");
        }
        else
        {
            MoveTowardsFirstPoint(targetPosition);
        }
    }

    void MoveTowardsFirstPoint(Vector3 targetPosition)
    {
        Vector3 flatTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z); // Игнорируем изменения по оси Y
        Vector3 direction = (flatTargetPosition - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime); // Плавное движение к первой точке
        SmoothLookAt(flatTargetPosition);
    }

    void MoveAlongBezierCurve()
    {
        if (currentSegment + 3 < bezierPoints.Length)
        {
            t += Time.deltaTime * speed / Vector3.Distance(bezierPoints[currentSegment], bezierPoints[currentSegment + 3]);
            if (t > 1.0f)
            {
                t = 0f;
                currentSegment += 3; // Переход к следующему сегменту кривой
                if (currentSegment >= bezierPoints.Length - 3)
                {
                    moveAlongCurve = false;
                    moveStraight = true; // Начать движение прямо после кривой
                    CorrectRotation(); // Корректируем угол вращения
                    Debug.Log("Finished moving along the curve. Starting to move straight.");
                    return;
                }
            }

            Vector3 nextPoint = Bezier.GetPoint(bezierPoints[currentSegment], bezierPoints[currentSegment + 1], bezierPoints[currentSegment + 2], bezierPoints[currentSegment + 3], t);
            MoveTowardsTarget(nextPoint);
        }
        else
        {
            moveAlongCurve = false;
            moveStraight = true;
            CorrectRotation(); // Корректируем угол вращения
            Debug.Log("Finished moving along the curve. Starting to move straight.");
        }
    }

    void MoveStraight()
    {
        Vector3 direction = transform.forward;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 flatTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z); // Игнорируем изменения по оси Y
        Vector3 direction = (flatTargetPosition - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime); // Плавное движение к целевой точке
        SmoothLookAt(flatTargetPosition);
    }

    void SmoothLookAt(Vector3 point)
    {
        Vector3 flatPoint = new Vector3(point.x, transform.position.y, point.z); // Игнорируем изменения по оси Y
        Vector3 direction = (flatPoint - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed)); // Плавное вращение
        }
    }

    void CorrectRotation()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = Mathf.Round(euler.y / 90) * 90; // Корректируем угол вращения до ближайшего целого значения 90 градусов
        transform.rotation = Quaternion.Euler(euler);
    }

    // Обработка столкновений
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isStopped = true; // Устанавливаем флаг остановки при столкновении с объектом, имеющим тег Player
            Debug.Log("Collision with Player detected. Car will stop physically.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isStopped = true; // Устанавливаем флаг остановки при попадании в триггер с тегом Player
            Debug.Log("Trigger with Player detected. Car will stop physically.");
        }
    }
}
