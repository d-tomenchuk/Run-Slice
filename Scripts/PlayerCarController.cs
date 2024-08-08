using UnityEngine;
using System.Collections;


public class PlayerCarController : MonoBehaviour
{
    public GameObject playerCarPrefab;
    public GameController gameController;
    public GameObject playerCar;
    private Rigidbody playerCarRb;

    public float sensitivity = 1f;
    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private bool needToTurn = false;
    private bool isTurning = false;
    private bool isTurningLeft = false;
    private float initialYAngle;

    private CameraMove cameraMove;
    public float doubleClickThreshold = 0.3f;
    private float lastClickTime = 0f;

    private bool canRestartGame = false;
    

    void Start()
    {
        InitializePlayerCar();
        cameraMove = Camera.main.GetComponent<CameraMove>();
        if (cameraMove == null)
        {
            Debug.LogError("CameraMove component is not found on the main camera.");
            this.enabled = false;
        }
    }

    private void InitializePlayerCar()
    {   Vector3 cameraPosition = Camera.main.transform.position;

        playerCar = Instantiate(playerCarPrefab, new Vector3(cameraPosition.x - 4.5f, 0, 0), Quaternion.Euler(0, -90, 0));
        playerCar.transform.SetParent(transform);
        playerCarRb = playerCar.GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        HandleMouseInput();
        if (playerCar != null && playerCar.transform.localPosition.x < 28.5f)
        {
            MoveCar(new Vector3(28.5f, playerCar.transform.localPosition.y, playerCar.transform.localPosition.z), cameraMove.GetCurrentSpeed());
        }

        isTurningLeft = playerCar.transform.localPosition.z > 0;
    }

    void FixedUpdate()
    {
        if (needToTurn && !isTurning && Mathf.Abs(playerCarRb.angularVelocity.y) < 0.01f)
        {
            StartTurning();
        }

        if (isTurning)
        {
            ContinueTurning();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                needToTurn = true;
            }
            else
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            lastClickTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float newZ = Mathf.Clamp(playerCar.transform.localPosition.z + mouseDelta.x * sensitivity, -5.4f, 5.4f);
            MoveCar(new Vector3(playerCar.transform.localPosition.x, playerCar.transform.localPosition.y, newZ), cameraMove.GetCurrentSpeed());
            lastMousePosition = Input.mousePosition;
        }
    }

    private void StartTurning()
    {
        initialYAngle = playerCarRb.transform.eulerAngles.y;
        isTurning = true;
        cameraMove.BeginStopping();
    }

    private void ContinueTurning()
    {
        TurnCar();
        CheckIfTurnCompleted();
    }

    private void TurnCar()
    {
        float turnDirection = isTurningLeft ? -1f : 1f;
        float rotationSpeed = 270f * Mathf.Clamp(cameraMove.GetCurrentSpeed() / 10f, 0.5f, 2f);
        Quaternion deltaRotation = Quaternion.Euler(0f, rotationSpeed * turnDirection * Time.deltaTime, 0f);
        if (isTurningLeft)
            MoveCar(new Vector3(playerCar.transform.localPosition.x, playerCar.transform.localPosition.y, 5f), cameraMove.GetCurrentSpeed() * 0.4f);
        else
            MoveCar(new Vector3(playerCar.transform.localPosition.x, playerCar.transform.localPosition.y, -5f), cameraMove.GetCurrentSpeed() * 0.4f);
            
        playerCarRb.MoveRotation(playerCarRb.rotation * deltaRotation);
    }

    private void CheckIfTurnCompleted()
{
    float currentYAngle = playerCarRb.transform.eulerAngles.y;
    float angleDelta = Mathf.Abs(Mathf.DeltaAngle(initialYAngle, currentYAngle));

    if (angleDelta >= 169f && angleDelta < 191f)
    {
        if (isTurning)
        {
            isTurning = false;
            needToTurn = false;
            cameraMove.StopCamera();
            Debug.Log("Turn completed at " + currentYAngle + " degrees.");

            // Проверяем состояние флага canRestartGame
            if (canRestartGame)
            {
                StartCoroutine(DelayedRespawn(1f)); // Задержка в 1 секунду после завершения поворота
            }
            else
            {
                // Завершаем игру, если перезапуск не возможен
                
                gameController.GameOver();
                
            }
        }
    }
}

    IEnumerator DelayedRespawn(float delay)
    {
        yield return new WaitForSeconds(delay); // Ожидание заданной задержки
        RespawnPlayerCar(); // Вызов метода после задержки
    }

    public void MoveCar(Vector3 newPosition, float Speed)
    {
        Vector3 currentPosition = playerCar.transform.localPosition;
        playerCar.transform.localPosition = Vector3.MoveTowards(currentPosition, newPosition, Speed * Time.deltaTime);
    }

    public void RespawnPlayerCar()
    {
        Debug.Log("Respawn player car");
        playerCar.transform.SetParent(null);
        Destroy(playerCar, 3f);
        InitializePlayerCar();
        cameraMove.ResumeCamera();
    }
    public void SetCanRestartGame(bool canRestartGame) => this.canRestartGame = canRestartGame;

    public float getZPosition() => playerCar.transform.localPosition.z;
    
}
