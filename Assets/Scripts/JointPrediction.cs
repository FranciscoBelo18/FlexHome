using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class JointPrediction : MonoBehaviour
{
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
    private void CalculateDesiredPosition(GameObject[] landmarkPoints, int MainJoint, float angleTarget, float distance)
    {
        Vector3 mainJointPosition = landmarkPoints[MainJoint].transform.position;

        // Calculate the desired position based on the angle and distance
        Vector3 desiredPosition = mainJointPosition + Quaternion.Euler(0, angleTarget, 0) * Vector3.forward * distance;

        //falta agora é criar um objeto (bola talvez) para representar a posicao ideal e ter algo a apontar (uma animação ou assim)
        
   
    }

    private float CalculateDistance(GameObject[] landmarkPoints, int jointA, int jointB)
    {
        Vector3 positionA = landmarkPoints[jointA].transform.position;
        Vector3 positionB = landmarkPoints[jointB].transform.position;

        float distance = Vector3.Distance(positionA, positionB);
        return distance;
    }
}
