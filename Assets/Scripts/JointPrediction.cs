using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;

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

    public void PredictPosition(int jointToAnalyze, GameObject[] landmarkPoints, float angleTarget, Color color)
    {
        if (landmarkPoints == null || jointToAnalyze >= landmarkPoints.Length || landmarkPoints[jointToAnalyze] == null)
        {
            Debug.LogWarning($"Joint {jointToAnalyze} não é válido para predição.");
            return;
        }

        Dictionary<int, int> jointsPair = ApplicationVariables.JointPairCorrection;

        if (jointsPair != null && jointsPair.ContainsKey(jointToAnalyze))
        {
            int jointToPredict = jointsPair[jointToAnalyze];

            if (jointToPredict >= landmarkPoints.Length || landmarkPoints[jointToPredict] == null)
            {
                Debug.LogWarning($"Joint {jointToPredict} não encontrado nos landmarkPoints.");
                return;
            }

            float jointsDistance = CalculateDistance(landmarkPoints, jointToAnalyze, jointToPredict);

            CalculateDesiredPosition(landmarkPoints, jointToAnalyze, angleTarget, jointsDistance, jointToPredict, color);
        }
    }

    private void CalculateDesiredPosition(GameObject[] landmarkPoints, int mainJoint, float angleTarget, float distance, int jointToPredict, Color color)
    {
        Vector3 mainJointPosition = landmarkPoints[mainJoint].transform.position;

        if (!ApplicationVariables.JointGroupsFromPlayfab.TryGetValue(mainJoint, out var neighbors) 
            || neighbors == null || neighbors.Length < 2)
        {
            Debug.LogWarning($"Não há vizinhos suficientes para calcular ângulo no joint {mainJoint}.");
            return;
        }

        int anchorJoint = (neighbors[0] == jointToPredict) ? neighbors[1] : neighbors[0];
        if (anchorJoint < 0 || anchorJoint >= landmarkPoints.Length || landmarkPoints[anchorJoint] == null)
        {
            Debug.LogWarning($"Joint âncora {anchorJoint} inválido.");
            return;
        }

        Vector3 anchorDir = (landmarkPoints[anchorJoint].transform.position - mainJointPosition).normalized;
        Quaternion rotation = Quaternion.AngleAxis(angleTarget, Vector3.forward);
        Vector3 desiredDir = rotation * anchorDir;
        Vector3 desiredPosition = mainJointPosition + desiredDir * distance;

        if (jointPointerManager != null)
        {
            jointPointerManager.CreatePointer(landmarkPoints[jointToPredict].transform, desiredPosition, color);
        }
    }

    private float CalculateDistance(GameObject[] landmarkPoints, int jointA, int jointB)
    {
        Vector3 positionA = landmarkPoints[jointA].transform.position;
        Vector3 positionB = landmarkPoints[jointB].transform.position;
        return Vector3.Distance(positionA, positionB);
    }
}
