using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class JointPrediction : MonoBehaviour
{
    private JointPointerManager jointPointerManager;

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

    public void PredictPosition(int jointToAnalyze, GameObject[] landmarkPoints, float angleTarget, Color color, bool feetOnTheGround)
    {
        if (landmarkPoints == null || jointToAnalyze >= landmarkPoints.Length) return;

        Dictionary<int, int> jointsPair = ApplicationVariables.JointPairCorrection;

        if (jointsPair != null && jointsPair.ContainsKey(jointToAnalyze))
        {
            int jointToPredict = jointsPair[jointToAnalyze];

            if (jointToPredict >= landmarkPoints.Length) return;

            float jointsDistance = CalculateDistance(landmarkPoints, jointToAnalyze, jointToPredict);

            CalculateDesiredPosition(landmarkPoints, jointToAnalyze, angleTarget, jointsDistance, jointToPredict, color, feetOnTheGround);
        }
    }

    private void CalculateDesiredPosition(GameObject[] landmarkPoints, int mainJoint, float angleTarget, float distance, int jointToPredict, Color color, bool feetOnTheGround)
    {
        if (landmarkPoints[mainJoint] == null || landmarkPoints[jointToPredict] == null) return;

        Vector3 mainJointPosition = landmarkPoints[mainJoint].transform.position;

        if (!ApplicationVariables.JointGroupsFromPlayfab.TryGetValue(mainJoint, out var neighbors)
            || neighbors == null || neighbors.Length < 2) return;

        int anchorJoint = (neighbors[0] == jointToPredict) ? neighbors[1] : neighbors[0];

        if (anchorJoint < 0 || anchorJoint >= landmarkPoints.Length) return;

        Vector3 anchorPosition = landmarkPoints[anchorJoint] != null
            ? landmarkPoints[anchorJoint].transform.position
            : mainJointPosition + Vector3.right; // fallback

        // direção base do anchor para o mainJoint
        Vector3 anchorDir = (anchorPosition - mainJointPosition).normalized;

        // 1) ângulo absoluto do anchorDir em graus
        float anchorAngleDeg = Mathf.Atan2(anchorDir.y, anchorDir.x) * Mathf.Rad2Deg;

        // 2) ângulo pretendido somando o angleTarget (em graus)
        float intendedAngleDeg = anchorAngleDeg + angleTarget;

        // 3) se for o lado esquerdo, espelhar o ângulo em relação ao eixo vertical do corpo
        //    geometricamente, espelho em X => angle' = 180 - angle
        float finalAngleDeg = ApplicationVariables.isLeftLegSide ? (180f - intendedAngleDeg) : intendedAngleDeg;

        // normalizar para intervalo [-180,180] (opcional, ajuda a manter valores limpos)
        finalAngleDeg = Mathf.Repeat(finalAngleDeg + 180f, 360f) - 180f;

        // 4) converter de volta para vetor direção e posição desejada
        float finalAngleRad = finalAngleDeg * Mathf.Deg2Rad;
        Vector3 desiredDir = new Vector3(Mathf.Cos(finalAngleRad), Mathf.Sin(finalAngleRad), 0f).normalized;
        Vector3 desiredPosition = mainJointPosition + desiredDir * distance;

        // debug (opcional) para ver a linha na Scene view
        Debug.DrawLine(mainJointPosition, desiredPosition, color, 0.1f);

        if (jointPointerManager != null)
        {
            jointPointerManager.CreatePointer(landmarkPoints[jointToPredict].transform, desiredPosition, color, feetOnTheGround);
        }
    }

    
    public void PredictPointerForStaticFeet(int jointToAnalyze, Color color, float angleDifferenceForPrediction, GameObject[] landmarkPoints, string Direction)
    {
        jointPointerManager.CreatePointerForStaticFeet(landmarkPoints[jointToAnalyze].transform, color, angleDifferenceForPrediction, Direction);
    }

    private float CalculateDistance(GameObject[] landmarkPoints, int jointA, int jointB)
    {
        Vector3 posA = landmarkPoints[jointA] != null ? landmarkPoints[jointA].transform.position : Vector3.zero;
        Vector3 posB = landmarkPoints[jointB] != null ? landmarkPoints[jointB].transform.position : Vector3.zero;
        return Vector3.Distance(posA, posB);
    }
}