using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;

public class JointPrediction : MonoBehaviour
{
    // Dicionário para guardar marcadores de cada joint
    private Dictionary<int, GameObject> markers = new Dictionary<int, GameObject>();

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

    public void PredictPosition(int jointToAnalyze, GameObject[] landmarkPoints, float angleTarget)
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

            CalculateDesiredPosition(landmarkPoints, jointToAnalyze, angleTarget, jointsDistance, jointToPredict);
        }
    }

    private void CalculateDesiredPosition(GameObject[] landmarkPoints, int mainJoint, float angleTarget, float distance, int jointToPredict)
    {
        Vector3 mainJointPosition = landmarkPoints[mainJoint].transform.position;
        Vector3 currentDir = (landmarkPoints[jointToPredict].transform.position - mainJointPosition).normalized;

        // Rotaciona o vetor atual pelo ângulo desejado
        Quaternion rotation = Quaternion.AngleAxis(angleTarget, Vector3.forward);
        Vector3 desiredDir = rotation * currentDir;

        Vector3 desiredPosition = mainJointPosition + desiredDir * distance;

        // Cria marcador se ainda não existir para este joint
        if (!markers.ContainsKey(mainJoint))
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.localScale = Vector3.one * 0.03f;
            marker.GetComponent<Renderer>().material.color = new Color(0f, 0.5f, 1f, 0.7f); // Azul translúcido

            LineRenderer line = marker.AddComponent<LineRenderer>();
            line.startWidth = 0.01f;
            line.endWidth = 0.005f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.cyan;
            line.endColor = Color.blue;

            markers[mainJoint] = marker;
        }

        GameObject idealMarker = markers[mainJoint];
        idealMarker.transform.position = desiredPosition;

        // Atualiza linha de correção
        LineRenderer lr = idealMarker.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, mainJointPosition);
            lr.SetPosition(1, desiredPosition);
        }
    }

    private float CalculateDistance(GameObject[] landmarkPoints, int jointA, int jointB)
    {
        Vector3 positionA = landmarkPoints[jointA].transform.position;
        Vector3 positionB = landmarkPoints[jointB].transform.position;
        return Vector3.Distance(positionA, positionB);
    }
}