using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI exerciseText;
    public TextMeshProUGUI gamePhaseText;
    public GameObject UserPoseDisplay;
    public GameObject DemoVideoDisplay;
    private string previousState = "";
    public GameObject PopUpExercisesCompleted;
    public GameObject TimerObject;
    public GameObject exTextObj;
    public GameObject gamePhaseTextObj;
    public ReadStretchingFile readStretchingFile;
    private SortedDictionary<int, int> JointAnglePair;
    private GameObject landmarkListAnnotation;
    private GameObject[] landmarkPoints;
    public JointAngleCalculation jointAngleCalculation;
    private int[] activeExerciseJoints = new int[0];

    void Start()
    {
        Debug.Log("--------------------------------------------");
        Debug.Log("Type of exercises: " + ApplicationVariables.TypeOfExercises);
        Debug.Log("Game version: " + ApplicationVariables.GameVersion);
        Debug.Log("--------------------------------------------");
        GetExercises();
        jointAngleCalculation.GetJointsToCalculateAngles();
    }

    void Update()
    {
        if (!ApplicationVariables.isAllExercisesCompleted)
        {
            if (exerciseText.text != "Active Exercise: " + ApplicationVariables.ActualExercise)
            {
                UpdateExerciseText();
            }

            if (previousState != ApplicationVariables.ActualState)
            {
                if (previousState == "Exercise" && ApplicationVariables.ActualState == "ExerciseDemo")
                {
                    UpdateActiveExercise();
                    UpdateExerciseText();
                }

                UpdateGamePhaseText();
                previousState = ApplicationVariables.ActualState;
            }

            if (ApplicationVariables.ActualState == "ExerciseDemo")
            {
                UserPoseDisplay.SetActive(false);
                DemoVideoDisplay.SetActive(true);
            }
            else if (ApplicationVariables.ActualState == "Exercise")
            {
                UserPoseDisplay.SetActive(true);
                DemoVideoDisplay.SetActive(false);
                StartCoroutine(CacheLandmarkPointsWhenReady());
                AnalyzePose();
            }

            if (landmarkPoints != null)
            {
                var leftShoulder = landmarkPoints[12];
                if (leftShoulder.activeInHierarchy)
                {
                    leftShoulder.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else
        {
            if (PopUpExercisesCompleted != null && !PopUpExercisesCompleted.activeSelf)
            {
                TimerObject.SetActive(false);
                UserPoseDisplay.SetActive(false);
                DemoVideoDisplay.SetActive(false);
                gamePhaseTextObj.SetActive(false);
                exTextObj.SetActive(false);
                PopUpExercisesCompleted.SetActive(true);
            }
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
                        GetExerciseJointsAndAnglesFromFile();
                    }

                    //UpdateExerciseText();
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
        //Debug.Log("Current exercise index: " + currentExerciseIndex);
        if (currentExerciseIndex >= 0 && currentExerciseIndex < ApplicationVariables.Exercises.Length - 1)
        {
            ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[currentExerciseIndex + 1];
            GetExerciseJointsAndAnglesFromFile();
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

    private void UpdateGamePhaseText()
    {
        if (gamePhaseText != null)
        {
            if (ApplicationVariables.ActualState == "ExerciseDemo")
            {
                gamePhaseText.text = "Exercise Demo";
            }
            else if (ApplicationVariables.ActualState == "Exercise")
            {
                gamePhaseText.text = "Gameplay";
            }
        }
        else
        {
            Debug.LogWarning("gamePhaseText is not assigned in the inspector.");
        }
    }

    public void GetExerciseJointsAndAnglesFromFile()
    {
        //Debug.Log("Reading angles for exercise: " + ApplicationVariables.ActualExercise);
        JointAnglePair = readStretchingFile.ReadFile(ApplicationVariables.ActualExercise);
        //Debug.Log("Joint angles for exercise: " + ApplicationVariables.ActualExercise);
        /*foreach (KeyValuePair<int, int> kvp in JointAnglePair)
        {
            Debug.Log("Joint: " + kvp.Key + ", Angle: " + kvp.Value);
        }*/
        activeExerciseJoints = JointAnglePair.Keys.ToArray();
        //Debug.LogWarning("Active exercise joints: " + string.Join(", ", activeExerciseJoints));
        //jointAngleCalculation.CalculateAngle(activeExerciseJoints, landmarkPoints);
    }

    IEnumerator CacheLandmarkPointsWhenReady()
    {
        while (GameObject.Find("Point List Annotation") == null ||
            GameObject.Find("Point List Annotation").transform.childCount < 33)
        {
            yield return null; // espera até o objeto e os filhos existirem
        }

        landmarkListAnnotation = GameObject.Find("Point List Annotation");

        int count = landmarkListAnnotation.transform.childCount;
        landmarkPoints = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            landmarkPoints[i] = landmarkListAnnotation.transform.GetChild(i).gameObject;
        }

        //Debug.Log("Landmark points atualizados.");
    }
    
    public void AnalyzePose()
    {
        foreach (var ExJoint in activeExerciseJoints)
        {
            foreach (var joints in ApplicationVariables.JointGroupsFromPlayfab)
            {
                if (joints.Key == ExJoint)
                {
                    var jointsToCalculateAngle = joints.Value;
                    float angle = jointAngleCalculation.CalculateAngle(jointsToCalculateAngle, landmarkPoints);
                    foreach (var joint in JointAnglePair)
                    {
                        if (joint.Key == joints.Key)
                        {
                            //obter o valor absoluto da diferença entre o angulo calculado e o angulo do ficheiro
                            float angleDifference = Mathf.Abs(angle - joint.Value);
                            if (angleDifference <= ApplicationVariables.GoodPerformanceRange)
                            {
                                landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.green;
                            }
                            else if (angleDifference <= ApplicationVariables.AveragePerformanceRange)
                            {
                                landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.yellow;
                            }
                            else
                            {
                                landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                }
            }
        }
    }

}
