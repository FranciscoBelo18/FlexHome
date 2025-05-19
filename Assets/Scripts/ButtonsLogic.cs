using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonsLogic : MonoBehaviour
{

    public void PlayGame()
    {
        ApplicationVariables.SceneToLoad = "SelectVersionScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void Profile()
    {
        ApplicationVariables.SceneToLoad = "ProfileScene";
        SceneManager.LoadScene("LoadingScene"); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        ApplicationVariables.SceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScene");
    }

    public void SelectSimpleVersion()
    {
        ApplicationVariables.SceneToLoad = "GymScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void SelectAdvancedVersion()
    {
        ApplicationVariables.SceneToLoad = "GymScene";
        SceneManager.LoadScene("LoadingScene");
    }
}
