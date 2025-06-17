using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class JointAngleCalculation : MonoBehaviour
{
    public void GetJointsToCalculateAngles()
    {
        Dictionary<int, int[]> AllJointsFromPlayfab = new Dictionary<int, int[]>();
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("Angles"))
            {
                string jointsJson = result.Data["Angles"];
                Debug.Log("Joints JSON: " + jointsJson);

                // Primeiro ler como Dictionary<string, int[]>
                var tempDict = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(jointsJson);

                // Converter para Dictionary<int, int[]>
                foreach (var kvp in tempDict)
                {
                    int key = int.Parse(kvp.Key);
                    AllJointsFromPlayfab[key] = kvp.Value;
                }

                ApplicationVariables.JointGroupsFromPlayfab = AllJointsFromPlayfab;

                /*foreach (var kvp in AllJointsFromPlayfab)
                {
                    Debug.Log($"Joint {kvp.Key} needs the following joints to calculate its angle: [{string.Join(", ", kvp.Value)}]");
                }*/
            }
            else
            {
                Debug.LogWarning("No Angles key found in title data.");
            }


        },
        error =>
        {
            Debug.LogError("Error getting title data of angles: " + error.GenerateErrorReport());
        });
    }

    public float CalculateAngle(int[] jointsToCalculate, GameObject[] landmarkPoints)
    {
        if (jointsToCalculate.Length < 3)
        {
            Debug.LogError("Not enough joints to calculate angle.");
            return 0f;
        }

        Vector3 jointA = landmarkPoints[jointsToCalculate[0]].transform.position;
        Vector3 jointB = landmarkPoints[jointsToCalculate[1]].transform.position;
        Vector3 jointC = landmarkPoints[jointsToCalculate[2]].transform.position;

        Vector3 vectorAB = jointB - jointA;
        Vector3 vectorBC = jointC - jointB;

        float angle = Vector3.Angle(vectorAB, vectorBC);

        return angle;
    }
}
