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
    private List<ExerciseData> selectedExercises = new List<ExerciseData>();
    private bool reachedBasePosition = false;
    private int[] KneesJoints = new int[] { 25, 26 }; 
    public PointsSystem pointsSystem;
    public TextMeshPro totalPointsText;

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
        if (totalPointsText != null)
        {
            totalPointsText.text = "Total Points: " + ApplicationVariables.PointsEarned;
        }
        else
        {
            Debug.LogWarning("totalPointsText is not assigned in the inspector.");
        }

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

                    selectedExercises = allExercises[selectedType];

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
        CheckAllJointsColor();
    }

    private void CheckAllJointsColor()
    {
        bool allGreen = true;
        foreach (var joint in activeExerciseJoints)
        {
            if (landmarkPoints[joint].GetComponent<Renderer>().material.color != Color.green)
            {
                allGreen = false;
                break;
            }
        }

        if (allGreen)
        {
            ApplicationVariables.PointsEarned += 10;
            var currentExerciseData = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);
            if (currentExerciseData != null)
            {
                while(reachedBasePosition == false)
                {
                    foreach (var kneeJoint in KneesJoints)
                    {
                        float kneeAngle = jointAngleCalculation.CalculateAngle(ApplicationVariables.JointGroupsFromPlayfab[kneeJoint], landmarkPoints);
                        if (Mathf.Abs(kneeAngle - ApplicationVariables.BasePoseKneeAngle) <= ApplicationVariables.GoodPerformanceRange)
                        {
                            landmarkPoints[kneeJoint].GetComponent<Renderer>().material.color = Color.green;
                            reachedBasePosition = true;
                        }
                        else if (Mathf.Abs(kneeAngle - ApplicationVariables.BasePoseKneeAngle) <= ApplicationVariables.AveragePerformanceRange)
                        {
                            landmarkPoints[kneeJoint].GetComponent<Renderer>().material.color = Color.yellow;
                            reachedBasePosition = false;
                        }
                        else
                        {
                            landmarkPoints[kneeJoint].GetComponent<Renderer>().material.color = Color.red;
                            reachedBasePosition = false;
                        }
                    }
                }
                pointsSystem.AddPointsRepCompleted();
                ApplicationVariables.RepsCompleted++;
                //Debug.LogWarning("Current exercise: " + currentExerciseData.name + ", bool do together: " + currentExerciseData.together);
                if (!currentExerciseData.together)
                {
                    SwapLegs();
                }
            }
        }
    }

    private void SwapLegs()
    {
        SortedDictionary<int, int> swappedJointAnglePair = new SortedDictionary<int, int>();

        var keys = JointAnglePair.Keys.OrderBy(k => k).ToList();
        for (int i = 0; i < keys.Count - 1; i++)
        {
            int currentKey = keys[i];

            if (currentKey % 2 != 0) // se for ímpar
            {
                int nextKey = keys[i + 1];

                if (nextKey == currentKey + 1)
                {
                    swappedJointAnglePair[nextKey] = JointAnglePair[currentKey];
                    swappedJointAnglePair[currentKey] = JointAnglePair[nextKey];

                    i++;
                }
                else
                {
                    swappedJointAnglePair[currentKey] = JointAnglePair[currentKey];
                }
            }
            else if (!swappedJointAnglePair.ContainsKey(currentKey))
            {
                // adiciona os que não entraram na lógica de troca
                swappedJointAnglePair[currentKey] = JointAnglePair[currentKey];
            }
        }

        // caso o último item não tenha sido tratado
        if (!swappedJointAnglePair.ContainsKey(keys[^1]))
        {
            swappedJointAnglePair[keys[^1]] = JointAnglePair[keys[^1]];
        }

        // Atualiza o dicionário principal e os joints ativos
        JointAnglePair = swappedJointAnglePair;
        activeExerciseJoints = JointAnglePair.Keys.ToArray();
    }
}
