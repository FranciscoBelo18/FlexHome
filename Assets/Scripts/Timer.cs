using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    private float time;
    public Image timerCircle;
    public TextMeshProUGUI timerText;
    private float initialTime;
    private bool isPaused = false;
    private AudioSource audioSource;
    public Image PauseIcon;
    public WriteJSONLogsToFile logger;

    void Start()
    {
        SetInitialTime();
        ResetTimerVisuals();
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        SetInitialTime();
        ResetTimerVisuals();
    }

    void Update()
    {
        if (!ApplicationVariables.StartWithTutorial)
        {
            if (isPaused)
            {
                return;
            }

            time -= Time.deltaTime;
            time = Mathf.Max(0, time);

            timerText.text = ((int)time).ToString();
            timerCircle.fillAmount = time / initialTime;

            if (time <= 0)
            {
                gameObject.SetActive(false);
                SwitchStateAndRestart();
                gameObject.SetActive(true);
            }
        }
    }

    private void SetInitialTime()
    {
        if (ApplicationVariables.ActualState == "ExerciseDemo")
        {
            time = ApplicationVariables.TimeToDisplayExerciseDemo;
        }
        else if (ApplicationVariables.ActualState == "Exercise")
        {
            time = ApplicationVariables.TimeLimitToCompleteExercise;
        }

        initialTime = time;
    }

    void ResetTimerVisuals()
    {
        timerText.text = ((int)initialTime).ToString();
        timerCircle.fillAmount = 1f;
    }

    void SwitchStateAndRestart()
    {
        if (ApplicationVariables.ActualState == "ExerciseDemo")
        {
            ApplicationVariables.ActualState = "Exercise";
        }
        else
        {
            logger.LogEvent("Exercise Time Ended");
            logger.ForceSave();
            ApplicationVariables.ActualState = "ExerciseDemo";  
        }
    }

    public void PauseTimer()
    {
        PauseIcon.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        isPaused = true;
        logger.LogEvent("Timer Paused");
    }

    public void ResumeTimer()
    {
        PauseIcon.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        isPaused = false;
        logger.LogEvent("Timer Resumed");
    }

    public void ForceEndTimer()
    {
        Debug.LogWarning("Timer forçado a terminar");
        time = 0;
    }

    public float GetTime()
    {
        return time;
    }

    public bool IsTimerPaused()
    {
        return isPaused;
    }

    public void TimerPressed()
    {
        if (isPaused)
        {
            ResumeTimer();
        }
        else
        {
            PauseTimer();
        }
    }
}
