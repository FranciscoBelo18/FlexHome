using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ButtonsLogic : MonoBehaviour
{
    public GameObject WarningPopUp;
    public GameObject dropdown;

    public void PlayGame()
    {
        ApplicationVariables.SceneToLoad = "SelectTypeOfExercises";
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

            if (version == "Standard" || version == "Dynamic")
            {
                switch (clickedButton.tag)
                {
                    case "Lower":
                        ApplicationVariables.GameVersion = version;
                        ApplicationVariables.TypeOfExercises = "LowerBody";
                        ApplicationVariables.SceneToLoad = "GymScene";
                        SceneManager.LoadScene("LoadingScene");
                        break;
                    case "Upper":
                        ApplicationVariables.GameVersion = version;
                        ApplicationVariables.TypeOfExercises = "UpperBody";
                        ApplicationVariables.SceneToLoad = "GymScene";
                        SceneManager.LoadScene("LoadingScene");
                        break;
                    case "Full":
                        ApplicationVariables.GameVersion = version;
                        ApplicationVariables.TypeOfExercises = "FullBody";
                        ApplicationVariables.SceneToLoad = "GymScene";
                        SceneManager.LoadScene("LoadingScene");
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
        }else
        {
            Debug.LogWarning("PopUpWarning GameObject not attached.");
        }
    }
}
