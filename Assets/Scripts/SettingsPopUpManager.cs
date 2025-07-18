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
            string toggleTag = obj.tag;

            var toggleComponent = obj.GetComponentInChildren<UnityEngine.UI.Toggle>();
            var labelText = obj.GetComponentInChildren<TMPro.TMP_Text>();

            if (result.Data != null && result.Data.ContainsKey(toggleTag))
            {
                var resultJson = result.Data[toggleTag].Value;

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
                Debug.LogWarning("Nenhum dado encontrado para a chave: " + toggleTag);
            }
        }
    }


    private void OnGetUserDataError(PlayFabError error)
    {
        Debug.LogError("Failed to retrieve user data: " + error.GenerateErrorReport());
    }
}

