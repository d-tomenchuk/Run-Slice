using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        StartCoroutine(LoadScene("Game"));
        Debug.Log("Scene changed");
    }

    IEnumerator LoadScene(string sceneName)
    {
        float fadeTime = Camera.main.GetComponent<Fading>().BeginFade(1f);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneName);


    }
}
