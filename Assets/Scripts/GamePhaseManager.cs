using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GamePhaseManager : MonoBehaviour
{
    public TextMeshProUGUI exerciseText;
    void Start()
    {
        ApplicationVariables.ActualState = "ExerciseDemo";
        ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[0];
        Debug.Log("Current exerciseweeeeeeeeeeeeeeeeeeeeeeeeee: " + ApplicationVariables.ActualExercise);
        UpdateExerciseText();
    }

    void Update()
    {
        if(exerciseText.text != ApplicationVariables.ActualExercise)
        {
            UpdateExerciseText();
        }
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
