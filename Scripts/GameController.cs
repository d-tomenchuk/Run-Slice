using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private CameraMove cameraMove;
    [SerializeField] private PlayerCarController playerCarController;

    void Start()
    {
        if (cameraTransform == null || playerCarController == null)
        {
            Debug.LogError("Essential component is missing!", this);
            enabled = false;
            return;
        }

        cameraMove = cameraTransform.GetComponent<CameraMove>();
        if (cameraMove == null)
        {
            Debug.LogError("CameraMove component is not found on the camera.", this);
            enabled = false;
            return;
        }
    }

    

    void Update()
    {
    
        transform.position += new Vector3(cameraMove.GetCurrentSpeed() * Time.deltaTime, 0, 0); 
         
    }

    
    public void GameOver()
    {
        if (cameraMove != null)
        {
            cameraMove.BeginStopping();
            playerCarController.SetCanRestartGame(false);
            StartCoroutine(LoadScene("MainMenu"));
            Debug.Log("Game over!");
        }
        
    }
    
    
    IEnumerator LoadScene(string sceneName)
    {
        float fadeTime = Camera.main.GetComponent<Fading>().BeginFade(1f);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneName);

    }

    
}