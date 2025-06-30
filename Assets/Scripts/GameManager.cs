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
    public PointsSystem pointsSystem;
    public TextMeshPro totalPointsText;
    private int swapCounter = 0;
    public GameObject[] StrikeThroughLines;
    private Dictionary<string, int> ExerciseResults = new Dictionary<string, int>();
    public TextMeshProUGUI ExerciseResultsText;
    public TextMeshProUGUI TotalPointsText;
    private bool isWaitingForBaseReturn = false;
    public GameObject ScreenDisplay;
    public Timer timer;
    private bool isTimerPausedByOutOfBounds = false;
    public TextMeshProUGUI activeLegText;
    public GameObject activeLegObject;
    public LeaderboardManager leaderboardManager;
    public TextMeshProUGUI LeaderboardText;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Log("--------------------------------------------");
        Debug.Log("Type of exercises: " + ApplicationVariables.TypeOfExercises);
        Debug.Log("Game version: " + ApplicationVariables.GameVersion);
        Debug.Log("--------------------------------------------");
        ApplicationVariables.PointsEarned = 0;
        ApplicationVariables.RepsCompleted = 0;
        ApplicationVariables.isAllExercisesCompleted = false;
        GetExercises();
        jointAngleCalculation.GetJointsToCalculateAngles();
    }

    void Update()
    {
        if (totalPointsText != null)
        {
            totalPointsText.text = "Total Points: " + ApplicationVariables.PointsEarned;
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
                    StoreExerciseResults();
                    UpdateActiveExercise();
                    UpdateExerciseText();
                    ResetStrikeThroughLines();
                    ApplicationVariables.RepsCompleted = 0;
                    swapCounter = 0;
                }

                var currentExerciseData = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);
               
                if (currentExerciseData != null && !currentExerciseData.together)
                {
                    activeLegObject.SetActive(true);
                }

                UpdateGamePhaseText();
                previousState = ApplicationVariables.ActualState;
            }

            if (ApplicationVariables.ActualState == "ExerciseDemo")
            {
                UserPoseDisplay.SetActive(false);
                DemoVideoDisplay.SetActive(true);
                ApplicationVariables.RepsCompleted = 0;
                activeLegText.text = "Right Leg";
                activeLegObject.SetActive(false);
            }
            else if (ApplicationVariables.ActualState == "Exercise")
            {
                UserPoseDisplay.SetActive(true);
                DemoVideoDisplay.SetActive(false);
                StartCoroutine(CacheLandmarkPointsWhenReady());
                /*if (AreAllLandmarksInsideScreenDisplay())
                {
                    if (isTimerPausedByOutOfBounds)
                    {
                        isTimerPausedByOutOfBounds = false;
                        timer.ResumeTimer();
                    }
                    AnalyzePose();
                }
                else
                {
                    if (!isTimerPausedByOutOfBounds)
                    {
                        isTimerPausedByOutOfBounds = true;
                        timer.PauseTimer();
                    }
                }*/
                AnalyzePose();
            }
            CheckRepsCompleted();
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
                DisplayResultsOnPopup();
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
                var allExercises = JsonConvert.DeserializeObject<Dictionary<string, List<ExerciseData>>>(exercisesJson);
                string selectedType = ApplicationVariables.TypeOfExercises;

                if (allExercises.ContainsKey(selectedType))
                {
                    selectedExercises = allExercises[selectedType].OrderBy(x => Random.value).ToList();
                    ApplicationVariables.Exercises = selectedExercises.Select(e => e.name).ToArray();
                    ApplicationVariables.ActualState = "ExerciseDemo";
                    ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[0];

                    var teste = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);
                    if (teste != null)
                    {
                        GetExerciseJointsAndAnglesFromFile();
                    }
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
        if (currentExerciseIndex >= 0 && currentExerciseIndex < ApplicationVariables.Exercises.Length - 1)
        {
            ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[currentExerciseIndex + 1];
            GetExerciseJointsAndAnglesFromFile();
        }
        else
        {
            ApplicationVariables.isAllExercisesCompleted = true;
        }
    }

    private void UpdateExerciseText()
    {
        if (exerciseText != null)
        {
            exerciseText.text = "Active Exercise: " + ApplicationVariables.ActualExercise;
        }
    }

    private void UpdateGamePhaseText()
    {
        if (gamePhaseText != null)
        {
            gamePhaseText.text = ApplicationVariables.ActualState == "ExerciseDemo" ? "Exercise Demo" : "Gameplay";
        }
    }

    public void GetExerciseJointsAndAnglesFromFile()
    {
        JointAnglePair = readStretchingFile.ReadFile(ApplicationVariables.ActualExercise);
        activeExerciseJoints = JointAnglePair.Keys.ToArray();
    }

    IEnumerator CacheLandmarkPointsWhenReady()
    {
        while (GameObject.Find("Point List Annotation") == null || GameObject.Find("Point List Annotation").transform.childCount < 33)
        {
            yield return null;
        }

        landmarkListAnnotation = GameObject.Find("Point List Annotation");
        int count = landmarkListAnnotation.transform.childCount;
        landmarkPoints = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            landmarkPoints[i] = landmarkListAnnotation.transform.GetChild(i).gameObject;
        }
    }

    public void AnalyzePose()
    {
        foreach (var ExJoint in activeExerciseJoints)
        {
            foreach (var joints in ApplicationVariables.JointGroupsFromPlayfab)
            {
                if (joints.Key == ExJoint)
                {
                    float angle = jointAngleCalculation.CalculateAngle(joints.Value, landmarkPoints);
                    float angleTarget = JointAnglePair[ExJoint];
                    float angleDiff = Mathf.Abs(angle - angleTarget);

                    if (angleDiff <= ApplicationVariables.GoodPerformanceRange)
                        landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.green;
                    else if (angleDiff <= ApplicationVariables.AveragePerformanceRange)
                        landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.yellow;
                    else
                        landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }

        CheckAllJointsColor();
    }

    private void CheckAllJointsColor()
    {
        bool allGreen = activeExerciseJoints.All(joint => landmarkPoints[joint].GetComponent<Renderer>().material.color == Color.green);

        if (allGreen && !isWaitingForBaseReturn)
        {
            isWaitingForBaseReturn = true;

            audioSource.Play();

            var currentExerciseData = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);

            if (currentExerciseData != null)
            {
                if (currentExerciseData.together)
                {
                    StartCoroutine(CheckBasePositionCoroutine(currentExerciseData));
                }
                else
                {
                    SwapLegs();
                    ChangeActiveLegText();
                    isWaitingForBaseReturn = false; // ← apenas aqui para unilateral
                }
            }
        }
    }


    private void ChangeActiveLegText()
    {
        if (activeLegText != null)
        {
            if (activeLegText.text == "Left Leg")
            {
                activeLegText.text = "Right Leg";
            }
            else if (activeLegText.text == "Right Leg")
            {
                activeLegText.text = "Left Leg";
            }
        }
    }

    private IEnumerator CheckBasePositionCoroutine(ExerciseData currentExerciseData)
    {
        // Aguarda enquanto o jogador ainda está na pose (todos os pontos estão verdes)
        while (activeExerciseJoints.All(joint => landmarkPoints[joint].GetComponent<Renderer>().material.color == Color.green))
        {
            yield return null;
        }

        // Aguarda até que o jogador tenha retornado à base (nenhum ponto mais verde)
        while (!activeExerciseJoints.All(joint => landmarkPoints[joint].GetComponent<Renderer>().material.color != Color.green))
        {
            yield return null;
        }

        // Agora sim: pose concluída e retorno à base detectado
        pointsSystem.AddPointsRepCompleted();
        ApplicationVariables.RepsCompleted++;

        isWaitingForBaseReturn = false;
    }

    private void SwapLegs()
    {
        SortedDictionary<int, int> swapped = new SortedDictionary<int, int>();
        var keys = JointAnglePair.Keys.OrderBy(k => k).ToList();

        for (int i = 0; i < keys.Count - 1; i++)
        {
            int current = keys[i];
            if (current % 2 != 0)
            {
                int next = keys[i + 1];
                if (next == current + 1)
                {
                    swapped[next] = JointAnglePair[current];
                    swapped[current] = JointAnglePair[next];
                    i++;
                }
                else
                {
                    swapped[current] = JointAnglePair[current];
                }
            }
            else if (!swapped.ContainsKey(current))
            {
                swapped[current] = JointAnglePair[current];
            }
        }

        if (!swapped.ContainsKey(keys[^1]))
        {
            swapped[keys[^1]] = JointAnglePair[keys[^1]];
        }

        JointAnglePair = swapped;
        activeExerciseJoints = JointAnglePair.Keys.ToArray();

        swapCounter++;

        if (swapCounter % 2 == 0 && swapCounter > 0)
        {
            pointsSystem.AddPointsRepCompleted();
            ApplicationVariables.RepsCompleted++;
        }
    }

    private void CheckRepsCompleted()
    {
        if (ApplicationVariables.RepsCompleted >= ApplicationVariables.DesiredReps)
        {
            foreach (GameObject line in StrikeThroughLines)
            {
                if (line.tag == "HighestGoal" && !line.activeSelf)
                    line.SetActive(true);
            }
            pointsSystem.AddPointsExerciseCompleted();
            FinishExercise();
        }
        else if (ApplicationVariables.RepsCompleted >= ApplicationVariables.MediumRepsGoal)
        {
            foreach (GameObject line in StrikeThroughLines)
            {
                if (line.tag == "MediumGoal" && !line.activeSelf)
                {
                    pointsSystem.AddPointsFromGoal(ApplicationVariables.MediumGoalPoints);
                    line.SetActive(true);
                }
            }
        }
        else if (ApplicationVariables.RepsCompleted >= ApplicationVariables.LowestRepsGoal)
        {
            foreach (GameObject line in StrikeThroughLines)
            {
                if (line.tag == "LowestGoal" && !line.activeSelf)
                {
                    pointsSystem.AddPointsFromGoal(ApplicationVariables.LowestGoalPoints);
                    line.SetActive(true);
                }
            }
        }
    }

    private void FinishExercise()
    {
        float timeLeft = timer.GetTime();
        //converte para inteiro
        int timeInt = Mathf.FloorToInt(timeLeft);
        pointsSystem.AddPointsFromGoal(timeInt * 2);
        timer.ForceEndTimer();
    }

    public void ResetStrikeThroughLines()
    {
        foreach (GameObject line in StrikeThroughLines)
        {
            line.SetActive(false);
        }
    }

    private void StoreExerciseResults()
    {
        ExerciseResults[ApplicationVariables.ActualExercise] = ApplicationVariables.RepsCompleted;
    }

    private void DisplayResultsOnPopup()
    {
        var actualType = ApplicationVariables.TypeOfExercises;
        int index = 1;
        ExerciseResultsText.text = "";

        foreach (var result in ExerciseResults)
        {
            ExerciseResultsText.text += index + ") " + result.Key + ": " + result.Value + " reps\n";
            index++;
        }

        TotalPointsText.text = pointsSystem.GetPointsEarned().ToString();

        if (leaderboardManager != null)
        {
            if (pointsSystem.GetPointsEarned() > 0)
            {
                leaderboardManager.SendToLeaderboard(actualType, pointsSystem.GetPointsEarned());
            }

            LeaderboardText.text = "";

            // Espera 2 segundos antes de buscar
            StartCoroutine(WaitThenGetLeaderboard(actualType));
        }
    }

    private IEnumerator WaitThenGetLeaderboard(string leaderboardName)
    {
        yield return new WaitForSeconds(1f); // tempo para o PlayFab propagar a atualização

        leaderboardManager.GetLeaderboard(leaderboardName, () =>
        {
            StartCoroutine(DisplayLeaderboardResults());
        });
    }

    private IEnumerator DisplayLeaderboardResults()
    {
        yield return null;

        foreach (var entry in ApplicationVariables.LeaderboardResults)
        {
            LeaderboardText.text += entry.Position + "º " + entry.DisplayName + ": " + entry.Score + " points\n";
        }
    }

    private bool AreAllLandmarksInsideScreenDisplay()
    {
        if (ScreenDisplay == null || landmarkPoints == null || landmarkPoints.Length == 0)
            return false;

        RectTransform screenRect = ScreenDisplay.GetComponent<RectTransform>();
        if (screenRect == null)
        {
            Debug.LogWarning("ScreenDisplay does not have a RectTransform.");
            return false;
        }

        foreach (var point in landmarkPoints)
        {
            if (point == null) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(point.transform.position);

            if (screenPos.z < 0)
                return false;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                screenRect,
                screenPos,
                Camera.main,
                out localPoint
            );

            if (!screenRect.rect.Contains(localPoint))
                return false;
        }

        return true;
    }

}
