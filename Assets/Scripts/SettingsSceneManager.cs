using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SettingsSceneManager : MonoBehaviour
{
    public GameObject[] AudioSettingsToggles;
    public GameObject PopUpSaveSettings;

    void Start()
    {
        GetPlayerData();
    }

    private void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataSuccess, OnGetUserDataError);
    }

    private void OnGetUserDataSuccess(GetUserDataResult result)
    {
        Debug.Log("User data retrieved successfully.");
        foreach (var toggle in AudioSettingsToggles)
        {
            string toggleTag = toggle.tag;
            if (result.Data != null && result.Data.ContainsKey(toggleTag))
            {
                var resultJson = result.Data[toggleTag].Value;
                var settings = JsonConvert.DeserializeObject<Dictionary<string, bool>>(resultJson);

                string toggleText = toggle.GetComponentInChildren<TMPro.TMP_Text>().text;

                if (settings != null && settings.ContainsKey(toggleText))
                {
                    bool isActivated = settings[toggleText];
                    toggle.GetComponent<UnityEngine.UI.Toggle>().isOn = isActivated;
                    Debug.Log("Setting retrieved from player data -> " + toggleText + ": " + isActivated);
                }
                else
                {
                    Debug.Log("No settings found for toggle with text: " + toggleText);
                }
            }
        }
    }

    private void OnGetUserDataError(PlayFabError error)
    {
        Debug.LogError("Failed to retrieve user data: " + error.GenerateErrorReport());
    }

    public void SavePlayerData()
    {
        Dictionary<string, bool> audioSettings = new Dictionary<string, bool>();

        foreach (var toggle in AudioSettingsToggles)
        {
            string toggleText = toggle.GetComponentInChildren<TMPro.TMP_Text>().text;
            bool isOn = toggle.GetComponent<UnityEngine.UI.Toggle>().isOn;
            audioSettings[toggleText] = isOn;
        }

        ApplicationVariables.AudioSettings = audioSettings;

        Dictionary<string, string> data = new Dictionary<string, string>
        {
            { "AudioSettings", JsonConvert.SerializeObject(audioSettings) }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = data
        }, OnUpdateUserDataSuccess, OnUpdateUserDataError);
    }

    private void OnUpdateUserDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("User data updated successfully.");
        PopUpSaveSettings.SetActive(true);
        Invoke("ClosePopUp", 2f); 
    }

    private void OnUpdateUserDataError(PlayFabError error)
    {
        Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
    }

    private void ClosePopUp()
    {
        PopUpSaveSettings.SetActive(false);
    }
}
