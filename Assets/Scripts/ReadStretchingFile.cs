using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class ReadStretchingFile : MonoBehaviour
{
    private string filePath = @"C:\Users\user\Documents\MediaPipe\AnglesDB.txt";

    void Start()
    {
        /*SortedDictionary<int, int> result = ReadFile("Squats");
        foreach (KeyValuePair<int, int> kvp in result)
        {
            Debug.Log("Joint: " + kvp.Key + ", Angle: " + kvp.Value);
        }*/
    }

    public SortedDictionary<int, int> ReadFile(string ExerciseName)
    {
        SortedDictionary<int, int> JointAnglePair = new SortedDictionary<int, int>();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath); 

            foreach (string line in lines)
            {
                if (line.StartsWith(ExerciseName + ":"))
                {
                    string ExerciseValues = line.Substring(line.IndexOf(':') + 1).Trim();
                    Debug.Log("Só dos squats -> " + ExerciseValues);
                    string[] values = ExerciseValues.Split(' ');
                    foreach (string value in values) {
                        //Debug.Log("Valor: " + value);
                        string Angle = value.Substring(value.IndexOf('_') + 1);
                        string Joint = value.Substring(0, value.IndexOf('_'));
                        
                        int JointInt = int.Parse(Joint);
                        int AngleInt = int.Parse(Angle);

                        JointAnglePair.Add(JointInt, AngleInt);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Ficheiro não encontrado: " + filePath);
        }

        return JointAnglePair;
    }
}
