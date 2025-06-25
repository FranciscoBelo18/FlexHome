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

    public void AddPointsFromGoal(int goalPoints)
    {
        ApplicationVariables.PointsEarned += goalPoints;
    }

    public int GetPointsEarned()
    {
        return ApplicationVariables.PointsEarned;
    }

}
