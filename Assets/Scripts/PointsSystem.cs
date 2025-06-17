using UnityEngine;

public class PointsSystem : MonoBehaviour
{

    public void AddPointsRepCompleted()
    {
        ApplicationVariables.PointsEarned += 10;
    }

    public void AddPointsExerciseCompleted()
    {
        ApplicationVariables.PointsEarned += 50;
    }

}
