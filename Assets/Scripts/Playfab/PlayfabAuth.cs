using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayfabAuth : MonoBehaviour
{

    public TMP_InputField usernameRegister;
    public TMP_InputField passwordRegister;
    public TMP_InputField repeatPasswordRegister;
    public TMP_InputField usernameLogin;
    public TMP_InputField passwordLogin;

    public void RegisterButton()
    {
        string user = usernameRegister.text;
        string pass = passwordRegister.text;
        string repeatPass = repeatPasswordRegister.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(repeatPass))
        {
            Debug.Log("Username or Password is empty");
            return;
        }
        else if (pass != repeatPass)
        {
            Debug.Log("Passwords do not match");
            return;
        }

        Debug.Log("Registering user: " + user);
        Debug.Log("Password: " + pass);

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
        Debug.Log("Registration successful: " + result.PlayFabId);
        PlayfabUserManager.GetUserDataFromID(result.PlayFabId);
        ApplicationVariables.SceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScene");
    }

    private void OnRegisterError(PlayFabError error)
    {
        Debug.Log("Registration failed: " + error.GenerateErrorReport());
    }

    public void LoginButton()
    {
        string user = usernameLogin.text;
        string pass = passwordLogin.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            Debug.Log("Username or Password is empty");
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
        Debug.Log("Login successful: " + result.PlayFabId);
        PlayfabUserManager.GetUserDataFromID(result.PlayFabId);
        ApplicationVariables.SceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScene");
    }

    private void OnLoginError(PlayFabError error)
    {
        Debug.Log("Login failed: " + error.GenerateErrorReport());
    }
    

}
