using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public CameraMove cameraMove;
    public PlayerCarController playerCarController;
    public GameController gameController;

    private bool hasHitBarrier = false;
    private bool isRestartingGame = false;
    private float restartCooldown = 2.0f;  // 2 секунды задержки перед возможностью нового перезапуска
    private float lastRestartTime = 0;
    private float sceneLoadTime = 0f;  // Время загрузки сцены
    private bool wasStopped = true;
    private bool isInNoSpawnZone = false;
    private float timeSinceRespawn = 0f;

    void Start()
    {
        ValidateGameComponents();
        sceneLoadTime = Time.time;  // Устанавливаем время загрузки сцены
    }

    void Update()
    {
        timeSinceRespawn += Time.deltaTime;

        if (cameraMove.GetCurrentSpeed() <= 0.01f)
        {
            if (!wasStopped)
            {
                Debug.Log("Машина остановилась");
            }
            wasStopped = true;
        }
        else if (wasStopped && cameraMove.GetCurrentSpeed() > 0.01f)
        {
            Debug.Log("Машина снова в движении");
            wasStopped = false;
        }

        if (isInNoSpawnZone && wasStopped)
        {
            gameController.GameOver();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (timeSinceRespawn < 1.0f) return;

        // Условие с задержкой после загрузки сцены
        if (collision.collider.CompareTag("Blinker") && !hasHitBarrier && !isRestartingGame && Time.time > lastRestartTime + restartCooldown && Time.time > sceneLoadTime + 2.0f)
        {
            gameController.GameOver();
        }

        if (collision.collider.CompareTag("Barrier") && !wasStopped && !isRestartingGame && Time.time > lastRestartTime + restartCooldown)
        {
            gameController.GameOver();
        }

        if ((playerCarController.getZPosition() > 3f || playerCarController.getZPosition() < -3f) && !hasHitBarrier && !isRestartingGame && Time.time > lastRestartTime + restartCooldown)
        {
            Debug.Log("Condition met: Restarting game.");
            lastRestartTime = Time.time;
            playerCarController.SetCanRestartGame(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NoSpawnZone"))
        {
            isInNoSpawnZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoSpawnZone"))
        {
            isInNoSpawnZone = false;
        }
    }

    private void ValidateGameComponents()
    {
        playerCarController = FindObjectOfType<PlayerCarController>();
        if (playerCarController == null)
        {
            Debug.LogError("PlayerCarController not found!");
        }

        cameraMove = Camera.main?.GetComponent<CameraMove>();
        if (cameraMove == null)
        {
            Debug.LogError("CameraMove not found!");
        }

        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found!");
        }
    }
}
