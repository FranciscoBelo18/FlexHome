//por estar como static vai estar acessivel quando o jogo estiver a executar independentemente das mudanças de cena
//apenas será resetado quando o jogo for fechado, e entao vai ajudar para ir guardando os dados temporariamente 
//para depois fazer o respetivo tratamento
using System.Collections.Generic;
using UnityEngine;

public static class ApplicationVariables
{
    public static string SceneToLoad = "";
    public static string userLoggedID = "";
    public static string userLoggedName = "";
    public static string TypeOfExercises = "";
    public static int PointsEarned = 0;
    public static string GameVersion = "";
    public static string[] Exercises = new string[0];
    public static string ActualExercise = "";
    public static bool isAllExercisesCompleted = false;
    public static string ActualState = "ExerciseDemo";
    public static float TimeLimitToCompleteExercise = 5;
    public static float TimeToDisplayExerciseDemo = 5;
    public static Dictionary<int, int[]> JointGroupsFromPlayfab = new Dictionary<int, int[]>();
    public static int GoodPerformanceRange = 20;
    public static int AveragePerformanceRange = 40;
    public static int BadPerformanceRange = 60;
    public static int BasePoseKneeAngle = 180;
    public static int RepsCompleted = 0;
    public static int DesiredReps = 10;
    
}
