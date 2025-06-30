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

    void Start()
    {
        SetInitialTime();
        ResetTimerVisuals();
    }

    void OnEnable()
    {
        SetInitialTime();
        ResetTimerVisuals();
    }

    void Update()
    {
        /*if (isPaused)
        {
            Debug.LogWarning("Timer está pausado no update");
            return;
        }*/

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
            ApplicationVariables.ActualState = "ExerciseDemo";
        }
    }
    /*public void PauseTimer()
    {
        Debug.LogWarning("Timer pausado");
        isPaused = true;
    }

    public void ResumeTimer()
    {
        Debug.LogWarning("Timer retomado");
        isPaused = false;
    }*/

    public void ForceEndTimer()
    {
        Debug.LogWarning("Timer forçado a terminar");
        time = 0;
    }

    public float GetTime()
    {
        return time;
    }
}
