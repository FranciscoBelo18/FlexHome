using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    //buscar player data do playfab
    private void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataSuccess, OnGetUserDataError);
    }

    private void OnGetUserDataSuccess(GetUserDataResult result)
    {
        Debug.Log("User data retrieved successfully.");

    }

    private void OnGetUserDataError(PlayFabError error)
    {
        Debug.LogError("Failed to retrieve user data: " + error.GenerateErrorReport());
    }

    //guardar player data no playfab
    private void SavePlayerData(Dictionary<string, string> data)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = data
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
