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
    public TextMeshProUGUI gamePhaseText;
    public GameObject UserPoseDisplay;
    public GameObject DemoVideoDisplay;
    private string previousState = "";
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
        // Atualiza o texto do exercício se necessário
        if (exerciseText.text != "Active Exercise: " + ApplicationVariables.ActualExercise)
        {
            UpdateExerciseText();
        }

        // Verifica se houve mudança de estado
        if (previousState != ApplicationVariables.ActualState)
        {
            Debug.Log($"State changed from {previousState} to {ApplicationVariables.ActualState}");

            // Se mudou de Gameplay para ExerciseDemo
            if (previousState == "Exercise" && ApplicationVariables.ActualState == "ExerciseDemo")
            {
                UpdateActiveExercise();
                UpdateExerciseText();
            }

            UpdateGamePhaseText();
            previousState = ApplicationVariables.ActualState;
        }

        // Ativar/desativar displays com base no estado atual
        if (ApplicationVariables.ActualState == "ExerciseDemo")
        {
            UserPoseDisplay.SetActive(false);
            DemoVideoDisplay.SetActive(true);
        }
        else if (ApplicationVariables.ActualState == "Exercise")
        {
            UserPoseDisplay.SetActive(true);
            DemoVideoDisplay.SetActive(false);
        }
    }


    public class ExerciseData
    {
        public string name;
        public bool together;
    }

    public void GetExercises()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("Exercises"))
            {
                string exercisesJson = result.Data["Exercises"];
                Debug.Log("Exercises JSON: " + exercisesJson);

                var allExercises = JsonConvert.DeserializeObject<Dictionary<string, List<ExerciseData>>>(exercisesJson);

                string selectedType = ApplicationVariables.TypeOfExercises;

                if (allExercises.ContainsKey(selectedType))
                {

                    List<ExerciseData> selectedExercises = allExercises[selectedType];

                    selectedExercises = selectedExercises.OrderBy(x => Random.value).ToList();

                    ApplicationVariables.Exercises = selectedExercises.Select(e => e.name).ToArray();

                    ApplicationVariables.ActualState = "ExerciseDemo";
                    ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[0];

                    //exemplo de como ir buscar um exercicio neste caso com o ActualExercise
                    var teste = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);
                    if (teste != null)
                    {
                        Debug.Log("Current exercise: " + teste.name + ", bool do together: " + teste.together);
                    }

                    UpdateExerciseText();
                }
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
        Debug.Log("Current exercise index: " + currentExerciseIndex);
        if (currentExerciseIndex >= 0 && currentExerciseIndex < ApplicationVariables.Exercises.Length - 1)
        {
            ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[currentExerciseIndex + 1];
        }
        else
        {
            ApplicationVariables.isAllExercisesCompleted = true;
            //ApplicationVariables.ActualState = "FinishedAllExercises";
            Debug.Log("All exercises completed.");
            Time.timeScale = 0; // Pausa o jogo
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

    private void UpdateGamePhaseText()
    {
        if (gamePhaseText != null)
        {
            if(ApplicationVariables.ActualState == "ExerciseDemo")
            {
                gamePhaseText.text = "Exercise Demo";
            }
            else if(ApplicationVariables.ActualState == "Exercise")
            {
                gamePhaseText.text = "Gameplay";
            }
        }
        else
        {
            Debug.LogWarning("gamePhaseText is not assigned in the inspector.");
        }
    }
}
