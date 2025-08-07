using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    private Animator tutorialAnimator;
    public GameObject popUpStartTutorial;
    public GameObject popUpTutorialCompleted;

    void Start()
    {
        if (ApplicationVariables.StartWithTutorial)
        {
            tutorialPanel.SetActive(true);
            popUpStartTutorial.SetActive(true);
            popUpTutorialCompleted.SetActive(false);
            tutorialAnimator = tutorialPanel.GetComponent<Animator>();
            tutorialAnimator.SetInteger("Change", 0);
        }
        else
        {
            tutorialPanel.SetActive(false);
            popUpStartTutorial.SetActive(false);
            popUpTutorialCompleted.SetActive(false);
        }
    }

    public int GetChangeValue()
    {
        int ChangeInt = tutorialAnimator.GetInteger("Change");
        return ChangeInt;
    }

    public void MoveToNextStep()
    {
        int currentChange = GetChangeValue();
        tutorialAnimator.SetInteger("Change", currentChange + 1);
        
        if (GetChangeValue() > 6)
        {
            popUpTutorialCompleted.SetActive(true);
        }
    }

    public void SkipTutorial()
    {
        tutorialPanel.SetActive(false);
        popUpStartTutorial.SetActive(false);
        ApplicationVariables.StartWithTutorial = false;
    }

    public void StartTutorial()
    {
        popUpStartTutorial.SetActive(false);
        tutorialAnimator.SetInteger("Change", 1);
    }

    public void FinishTutorial()
    {
        tutorialPanel.SetActive(false);
        ApplicationVariables.StartWithTutorial = false;
    }
}
