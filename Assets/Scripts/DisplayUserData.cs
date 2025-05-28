using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class DisplayUserData : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    // Start is called before the first frame update
    void Start()
    {
        string username = ApplicationVariables.userLoggedName;
        Debug.Log("Username retrieved: " + username);
        if (titleText != null)
        {
            titleText.text = "Let's make it worth it, " + username + "!";
        }
        else
        {
            Debug.LogError("Username text field not assigned in the inspector.");
        }
    }
}
