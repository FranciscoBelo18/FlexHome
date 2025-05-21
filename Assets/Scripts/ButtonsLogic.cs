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

    public void TypeOfExercisesSelected()
    {
        //ApplicationVariables.TypeOfExercises = type;
        ApplicationVariables.SceneToLoad = "GymScene";
        SceneManager.LoadScene("LoadingScene");
    } 
    
    public void CloseWarnig()
    {
        GameObject warning = GameObject.Find("PopUpWarning");
        if (warning != null)
        {
            warning.SetActive(false);
        }else
        {
            Debug.LogWarning("PopUpWarning GameObject not found.");
        }
    }
}
