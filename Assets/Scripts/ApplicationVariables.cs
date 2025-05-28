//por estar como static vai estar acessivel quando o jogo estiver a executar independentemente das mudanças de cena
//apenas será resetado quando o jogo for fechado, e entao vai ajudar para ir guardando os dados temporariamente 
//para depois fazer o respetivo tratamento

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
}
