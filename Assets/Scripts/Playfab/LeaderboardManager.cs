using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;

public class LeaderboardManager : MonoBehaviour
{
    public void SendToLeaderboard(string leaderboardName, int score, Action onSuccess = null)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    //Version = ApplicationVariables.GameVersion,
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
                onSuccess?.Invoke();
            });
    }

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

        onComplete?.Invoke();
    }
    private void OnGetLeaderboardError(PlayFabError error, Action onComplete)
    {
        Debug.LogError("Failed to retrieve leaderboard: " + error.GenerateErrorReport());
        onComplete?.Invoke();
    }
}
