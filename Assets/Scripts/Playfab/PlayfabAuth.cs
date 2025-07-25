using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class PlayfabAuth : MonoBehaviour
{
    public TMP_InputField usernameRegister;
    public TMP_InputField passwordRegister;
    public TMP_InputField repeatPasswordRegister;
    public TMP_InputField usernameLogin;
    public TMP_InputField passwordLogin;
    public TMP_Text validationText;
    public GameObject validationMessagePopup;
    private bool AuthSuccess = false;

    public void RegisterButton()
    {
        string user = usernameRegister.text;
        string pass = passwordRegister.text;
        string repeatPass = repeatPasswordRegister.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(repeatPass))
        {
            Debug.Log("Username or Password is empty");
            validationText.text = "Username or Password is empty!";
            validationMessagePopup.SetActive(true);
            Invoke(nameof(DisableValidationMessageObject), 2f);
            return;
        }
        else if (pass != repeatPass)
        {
            Debug.Log("Passwords do not match");
            validationText.text = "Passwords do not match!";
            validationMessagePopup.SetActive(true);
            Invoke(nameof(DisableValidationMessageObject), 2f);
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Username = user,
            Password = pass,
            RequireBothUsernameAndEmail = false,
            DisplayName = user
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        AuthSuccess = true;
        Debug.Log("Registration successful: " + result.PlayFabId);

        SaveDefaultUserSettings();

        PlayfabUserManager.GetUserDataFromID(result.PlayFabId);  // Se necessário
        ApplicationVariables.SceneToLoad = "MainMenu";

        validationText.text = "Registration successful!";
        validationMessagePopup.SetActive(true);

        Invoke(nameof(DisableValidationMessageObject), 2f);
    }

    private void OnRegisterError(PlayFabError error)
    {
        Debug.LogError("Registration failed: " + error.GenerateErrorReport());
        validationText.text = "Registration failed!";
        validationMessagePopup.SetActive(true);
        Invoke(nameof(DisableValidationMessageObject), 2f);
    }

    public void LoginButton()
    {
        string user = usernameLogin.text;
        string pass = passwordLogin.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            Debug.Log("Username or Password is empty");
            validationText.text = "Username or Password is empty!";
            validationMessagePopup.SetActive(true);
            Invoke(nameof(DisableValidationMessageObject), 2f);
            return;
        }

        var request = new LoginWithPlayFabRequest
        {
            Username = user,
            Password = pass
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        AuthSuccess = true;
        Debug.Log("Login successful: " + result.PlayFabId);

        PlayfabUserManager.GetUserDataFromID(result.PlayFabId);  // Se necessário
        ApplicationVariables.SceneToLoad = "MainMenu";
        

        validationText.text = "Login successful!";
        validationMessagePopup.SetActive(true);
        Invoke(nameof(DisableValidationMessageObject), 2f);
    }

    private void OnLoginError(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
        validationText.text = "Login failed! Username and/or password incorrect.";
        validationMessagePopup.SetActive(true);
        Invoke(nameof(DisableValidationMessageObject), 2f);
    }

    private Dictionary<string, string> DefaultSettings()
    {
        var audioSettings = new Dictionary<string, bool>
        {
            { "Background Music", true },
            { "Exercise Rep Completed", true },
            { "Clock Ticking", true }
        };

        string audioJson = JsonConvert.SerializeObject(audioSettings);

        return new Dictionary<string, string>
        {
            { "AudioSettings", audioJson }
        };
    }

    private void SaveDefaultUserSettings()
    {
        var request = new UpdateUserDataRequest
        {
            Data = DefaultSettings()
        };

        PlayFabClientAPI.UpdateUserData(request, OnUpdateUserDataSuccess, OnUpdateUserDataError);
    }

    private void OnUpdateUserDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Default user settings saved successfully.");
    }

    private void OnUpdateUserDataError(PlayFabError error)
    {
        Debug.LogError("Failed to save default user settings: " + error.GenerateErrorReport());
    }

    private void DisableValidationMessageObject()
    {
        validationText.text = "";
        validationMessagePopup.SetActive(false);

        if (AuthSuccess)
        {
            AuthSuccess = false;
            SceneManager.LoadScene("LoadingScene");
        }
    }
}
