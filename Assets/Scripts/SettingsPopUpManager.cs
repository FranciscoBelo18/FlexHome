using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SettingsPopUpManager : MonoBehaviour
{
    public GameObject[] AudioSettingsObjects;

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

        foreach (var obj in AudioSettingsObjects)
        {
            string SettingGroup = obj.tag;

            var toggleComponent = obj.GetComponentInChildren<UnityEngine.UI.Toggle>();
            var labelText = obj.GetComponentInChildren<TMPro.TMP_Text>();

            if (result.Data != null && result.Data.ContainsKey(SettingGroup))
            {
                var resultJson = result.Data[SettingGroup].Value;

                Dictionary<string, bool> settings = JsonConvert.DeserializeObject<Dictionary<string, bool>>(resultJson);

                if (settings != null && settings.ContainsKey(labelText.text))
                {
                    bool isActivated = settings[labelText.text];
                    toggleComponent.isOn = isActivated;
                    Debug.Log("Configuração carregada: " + labelText.text + " = " + isActivated);
                }
                else
                {
                    Debug.LogWarning("Nenhuma configuração encontrada para o texto: " + labelText.text);
                }
            }
            else
            {
                Debug.LogWarning("Nenhum dado encontrado para a chave: " + SettingGroup);
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

        foreach (var obj in AudioSettingsObjects)
        {
            var toggleComponent = obj.GetComponentInChildren<UnityEngine.UI.Toggle>();
            var labelText = obj.GetComponentInChildren<TMPro.TMP_Text>();

            if (toggleComponent != null && labelText != null)
            {
                audioSettings[labelText.text] = toggleComponent.isOn;
                Debug.Log("Configuração salva: " + labelText.text + " = " + toggleComponent.isOn);
            }
        }

        string jsonSettings = JsonConvert.SerializeObject(audioSettings);
        
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { "AudioSettings", jsonSettings } }
        }, OnUpdateUserDataSuccess, OnUpdateUserDataError);
    }

    private void OnUpdateUserDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("User data updated successfully.");
    }

    private void OnUpdateUserDataError(PlayFabError error)
    {
        Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
    }
}

