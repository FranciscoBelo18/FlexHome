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

        // 1) Descobrir o joint âncora (o outro braço que define o ângulo no mainJoint)
        if (!ApplicationVariables.JointGroupsFromPlayfab.TryGetValue(mainJoint, out var neighbors) 
            || neighbors == null || neighbors.Length < 2)
        {
            Debug.LogWarning($"Não há vizinhos suficientes para calcular ângulo no joint {mainJoint}.");
            return;
        }

        // Pega o vizinho que NÃO é o jointToPredict
        int anchorJoint = (neighbors[0] == jointToPredict) ? neighbors[1] : neighbors[0];
        if (anchorJoint < 0 || anchorJoint >= landmarkPoints.Length || landmarkPoints[anchorJoint] == null)
        {
            Debug.LogWarning($"Joint âncora {anchorJoint} inválido.");
            return;
        }

        // 2) Vetor base: do mainJoint para o âncora
        Vector3 anchorDir = (landmarkPoints[anchorJoint].transform.position - mainJointPosition).normalized;

        // 3) Rotaciona este vetor pelo ângulo alvo (em XY)
        Quaternion rotation = Quaternion.AngleAxis(angleTarget, Vector3.forward);
        Vector3 desiredDir = rotation * anchorDir;

        // 4) Calcula a posição ideal mantendo o comprimento do osso
        Vector3 desiredPosition = mainJointPosition + desiredDir * distance;

        // 5) Criar marcador só uma vez
        if (!markers.ContainsKey(jointToPredict))
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.localScale = Vector3.one * 0.03f;
            marker.GetComponent<Renderer>().material.color = new Color(0f, 0.5f, 1f, 0.7f);

            LineRenderer line = marker.AddComponent<LineRenderer>();
            line.startWidth = 0.01f;
            line.endWidth = 0.005f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.cyan;
            line.endColor = Color.blue;

            markers[jointToPredict] = marker;
        }

        // 6) Atualizar marcador em tempo real
        GameObject idealMarker = markers[jointToPredict];
        idealMarker.transform.position = desiredPosition;

        // Atualiza linha: posição atual -> posição ideal
        LineRenderer lr = idealMarker.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, landmarkPoints[jointToPredict].transform.position);
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