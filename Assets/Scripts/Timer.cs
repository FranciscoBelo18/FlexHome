using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
    }

    void Update()
    {
        time -= Time.deltaTime;
        timerText.text = "" + (int)time;
        timerCircle.fillAmount = time / initialTime;
        if (time <= 0)
        {
            time = 0;
            //ApplicationVariables.ActualState = "Exercise";
            //ApplicationVariables.ActualExercise = ApplicationVariables.Exercises[1];
           
        }
    }
    

}
