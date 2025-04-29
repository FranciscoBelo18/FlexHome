using System.IO;
using UnityEngine;

public class ReadStretchingFile : MonoBehaviour
{
    private string filePath = @"C:\Users\user\Documents\MediaPipe\AnglesDB.txt";

    void Start()
    {
        ReadFile();
    }

    void ReadFile()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath); // Lê o ficheiro linha a linha

            foreach (string line in lines)
            {
                if (line.StartsWith("Squats:"))
                {
                    Debug.Log("Só dos squats -> " + line);
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Ficheiro não encontrado: " + filePath);
        }
    }
}
