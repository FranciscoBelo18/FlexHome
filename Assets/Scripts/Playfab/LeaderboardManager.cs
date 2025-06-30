using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;

public class LeaderboardManager : MonoBehaviour
{
    public class LeaderboardDataStruct
    {
        public string DisplayName;
        public int Position;
        public int Score;
    }

    public void SendToLeaderboard(string leaderboardName, int score, Action onSuccess = null)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result =>
            {
                Debug.Log("Leaderboard updated successfully.");
                onSuccess?.Invoke();
            },
            error =>
            {
                Debug.LogError("Failed to update leaderboard: " + error.GenerateErrorReport());
                onSuccess?.Invoke(); // mesmo se falhar, pode seguir
            });
    }


    // ADAPTADO: Agora aceita um callback opcional
    public void GetLeaderboard(string leaderboardName, Action onComplete = null)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            MaxResultsCount = ApplicationVariables.maxResultsToDisplayLeaderboard
        };

        PlayFabClientAPI.GetLeaderboard(request,
            result => OnGetLeaderboardSuccess(result, onComplete),
            error => OnGetLeaderboardError(error, onComplete)
        );
    }

    // ADAPTADO: Agora aceita callback
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result, Action onComplete)
    {
        Debug.Log("Leaderboard retrieved successfully.");

        ApplicationVariables.LeaderboardResults.Clear();

        foreach (var entry in result.Leaderboard)
        {
            var displayName = string.IsNullOrEmpty(entry.DisplayName) ? "Unknown" : entry.DisplayName;

            Debug.Log("Leaderboard Entry: " + displayName + " - Position: " + (entry.Position + 1) + " - Score: " + entry.StatValue);

            ApplicationVariables.LeaderboardResults.Add(new ApplicationVariables.LeaderboardEntry
            {
                DisplayName = displayName,
                Position = entry.Position + 1, // Começa do 1
                Score = entry.StatValue
            });
        }

        onComplete?.Invoke(); // Chama callback se existir
    }

    // ADAPTADO: Também chama callback mesmo em erro
    private void OnGetLeaderboardError(PlayFabError error, Action onComplete)
    {
        Debug.LogError("Failed to retrieve leaderboard: " + error.GenerateErrorReport());
        onComplete?.Invoke();
    }
}
