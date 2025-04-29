using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoseTracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    public GameObject[] PosePoints;
    public GameObject PoseParent;
    public GameObject SpawnPose;
    private bool changed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;

        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);

        string[] points = data.Split(',');

        for (int i = 0; i < PosePoints.Length; i++)
        {
            /*float x = float.Parse(points[i * 3])/80-8;
            float y = float.Parse(points[i * 3 + 1])/80-5;
            float z = float.Parse(points[i * 3 + 2])/80;*/

            float x = 7 - float.Parse(points[i * 3]) / 1000;
            float y = float.Parse(points[i * 3 + 1]) / 1000;
            float z = float.Parse(points[i * 3 + 2]) / 1000;

            PosePoints[i].transform.localPosition = new Vector3(x, y, z);
        }
        if (!changed)
        {
            PoseParent.transform.position = SpawnPose.transform.position;
            changed = true;
        }
    }
}