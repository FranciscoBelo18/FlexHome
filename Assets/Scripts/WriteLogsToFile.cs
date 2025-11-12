using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class WriteLogsToFile : MonoBehaviour
{
    private string filename = "TestingLogs.txt";
    private StreamWriter writer;

    void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        writer = new StreamWriter(path, true);
        Debug.LogWarning("Log file path: " + path);
    }

    public void WriteHeaderToFile()
    {
        string userID = ApplicationVariables.userLoggedName;
        string gameVersion = ApplicationVariables.GameVersion;
        string exerciseName = ApplicationVariables.ActualExercise;
        string header = System.DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss") + " | " + userID + " | " + gameVersion + " | " + exerciseName;
        writer.WriteLine(header);
        writer.Flush(); 
    }

    public void WriteRepDataToFile(int jointID, float ActualAngle, float DesiredAngle)
    {
        string logEntry = "Rep " + (ApplicationVariables.RepsCompleted + 1) + ": Joint " + jointID + " | Actual Angle " + ActualAngle.ToString() + " | Desired Angle " + DesiredAngle.ToString();
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void WriteRepTimeToFile()
    {
        string logEntry = "Rep " + (ApplicationVariables.RepsCompleted + 1) + " pose reached | " + System.DateTime.Now.ToString("HH:mm:ss");
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void WriteLegPoseTimeToFile(string legSide)
    {
        string logEntry = "Rep " + (ApplicationVariables.RepsCompleted + 1) + ": " + legSide + " leg pose reached | " + System.DateTime.Now.ToString("HH:mm:ss");
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void WriteStartingPoseTimeToFile()
    {
        string logEntry = "Rep " + (ApplicationVariables.RepsCompleted + 1) + ": starting pose reached | " + System.DateTime.Now.ToString("HH:mm:ss");
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void WritePausedTimeToFile()
    {
        string logEntry = "Game paused at: " + System.DateTime.Now.ToString("HH:mm:ss");
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void WriteResumedTimeToFile()
    {
        string logEntry = "Game resumed at: " + System.DateTime.Now.ToString("HH:mm:ss");
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void WriteRestartedSessionTimeToFile()
    {
        string logEntry = "Game restarted at: " + System.DateTime.Now.ToString("HH:mm:ss");
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void WriteSessionQuitTimeToFile()
    {
        string logEntry = "Left during game at: " + System.DateTime.Now.ToString("HH:mm:ss");
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void WriteSplitLineBetweenExercises()
    {
        string logEntry = "----------------------------------------------------------------";
        writer.WriteLine(logEntry);
        writer.Flush();
    }

    public void FinishWriting()
    {
        writer.Flush();
        writer.Close();
        writer = null;
    }

}
