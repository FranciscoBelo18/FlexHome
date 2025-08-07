using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;

public class JointPrediction : MonoBehaviour
{
    private GameObject idealMarker;
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

    public void PredictPosition(int JointToAnalyze, GameObject[] landmarkPoints, float angleTarget)
    {
        Dictionary<int, int> JointsPair = ApplicationVariables.JointPairCorrection;

        if (JointsPair.ContainsKey(JointToAnalyze))
        {
            int JointToPredict = JointsPair[JointToAnalyze];

            float JointsDistance = CalculateDistance(landmarkPoints, JointToAnalyze, JointToPredict);

            //pegar no angulo ideal e prever a pos da joint com base no angulo ideal e da distancia 
            
            CalculateDesiredPosition(landmarkPoints, JointToAnalyze, angleTarget, JointsDistance);
        }

    }

    //como ja tenho a distancia a que o ponto deve estar da joint principal, agora é preciso pegar na mainjoint, aplicar o target angle e a distancia para prever a posicao do ponto ideal
    private void CalculateDesiredPosition(GameObject[] landmarkPoints, int mainJoint, float angleTarget, float distance)
    {
        //a formula da logica de calculo está no bloco de notas com os passos a seguir

        Vector3 mainJointPosition = landmarkPoints[mainJoint].transform.position;

        float angleRad = angleTarget * Mathf.Deg2Rad;

        Vector3 desiredPosition = new Vector3(mainJointPosition.x + Mathf.Cos(angleRad) * distance, mainJointPosition.y + Mathf.Sin(angleRad) * distance, mainJointPosition.z);


        if (idealMarker == null)
        {
            idealMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            idealMarker.transform.localScale = Vector3.one * 0.02f;
            idealMarker.GetComponent<Renderer>().material.color = Color.blue;
        }

        idealMarker.transform.position = desiredPosition;
    }


    private float CalculateDistance(GameObject[] landmarkPoints, int jointA, int jointB)
    {
        Vector3 positionA = landmarkPoints[jointA].transform.position;
        Vector3 positionB = landmarkPoints[jointB].transform.position;

        float distance = Vector3.Distance(positionA, positionB);
        return distance;
    }
}
