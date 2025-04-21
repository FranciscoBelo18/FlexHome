using UnityEngine;
using System.Diagnostics;
using System.IO;

public class PythonRunner : MonoBehaviour
{
    private Process pythonProcess;

    void Start()
    {
        // Caminho para o Python instalado no meu pc
        string pythonPath = @"C:\Users\user\AppData\Local\Programs\Python\Python312\python.exe";

        // Caminho para o script Python para a pose
        string scriptPath = @"C:\Users\user\Documents\MediaPipe\pose.py";

        if (!File.Exists(pythonPath))
        {
            UnityEngine.Debug.LogError("Python não encontrado: " + pythonPath);
            return;
        }

        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError("Script Python não encontrado: " + scriptPath);
            return;
        }

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            pythonProcess = Process.Start(start);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Erro ao iniciar o script Python: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
            UnityEngine.Debug.Log("Processo Python terminado.");
        }
    }
}
