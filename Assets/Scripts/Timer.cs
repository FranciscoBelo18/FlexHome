using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    private float time;
    public Image timerCircle;
    public TextMeshProUGUI timerText;
    private float initialTime;

    void Start()
    {
        time = ApplicationVariables.TimeToDisplayExerciseDemo;
        initialTime = time;
        timerText.text = ((int)initialTime).ToString();
        timerCircle.fillAmount = 1f;
    }


    void OnEnable()
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
        ResetTimerVisuals();
    }

    void Update()
    {
        time -= Time.deltaTime;
        time = Mathf.Max(0, time); // Evita valores negativos

        timerText.text = ((int)time).ToString();
        timerCircle.fillAmount = time / initialTime;

        if (time <= 0)
        {
            gameObject.SetActive(false);
            SwitchStateAndRestart();
            //desativar o timer
            
        }
    }

    void ResetTimerVisuals()
    {
        timerText.text = ((int)initialTime).ToString();
        timerCircle.fillAmount = 1f;
    }

    void SwitchStateAndRestart()
    {
        // Alterna o estado
        if (ApplicationVariables.ActualState == "ExerciseDemo")
        {
            ApplicationVariables.ActualState = "Exercise";
        }
        else
        {
            ApplicationVariables.ActualState = "ExerciseDemo";
        }
    }
}
