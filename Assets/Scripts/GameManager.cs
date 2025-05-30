using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI exerciseText;
    void Start()
    {
        Debug.Log("--------------------------------------------");
        Debug.Log("Type of exercises: " + ApplicationVariables.TypeOfExercises);
        Debug.Log("Game version: " + ApplicationVariables.GameVersion);
        Debug.Log("--------------------------------------------");
        GetExercises();
    }

    void Update()
    {
        if (exerciseText.text != ApplicationVariables.ActualExercise)
        {
            UpdateExerciseText();
        }
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

                ApplicationVariables.ActualState = "ExerciseDemo";
                ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[0];
                Debug.Log("Current exerciseweeeeeeeeeeeeeeeeeeeeeeeeee: " + ApplicationVariables.ActualExercise);
                UpdateExerciseText();
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
    
    public void UpdateActiveExercise()
    {
        var currentExerciseIndex = System.Array.IndexOf(ApplicationVariables.Exercises, ApplicationVariables.ActualExercise);
        if (currentExerciseIndex >= 0 && currentExerciseIndex < ApplicationVariables.Exercises.Length - 1)
        {
            ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[currentExerciseIndex + 1];
        }
        else
        {
            ApplicationVariables.isAllExercisesCompleted = true;
            //ApplicationVariables.ActualState = "FinishedAllExercises";
            Debug.Log("All exercises completed.");
        }
    }
    
    private void UpdateExerciseText()
    {
        if (exerciseText != null)
        {
            exerciseText.text = "Active Exercise: " + ApplicationVariables.ActualExercise;
        }
        else
        {
            Debug.LogWarning("exerciseText is not assigned in the inspector.");
        }
    }
}
