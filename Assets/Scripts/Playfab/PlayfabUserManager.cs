using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public static class PlayfabUserManager
{
    public static void GetUserDataFromID(string id)
    {
        var request = new GetAccountInfoRequest
        {
            PlayFabId = id
        };

        PlayFabClientAPI.GetAccountInfo(request, OnGetUserDataSuccess, OnGetUserDataError);
    }

    private static void OnGetUserDataSuccess(GetAccountInfoResult result)
    {
        Debug.Log("User data retrieved successfully in the Main Menu scene: " + result.AccountInfo.Username);
        ApplicationVariables.userLoggedName = result.AccountInfo.Username;
    }

    private static void OnGetUserDataError(PlayFabError error)
    {
        Debug.Log("Failed to retrieve user data: " + error.GenerateErrorReport());
    }
}
