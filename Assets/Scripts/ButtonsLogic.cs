using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;

public class ButtonsLogic : MonoBehaviour
{
    public GameObject WarningPopUp;
    public GameObject dropdown;
    public GameObject PopUpTutorialObject;
    public GameObject loginPanel;
    public GameObject registerPanel;
    private WriteLogsToFile writeLogsToFile;

    public void PlayGame()
    {
        ApplicationVariables.SceneToLoad = "SelectTypeOfExercises";
        SceneManager.LoadScene("LoadingScene");
    }

    public void Settings()
    {
        ApplicationVariables.SceneToLoad = "SettingsScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        ApplicationVariables.SceneToLoad = "MainMenu";
        ApplicationVariables.ActualState = "ExerciseDemo";
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitFromSession()
    {
        writeLogsToFile = GameObject.Find("LogsManager").GetComponent<WriteLogsToFile>();
        writeLogsToFile.WriteSessionQuitTimeToFile();
        writeLogsToFile.WriteSplitLineBetweenExercises();
        writeLogsToFile.FinishWriting();
        ApplicationVariables.SceneToLoad = "MainMenu";
        ApplicationVariables.ActualState = "ExerciseDemo";
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

    public void TypeOfExercisesSelected(GameObject clickedButton)
    {
        //Debug.Log("Tag do botao clicado: " + clickedButton.tag);
        if (dropdown != null)
        {
            TMP_Dropdown dropdownComponent = dropdown.GetComponent<TMP_Dropdown>();
            int selectedIndex = dropdownComponent.value;
            //Debug.Log("Selected index: " + selectedIndex);  
            string version = dropdownComponent.options[selectedIndex].text;
            //Debug.Log("Selected game version: " + version);

            if (version == "None" || version == "Standard" || version == "Dynamic" || version == "Merged")
            {
                switch (clickedButton.tag)
                {
                    case "Lower":
                        ApplicationVariables.GameVersion = version;
                        ApplicationVariables.TypeOfExercises = "LowerBody";
                        ApplicationVariables.SceneToLoad = "GymScene";
                        PopUpTutorialObject.SetActive(true);
                        Time.timeScale = 0f;
                        //SceneManager.LoadScene("LoadingScene");
                        break;
                    case "Upper":
                        ApplicationVariables.GameVersion = version;
                        ApplicationVariables.TypeOfExercises = "UpperBody";
                        ApplicationVariables.SceneToLoad = "GymScene";
                        PopUpTutorialObject.SetActive(true);
                        Time.timeScale = 0f;
                        //SceneManager.LoadScene("LoadingScene");
                        break;
                    case "Full":
                        ApplicationVariables.GameVersion = version;
                        ApplicationVariables.TypeOfExercises = "FullBody";
                        ApplicationVariables.SceneToLoad = "GymScene";
                        PopUpTutorialObject.SetActive(true);
                        Time.timeScale = 0f;
                        //SceneManager.LoadScene("LoadingScene");
                        break;
                    default:
                        Debug.Log("Unknown exercise type selected.");
                        break;
                }
            }
            else
            {
                if (WarningPopUp != null)
                {
                    Time.timeScale = 0f;
                    WarningPopUp.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("PopUpWarning GameObject not attached.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Dropdown GameObject not found.");
        }

    }

    public void CloseWarning()
    {
        if (WarningPopUp != null)
        {
            WarningPopUp.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            Debug.LogWarning("PopUpWarning GameObject not attached.");
        }
    }

    public void PopUpTutorial(GameObject clickedButton)
    {
        if (clickedButton.tag == "Tutorial")
        {
            ApplicationVariables.StartWithTutorial = true;
            PopUpTutorialObject.SetActive(false);
            Time.timeScale = 1f;
            SceneManager.LoadScene("LoadingScene");

        }
        else if (clickedButton.tag == "Play")
        {
            ApplicationVariables.StartWithTutorial = false;
            PopUpTutorialObject.SetActive(false);
            Time.timeScale = 1f;
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            Debug.Log("Clicked button non identified.");
        }
    }

    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        ApplicationVariables.SceneToLoad = "AuthenticationScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void RestartLevel()
    {
        ApplicationVariables.SceneToLoad = "GymScene";
        ApplicationVariables.ActualState = "ExerciseDemo";
        SceneManager.LoadScene("LoadingScene");
        writeLogsToFile.WriteRestartedSessionTimeToFile();
        writeLogsToFile.WriteSplitLineBetweenExercises();
        writeLogsToFile.FinishWriting();
    }
    
    public void ActivateLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ActivateRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
}
