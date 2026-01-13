using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

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
    private SortedDictionary<int, int> basePositionJointAngles;
    private GameObject landmarkListAnnotation;
    private GameObject[] landmarkPoints;
    public JointAngleCalculation jointAngleCalculation;
    private int[] activeExerciseJoints = new int[0];
    private int[] basePositionJoints = new int[0];
    private List<ExerciseData> selectedExercises = new List<ExerciseData>();
    public PointsSystem pointsSystem;
    public TextMeshPro totalPointsText;
    private int swapCounter = 0;
    public GameObject[] StrikeThroughLines;
    private Dictionary<string, int> ExerciseResults = new Dictionary<string, int>();
    public TextMeshProUGUI ExerciseResultsText;
    public TextMeshProUGUI TotalPointsText;
    public Timer timer;
    public TextMeshProUGUI activeLegText;
    public GameObject activeLegObject;
    public LeaderboardManager leaderboardManager;
    public TextMeshProUGUI LeaderboardText;
    private AudioSource RepCompletedAudio;
    public GameObject SettingsButton;
    public GameObject SettingsPopUp;
    public VideoPlayer TutorialVideo;
    public GameObject PopUpWarningOutOfBounds;
    public AudioSource backgroundAudio;
    public Camera uiCamera;
    private bool isInitialized = false;
    public JointPrediction jointPrediction;
    public TextMeshPro BoardTitle;
    public JointPointerManager jointPointerManager;
    public VideoManager videoManager;
    private string leaderboardName;
    public GameObject ScreenDisplay;
    public WriteJSONLogsToFile writeJSONLogsToFile;
    public GameObject[] goals;
    public GameObject[] DisplayBoundaryObjects;

    void Start()
    {
        if (!ApplicationVariables.StartWithTutorial)
        {
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        RepCompletedAudio = GetComponent<AudioSource>();
        Debug.Log("--------------------------------------------");
        Debug.Log("Type of exercises: " + ApplicationVariables.TypeOfExercises);
        Debug.Log("Game version: " + ApplicationVariables.GameVersion);
        Debug.Log("--------------------------------------------");
        ApplicationVariables.PointsEarned = 0;
        ApplicationVariables.RepsCompleted = 0;
        ApplicationVariables.isAllExercisesCompleted = false;
        GetExercises();
        jointAngleCalculation.GetJointsToCalculateAngles();
        isInitialized = true;
        InitializeGoalsBoard();
        if (ApplicationVariables.GameVersion == "Dynamic" || ApplicationVariables.GameVersion == "Merged")
        {
            jointPrediction.GetJointPairsToCorrectFromPlayfab();
        }
        StartCoroutine(SendJointAngleToDB());
    }

    void Update()
    {
        if (!ApplicationVariables.StartWithTutorial)
        {
            if (!isInitialized)
            {
                InitializeGame();
            }

            AnalyzeSettings(RepCompletedAudio, backgroundAudio);

            foreach (var setting in ApplicationVariables.AudioSettings)
            {
                Debug.Log("Audio Setting - " + setting.Key + ": " + setting.Value);
            }


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
                        jointPointerManager.ClearAllPointers();
                    }
                    else if(previousState == "ExerciseDemo" && ApplicationVariables.ActualState == "Exercise")
                    {
                        //writeLogsToFile.WriteSplitLineBetweenExercises();
                        //writeLogsToFile.WriteHeaderToFile();
                        writeJSONLogsToFile.StartNewSession();
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
                    if (!AreEssentialPointsInsideRawImage(ScreenDisplay.GetComponent<RectTransform>(), landmarkPoints, uiCamera))
                    {
                        if (!timer.IsTimerPaused())
                        {
                            Debug.Log("Some essential points are out of bounds of the RawImage.");
                            PopUpWarningOutOfBounds.SetActive(true);
                            timer.PauseTimer();
                        }
                    }
                    else
                    {
                        PopUpWarningOutOfBounds.SetActive(false);
                        if (timer.IsTimerPaused())
                        {
                            timer.ResumeTimer();
                        }
                    }

                    if (!timer.IsTimerPaused())
                    {
                        AnalyzeBodyPositioning();
                        AnalyzePose();
                    }
                    else if (ApplicationVariables.GameVersion == "Dynamic" || ApplicationVariables.GameVersion == "Merged")
                    {
                        jointPointerManager.ClearAllPointers();
                    }
                }
                if (SettingsPopUp.activeSelf)
                {
                    jointPointerManager.ClearAllPointers();
                    Time.timeScale = 0f;
                    SettingsButton.SetActive(false);
                    if (ApplicationVariables.ActualState == "ExerciseDemo")
                    {
                        TutorialVideo.Pause();
                    }
                }
                else
                {
                    Time.timeScale = 1f;
                    SettingsButton.SetActive(true);
                    if (ApplicationVariables.ActualState == "ExerciseDemo")
                    {
                        TutorialVideo.Play();
                    }
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
                    SettingsButton.SetActive(false);
                    PopUpExercisesCompleted.SetActive(true);
                    DisplayResultsOnPopup();
                }
            }
        }
    }

    private void InitializeGoalsBoard()
    {
        if (ApplicationVariables.isDemoVersion)
        {
            foreach (var goal in goals)
            {
                if (goal.tag == "DemoGoals")
                {
                    goal.SetActive(true);
                }
                else if (goal.tag == "NormalGoals")
                {
                    goal.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var goal in goals)
            {
                if (goal.tag == "DemoGoals")
                {
                    goal.SetActive(false);
                }
                else if (goal.tag == "NormalGoals")
                {
                    goal.SetActive(true);
                }
            }
        }
    }

    public IEnumerator SendJointAngleToDB()
    {
        while (!ApplicationVariables.isAllExercisesCompleted)
        {
            if (ApplicationVariables.ActualState != "Exercise" || timer.IsTimerPaused())
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            Dictionary<int, JointData> jointDataDict = new Dictionary<int, JointData>();

            foreach (var ExJoint in activeExerciseJoints)
            {
                foreach (var joints in ApplicationVariables.JointGroupsFromPlayfab)
                {
                    if (joints.Key == ExJoint)
                    {
                        float angle = jointAngleCalculation.CalculateAngle(joints.Value, landmarkPoints);
                        float angleTarget = JointAnglePair[ExJoint];
                        Vector3 jointPosition = landmarkPoints[ExJoint].transform.localPosition;
                        
                        JointData jointData = new JointData()
                        {
                            jointX = jointPosition.x,
                            jointY = jointPosition.y,
                            actualAngle = angle,
                            desiredAngle = angleTarget
                        };

                        jointDataDict.Add(ExJoint, jointData);
                        
                        //writeLogsToFile.WriteRepDataToFile(ExJoint, angle, angleTarget);
                        //writeJSONLogsToFile.LogEvent(message: "Joint Angle Data", rep: ApplicationVariables.RepsCompleted + 1, joint: ExJoint, jointX: jointPosition.x, jointY: jointPosition.y, jointZ: jointPosition.z, actual: angle, desired: angleTarget);
                    }
                }
            }
            writeJSONLogsToFile.LogEvent(message: "Joint Angle Data", rep: ApplicationVariables.RepsCompleted + 1, jointData: jointDataDict);
            yield return new WaitForSeconds(1f);
        }
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
                        GetBasePositionJointsAndAnglesFromFile();
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
            //writeLogsToFile.WriteSplitLineBetweenExercises();
            //writeLogsToFile.FinishWriting();
            writeJSONLogsToFile.LogEvent(message: "All exercises completed");
        }
    }

    private void UpdateExerciseText()
    {
        if (exerciseText != null)
        {
            exerciseText.text = "Active Exercise: " + ApplicationVariables.ActualExercise;
            BoardTitle.text = "! " + ApplicationVariables.ActualExercise + " !";
            videoManager.VideoToPlay(ApplicationVariables.ActualExercise);
        }
    }

    private void UpdateGamePhaseText()
    {
        if (gamePhaseText != null)
        {
            if (ApplicationVariables.ActualState == "ExerciseDemo")
            {
                gamePhaseText.text = "Exercise Demo";
                gamePhaseTextObj.SetActive(true);
            }
            else
            {
                gamePhaseTextObj.SetActive(false);
            }
        }
    }

    public void GetExerciseJointsAndAnglesFromFile()
    {
        JointAnglePair = readStretchingFile.ReadFile(ApplicationVariables.ActualExercise);
        activeExerciseJoints = JointAnglePair.Keys.ToArray();
    }

    public void GetBasePositionJointsAndAnglesFromFile()
    {
        basePositionJointAngles = readStretchingFile.ReadFile("Normal Standing Posture");
        basePositionJoints = basePositionJointAngles.Keys.ToArray();
        foreach (var joint in basePositionJoints)
        {
            Debug.Log("Base Position Joint: " + joint + " Angle: " + basePositionJointAngles[joint]);
        }
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

    public void AnalyzeBodyPositioning()
    {
        Vector3 leftboundary = Vector3.zero;
        Vector3 rightboundary = Vector3.zero;
        foreach (var boundary in DisplayBoundaryObjects)
        {
            if (boundary.name.Contains("Left"))
            {
                leftboundary = boundary.transform.position;
            }
            else
            {
                rightboundary = boundary.transform.position;
            }
        }

        Vector3 NosePosition = landmarkPoints[0].transform.position;

        float DistanceToLeftBoundary = Mathf.Abs(NosePosition.x - leftboundary.x);
        float DistanceToRightBoundary = Mathf.Abs(NosePosition.x - rightboundary.x);

        if(DistanceToLeftBoundary >= DistanceToRightBoundary)
        {
            ApplicationVariables.PlayerPosition = "Right";
        }
        else
        {
            ApplicationVariables.PlayerPosition = "Left";
        }
    }

    public void AnalyzePose()
    {
        if (ApplicationVariables.GameVersion == "Standard")
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
                        {
                            landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.green;
                        }
                        else if (angleDiff <= ApplicationVariables.AveragePerformanceRange)
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
            CheckAllJointsColor();
        }
        else if (ApplicationVariables.GameVersion == "Dynamic" || ApplicationVariables.GameVersion == "Merged")
        {
            var currentExerciseData = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);

            foreach (var ExJoint in activeExerciseJoints)
            {
                foreach (var joints in ApplicationVariables.JointGroupsFromPlayfab)
                {
                    if (joints.Key == ExJoint)
                    {
                        float angle = jointAngleCalculation.CalculateAngle(joints.Value, landmarkPoints);
                        float angleTarget = JointAnglePair[ExJoint];
                        float angleDiff = Mathf.Abs(angle - angleTarget);
                        float angleDifferenceForPrediction = angle - angleTarget;

                        if (angleDiff <= ApplicationVariables.GoodPerformanceRange)
                        {
                            landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.green;
                        }
                        else if (angleDiff <= ApplicationVariables.AveragePerformanceRange)
                        {
                            landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.yellow;
                        }
                        else
                        {
                            landmarkPoints[ExJoint].GetComponent<Renderer>().material.color = Color.red;
                        }

                        if (currentExerciseData.StaticFeet && ApplicationVariables.ActualExercise == "Squats")
                        {
                            string Direction;
                            if (ExJoint == 24 || ExJoint == 23)
                            {
                                Direction = "Vertical";
                                jointPrediction.PredictPointerForStaticFeet(ExJoint, landmarkPoints[ExJoint].GetComponent<Renderer>().material.color, angleDifferenceForPrediction, landmarkPoints, Direction, null);
                            }
                        }
                        else if (ApplicationVariables.ActualExercise == "Lunges" || ApplicationVariables.ActualExercise == "Lateral Lunges")
                        {
                            string Direction;
                            if (ExJoint == 24 || ExJoint == 23)
                            {
                                Direction = "Vertical";
                            }
                            else
                            {
                                Direction = "Horizontal";
                            }
                            jointPrediction.PredictPointerForStaticFeet(ExJoint, landmarkPoints[ExJoint].GetComponent<Renderer>().material.color, angleDifferenceForPrediction, landmarkPoints, Direction, DisplayBoundaryObjects);
                        }
                        else
                        {
                            jointPrediction.PredictPosition(ExJoint, landmarkPoints, angleTarget, landmarkPoints[ExJoint].GetComponent<Renderer>().material.color, currentExerciseData.FeetOnTheGround);
                        }
                    }
                }
            }
            CheckAllJointsColor();
        }
        else if (ApplicationVariables.GameVersion == "None")
        {
            bool allJointsAreGood = true;

            foreach (var ExJoint in activeExerciseJoints)
            {
                foreach (var joints in ApplicationVariables.JointGroupsFromPlayfab)
                {
                    if (joints.Key == ExJoint)
                    {
                        float angle = jointAngleCalculation.CalculateAngle(joints.Value, landmarkPoints);
                        float angleTarget = JointAnglePair[ExJoint];
                        float angleDiff = Mathf.Abs(angle - angleTarget);
                        float angleDifferenceForPrediction = angle - angleTarget;

                        if (angleDiff > ApplicationVariables.GoodPerformanceRange)
                        {
                            allJointsAreGood = false;
                        }
                    }
                }
            }
            if (allJointsAreGood && !ApplicationVariables.isWaitingForBaseReturn)
            {
                StartCoroutine(ExerciseRepCompleted());
            }
        }
    }

    //para a versao sem feedback visual
    private IEnumerator ExerciseRepCompleted()
    {
        var currentExerciseData = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);

        ApplicationVariables.isWaitingForBaseReturn = true;
        StartCoroutine(PlaySoundWithBackground());

        while (!isInBasePosition())
        {
            yield return null;
        }

        //writeLogsToFile.WriteStartingPoseTimeToFile();
        writeJSONLogsToFile.LogEvent(message: "Returned to base position", rep: ApplicationVariables.RepsCompleted);

        if (currentExerciseData != null)
        {
            if (currentExerciseData.together)
            {
                pointsSystem.AddPointsRepCompleted();
                ApplicationVariables.RepsCompleted++;
                //writeLogsToFile.WriteStartingPoseTimeToFile();
                writeJSONLogsToFile.LogEvent(message: "Completed rep " + ApplicationVariables.RepsCompleted);
            }
            else
            {
                SwapLegs();
                ChangeActiveLegText();
            }
        }
        ApplicationVariables.isWaitingForBaseReturn = false;
    }

    private void CheckAllJointsColor()
    {
        bool allGreen = activeExerciseJoints.All(joint => landmarkPoints[joint].GetComponent<Renderer>().material.color == Color.green);

        if (allGreen && !ApplicationVariables.isWaitingForBaseReturn)
        {
            ApplicationVariables.isWaitingForBaseReturn = true;

            jointPointerManager.createPointers = false;

            jointPointerManager.ClearAllPointers();

            StartCoroutine(PlaySoundWithBackground());

            var currentExerciseData = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);

            if (currentExerciseData != null)
            {
                if (currentExerciseData.together)
                {
                    //writeLogsToFile.WriteRepTimeToFile();
                    writeJSONLogsToFile.LogEvent(message: "Reached Exercise Pose", rep: ApplicationVariables.RepsCompleted + 1);
                }
                else
                {
                    if(activeLegText.text == "Right Leg")
                    {
                        //writeLogsToFile.WriteLegPoseTimeToFile("Right");
                        writeJSONLogsToFile.LogEvent(message: "Reached Right Leg Pose", rep: ApplicationVariables.RepsCompleted + 1);
                    }
                    else
                    {
                        //writeLogsToFile.WriteLegPoseTimeToFile("Left");
                        writeJSONLogsToFile.LogEvent(message: "Reached Left Leg Pose", rep: ApplicationVariables.RepsCompleted + 1);
                    }
                }
                StartCoroutine(CheckBasePositionCoroutine(currentExerciseData));
            }
        }
    }

    private IEnumerator PlaySoundWithBackground()
    {
        if (!backgroundAudio.mute && !RepCompletedAudio.mute)
        {
            backgroundAudio.volume = 0.1f;

            RepCompletedAudio.Play();

            //o waitwhile espera até a codição da função ser falsa
            yield return new WaitWhile(() => RepCompletedAudio.isPlaying);

            backgroundAudio.volume = 1f;
        }
        else if (backgroundAudio.mute && !RepCompletedAudio.mute)
        {
            RepCompletedAudio.Play();
            yield return new WaitWhile(() => RepCompletedAudio.isPlaying);
        }
    }

    private void ChangeActiveLegText()
    {
        var currentExerciseData = selectedExercises.FirstOrDefault(e => e.name == ApplicationVariables.ActualExercise);
        if (activeLegText != null)
        {
            if (activeLegText.text == "Right Leg")
            {
                activeLegText.text = "Left Leg";
                if(currentExerciseData != null && currentExerciseData.LateralFrontFacing)
                {
                    ApplicationVariables.isLeftLegSide = true;
                }
            }
            else
            {
                activeLegText.text = "Right Leg";
                if (currentExerciseData != null && currentExerciseData.LateralFrontFacing)
                {
                    ApplicationVariables.isLeftLegSide = false;
                }
            }
        }
    }

    private IEnumerator CheckBasePositionCoroutine(ExerciseData currentExerciseData)
    {
        //para evitar que fique ali em loop no caso de o player ficar sempre na pose correta e nao ficar no loop de adiçao de pontos
        while (activeExerciseJoints.All(joint => landmarkPoints[joint].GetComponent<Renderer>().material.color == Color.green))
        {
            yield return null;
        }
        
        writeJSONLogsToFile.LogEvent(message: "Returning to base position", rep: ApplicationVariables.RepsCompleted);
        
        while (!isInBasePosition())
        {
            yield return null;
        }

        //writeLogsToFile.WriteStartingPoseTimeToFile();

        if (currentExerciseData.together)
        {
            pointsSystem.AddPointsRepCompleted();
            ApplicationVariables.RepsCompleted++;
            //writeLogsToFile.WriteStartingPoseTimeToFile();
            writeJSONLogsToFile.LogEvent(message: "Rep Completed", rep: ApplicationVariables.RepsCompleted);
        }
        else
        {
            SwapLegs();
            ChangeActiveLegText();
        }

        ApplicationVariables.isWaitingForBaseReturn = false;
        jointPointerManager.createPointers = true;
    }
    
    private bool isInBasePosition()
    {
        foreach (var baseJoint in basePositionJoints)
        {
            foreach (var joints in ApplicationVariables.JointGroupsFromPlayfab)
            {
                if (joints.Key == baseJoint)
                {
                    float angle = jointAngleCalculation.CalculateAngle(joints.Value, landmarkPoints);
                    float angleTarget = basePositionJointAngles[baseJoint];
                    float angleDiff = Mathf.Abs(angle - angleTarget);

                    if (angleDiff > ApplicationVariables.GoodPerformanceRange)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
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
        if ((ApplicationVariables.isDemoVersion == false && ApplicationVariables.RepsCompleted >= ApplicationVariables.DesiredReps) || (ApplicationVariables.isDemoVersion && ApplicationVariables.RepsCompleted >= ApplicationVariables.DemoDesiredReps))
        {
            foreach (GameObject line in StrikeThroughLines)
            {
                if (line.tag == "HighestGoal" && !line.activeSelf)
                {
                    line.SetActive(true);
                }
            }
            pointsSystem.AddPointsExerciseCompleted();
            FinishExercise();
        }
        else if ((ApplicationVariables.isDemoVersion == false && ApplicationVariables.RepsCompleted >= ApplicationVariables.MediumRepsGoal) || (ApplicationVariables.isDemoVersion && ApplicationVariables.RepsCompleted >= ApplicationVariables.DemoMediumRepsGoal))
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
        else if ((ApplicationVariables.isDemoVersion == false && ApplicationVariables.RepsCompleted >= ApplicationVariables.LowestRepsGoal) || (ApplicationVariables.isDemoVersion && ApplicationVariables.RepsCompleted >= ApplicationVariables.DemoLowestRepsGoal))
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
            var actualType = ApplicationVariables.TypeOfExercises;
            var actualVersion = ApplicationVariables.GameVersion;
            leaderboardName = actualType + actualVersion;

            int finalPoints = pointsSystem.GetPointsEarned();
            LeaderboardText.text = "Analysing your score...";

            leaderboardManager.SendToLeaderboard(leaderboardName, finalPoints, () =>
            {
                StartCoroutine(WaitAndGetLeaderboardAroundPlayer(leaderboardName));
            });
        }
    }
    
    private IEnumerator WaitAndGetLeaderboardAroundPlayer(string leaderboardName)
    {
        yield return new WaitForSeconds(2.5f); // dá tempo ao PlayFab

        leaderboardManager.GetLeaderboardAroundPlayer(leaderboardName, 5, (leaderboardEntries) =>
        {
            LeaderboardText.text = "";

            if (leaderboardEntries != null && leaderboardEntries.Count > 0)
            {
                foreach (var entry in leaderboardEntries)
                {
                    if (entry.DisplayName == ApplicationVariables.userLoggedName)
                        LeaderboardText.text += $"<b>{entry.Position}º {entry.DisplayName}: {entry.Score} pts </b>\n";
                    else
                        LeaderboardText.text += $"{entry.Position}º {entry.DisplayName}: {entry.Score} pts\n";
                }
            }
            else
            {
                LeaderboardText.text = "Something went wrong.";
            }
        });
    }



    private bool AreEssentialPointsInsideRawImage(RectTransform rawImageRect, GameObject[] landmarkPoints, Camera uiCamera)
    {
        int[] essentialPoints = { 0, 11, 12, 15, 16, 27, 28 };

        //se nao houver landmarkPoints
        if (landmarkPoints == null || landmarkPoints.Length == 0)
        {
            Debug.LogWarning("No landmark points available.");
            return false;
        }

        foreach (int index in essentialPoints)
        {
            if (index >= landmarkPoints.Length || landmarkPoints[index] == null)
            {
                Debug.LogWarning("Essential point " + index + " is not available.");
                return false;
            }

            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, landmarkPoints[index].transform.position);

            if (!IsPointInsideRawImage(rawImageRect, screenPos))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsPointInsideRawImage(RectTransform rect, Vector3 screenPos)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, uiCamera, out Vector2 localPoint) && rect.rect.Contains(localPoint);
    }

    private void AnalyzeSettings(AudioSource RepCompleted, AudioSource background)
    {
        foreach (var setting in ApplicationVariables.AudioSettings)
        {
            switch (setting.Key)
            {
                case "Background Music":
                    if (setting.Value) 
                    {
                        background.mute = false;

                        if (!background.isPlaying) 
                        {
                            background.Play();
                        }
                    }
                    else 
                    {
                        background.mute = true;
                    }
                    break;
                case "Exercise Rep Completed":
                    RepCompleted.mute = !setting.Value;
                    break;
                case "Clock Ticking":
                    //ainda por implementar este som, e se for necessário
                    break;
                default:
                    Debug.LogWarning("Unknown audio setting: " + setting.Key);
                    break;
            }
        }  
    }

}
