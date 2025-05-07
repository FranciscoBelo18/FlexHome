using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI loadingText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(ApplicationVariables.SceneToLoad);
        
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {
            loadingText.text = "Loading... " + (asyncLoad.progress * 100) + "%";
            yield return null;
        }
        loadingText.text = "Loading... 100%";
        yield return new WaitForSeconds(2);
        asyncLoad.allowSceneActivation = true;
    }
}
