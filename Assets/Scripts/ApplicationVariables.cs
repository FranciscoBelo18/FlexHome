//por estar como static vai estar acessivel quando o jogo estiver a executar independentemente das mudanças de cena
//apenas será resetado quando o jogo for fechado, e entao vai ajudar para ir guardando os dados temporariamente 
//para depois fazer o respetivo tratamento
using System.Collections.Generic;
using UnityEngine;

public static class ApplicationVariables
{
    public static string SceneToLoad = "";
    public static bool StartWithTutorial = false;
    public static string userLoggedName = "";
    public static string TypeOfExercises = "";
    public static int PointsEarned = 0;
    public static string GameVersion = "";
    public static string[] Exercises = new string[0];
    public static string ActualExercise = "";
    public static bool isAllExercisesCompleted = false;
    public static string ActualState = "ExerciseDemo";
    public static float TimeLimitToCompleteExercise = 120;
    public static float TimeToDisplayExerciseDemo = 5;
    public static Dictionary<int, int[]> JointGroupsFromPlayfab = new Dictionary<int, int[]>();
    public static int GoodPerformanceRange = 35;
    public static int AveragePerformanceRange = 70;
    public static int BadPerformanceRange = 100;
    public static int RepsCompleted = 0;
    public static int DesiredReps = 12;
    public static int LowestGoalPoints = 50;
    public static int MediumGoalPoints = 75;
    public static int HighestGoalPoints = 150;
    public static int LowestRepsGoal = 8;
    public static int MediumRepsGoal = 10;
    public static int maxResultsToDisplayLeaderboard = 5;
    public class LeaderboardEntry
    {
        public string DisplayName;
        public int Position;
        public int Score;
    }
    public static List<LeaderboardEntry> LeaderboardResults = new List<LeaderboardEntry>();
    public static Dictionary<string, bool> AudioSettings = new Dictionary<string, bool>();
    public static Dictionary<int, int> JointPairCorrection = new Dictionary<int, int>();
}
