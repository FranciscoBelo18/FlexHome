using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class JointPrediction : MonoBehaviour
{
    private JointPointerManager jointPointerManager;
    private Dictionary<int, Vector3> predictedPositions = new Dictionary<int, Vector3>();

    private void Awake()
    {
        jointPointerManager = FindFirstObjectByType<JointPointerManager>();
    }

    public void GetJointPairsToCorrectFromPlayfab()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("JointPairCorrection"))
            {
                string jointPairCorrectionJson = result.Data["JointPairCorrection"];
                var JointAnglePredictPair = JsonConvert.DeserializeObject<Dictionary<int, int>>(jointPairCorrectionJson);
                ApplicationVariables.JointPairCorrection = JointAnglePredictPair;
            }
            else
            {
                Debug.LogWarning("No JointPairCorrection key found in title data.");
            }
        },
        error =>
        {
            Debug.LogError("Error getting title data: " + error.GenerateErrorReport());
        });
    }

    public void ClearPredictions()
    {
        predictedPositions.Clear();
    }

    private Vector3 GetJointPosition(GameObject[] landmarkPoints, int jointId)
    {
        if (ApplicationVariables.HipOutOfRange && predictedPositions.ContainsKey(jointId))
            return predictedPositions[jointId];

        if (landmarkPoints[jointId] != null)
            return landmarkPoints[jointId].transform.position;

        return Vector3.zero;
    }

    public void PredictPosition(int jointToAnalyze, GameObject[] landmarkPoints, float angleTarget, Color color)
    {
        if (landmarkPoints == null || jointToAnalyze >= landmarkPoints.Length) return;

        Dictionary<int, int> jointsPair = ApplicationVariables.JointPairCorrection;

        if (jointsPair != null && jointsPair.ContainsKey(jointToAnalyze))
        {
            int jointToPredict = jointsPair[jointToAnalyze];
            if (jointToPredict >= landmarkPoints.Length) return;

            float jointsDistance = CalculateDistance(landmarkPoints, jointToAnalyze, jointToPredict);

            CalculateDesiredPosition(landmarkPoints, jointToAnalyze, angleTarget, jointsDistance, jointToPredict, color);
        }
    }

    private void CalculateDesiredPosition(GameObject[] landmarkPoints, int mainJoint, float angleTarget, float distance, int jointToPredict, Color color)
    {
        Vector3 mainJointPosition = GetJointPosition(landmarkPoints, mainJoint);

        if (!ApplicationVariables.JointGroupsFromPlayfab.TryGetValue(mainJoint, out var neighbors)
            || neighbors == null || neighbors.Length < 2) return;

        int anchorJoint = (neighbors[0] == jointToPredict) ? neighbors[1] : neighbors[0];
        Vector3 anchorPosition = GetJointPosition(landmarkPoints, anchorJoint);

        Vector3 anchorDir = (anchorPosition - mainJointPosition).normalized;
        Quaternion rotation = Quaternion.AngleAxis(angleTarget, Vector3.forward);
        Vector3 desiredDir = rotation * anchorDir;
        Vector3 desiredPosition = mainJointPosition + desiredDir * distance;

        predictedPositions[jointToPredict] = desiredPosition;

        if (jointPointerManager != null)
        {
            jointPointerManager.CreatePointer(landmarkPoints[jointToPredict].transform, desiredPosition, color);
        }
    }

    private float CalculateDistance(GameObject[] landmarkPoints, int jointA, int jointB)
    {
        Vector3 posA = GetJointPosition(landmarkPoints, jointA);
        Vector3 posB = GetJointPosition(landmarkPoints, jointB);
        return Vector3.Distance(posA, posB);
    }
}
