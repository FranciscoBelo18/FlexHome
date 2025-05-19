using UnityEngine;
using System.Collections.Generic;


public class JointAnalysis : MonoBehaviour
{

    private ReadStretchingFile readStretchingFile;

    private SortedDictionary<int, int> resultado = new SortedDictionary<int, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        readStretchingFile = GetComponent<ReadStretchingFile>();

        if (readStretchingFile != null)
        {
            resultado = readStretchingFile.ReadFile("Squats");
            /*foreach (var kvp in resultado)
            {
                Debug.Log("Para os squats -> Joint: " + kvp.Key + ", Angle: " + kvp.Value);
            }*/
        }
        else
        {
            Debug.LogError("ReadStretchingFile não encontrado no GameObject.");
        }

        GetChildsJoint();
    }

    void GetChildsJoint()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            //Debug.Log("Child: " + child.name);
            string childName = child.name;
            string childJoint = childName.Substring(childName.IndexOf(' ') + 1);
            int childJointInt = int.Parse(childJoint);
            Debug.Log("Child: " + childName + ", Joint: " + childJointInt);
            foreach (var kvp in resultado)
            {
                if (childJointInt == kvp.Key)
                {
                    ChangeJointColor(child);
                }
            }
            
        }
    }

    void ChangeJointColor(Transform JointObject)
    {
        Debug.Log("Jointtttttttt: " + JointObject.name);
        JointObject.GetComponent<Renderer>().material.color = Color.yellow;
    }
}
