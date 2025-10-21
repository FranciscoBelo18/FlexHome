using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string DisplayName;
        public int Position;
        public int Score;
    }

    // Guarda localmente o último leaderboard carregado
    public List<LeaderboardEntry> CurrentLeaderboard = new List<LeaderboardEntry>();

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
                Debug.Log($"[Leaderboard] Score {score} enviado com sucesso para {leaderboardName}");
                onSuccess?.Invoke();
            },
            error =>
            {
                Debug.LogError("[Leaderboard] Falha ao enviar: " + error.GenerateErrorReport());
                onSuccess?.Invoke();
            });
    }

    public void GetLeaderboardAroundPlayer(string leaderboardName, int maxResults = 5, Action<List<LeaderboardEntry>> onComplete = null)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = leaderboardName,
            MaxResultsCount = maxResults
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request,
            result =>
            {
                CurrentLeaderboard.Clear();

                foreach (var entry in result.Leaderboard)
                {
                    string displayName = string.IsNullOrEmpty(entry.DisplayName) ? "Unknown Player" : entry.DisplayName;

                    var newEntry = new LeaderboardEntry
                    {
                        DisplayName = displayName,
                        Position = entry.Position + 1,
                        Score = entry.StatValue
                    };

                    CurrentLeaderboard.Add(newEntry);
                }

                onComplete?.Invoke(CurrentLeaderboard);
            },
            error =>
            {
                Debug.LogError("[Leaderboard] Erro ao buscar leaderboard ao redor do jogador: " + error.GenerateErrorReport());
                onComplete?.Invoke(null);
            });
    }
}