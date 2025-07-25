using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class ExercisesManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("--------------------------------------------");
        Debug.Log("Type of exercises: " + ApplicationVariables.TypeOfExercises);
        Debug.Log("Game version: " + ApplicationVariables.GameVersion);
        Debug.Log("--------------------------------------------");
        GetExercises();
    }

    public void GetExercises()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("Exercises"))
            {
                string exercisesJson = result.Data["Exercises"];
                Debug.Log("Exercises JSON: " + exercisesJson);

                var exercises = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(exercisesJson);

                string selectedType = ApplicationVariables.TypeOfExercises;

                // Exemplo: acessar exercícios de membros inferiores
                foreach (var ex in exercises[selectedType])
                {
                    Debug.Log(selectedType + " : " + ex);
                    //criar um novo array para guardar os exercícios
                    ApplicationVariables.Exercises = exercises[selectedType].ToArray();
                }
                //Randomizar a ordem dos exercícios mo array
                ApplicationVariables.Exercises = ApplicationVariables.Exercises.OrderBy(x => Random.value).ToArray();
                Debug.Log("Exercises after changing its position randomly: " + string.Join(", ", ApplicationVariables.Exercises));
            }
            else
            {
                Debug.LogWarning("No Exercises key found in title data.");
            }
        },
        error =>
        {
            Debug.LogError("Error getting title data: " + error.GenerateErrorReport());
        });
    }
}
