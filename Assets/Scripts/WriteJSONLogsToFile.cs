using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WriteJSONLogsToFile : MonoBehaviour
{
    private string folderName = "UserLogs";
    private string currentFilePath;
    private UserSessions root; 
    private UserSession currentSession;
    
    private void Awake()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public void StartNewSession()
    {
        string userID = ApplicationVariables.userLoggedName;
        currentFilePath = Path.Combine(Application.persistentDataPath, folderName, userID + ".json");
        Debug.Log("Log File Path: " + currentFilePath);

        LoadUserFile();

        string actualGameMode = ApplicationVariables.isDemoVersion ? "Demo" : "Normal";

        currentSession = new UserSession()
        {
            date = System.DateTime.Now.ToString("dd/MM/yyyy"),
            time = System.DateTime.Now.ToString("HH:mm:ss.fff"),  
            gameVersion = ApplicationVariables.GameVersion,
            gameMode = actualGameMode,
            exercise = ApplicationVariables.ActualExercise,
            events = new List<EventData>()
        };

        root.sessions.Add(currentSession);

        Save();
    }

    private void LoadUserFile()
    {
        if (File.Exists(currentFilePath))
        {
            try
            {
                string json = File.ReadAllText(currentFilePath);
                root = JsonConvert.DeserializeObject<UserSessions>(json);

                if (root == null) InitializeNewRoot();
            }
            catch
            {
                Debug.LogError("Erro ao ler JSON do User. A criar novo...");
                InitializeNewRoot();
            }
        }
        else
        {
            InitializeNewRoot();
        }
    }

    private void InitializeNewRoot()
    {
        root = new UserSessions(); 
        root.sessions = new List<UserSession>();
    }

    public void LogEvent(string message, int? rep = null, Dictionary<int, JointData> jointData = null)
    {
        if (currentSession == null)
        {
            Debug.LogError("Tentaste gravar um evento sem sessão iniciada!");
            return;
        }

        EventData evt = new EventData()
        {
            time = System.DateTime.Now.ToString("HH:mm:ss.fff"),
            rep = rep,
            jointData = jointData,
            message = message
        };

        currentSession.events.Add(evt);
    }

    public void ForceSave()
    {
        Save();
        Debug.Log("Logs gravados com sucesso.");
    }

    private void Save()
    {
        if (root == null) return;

        string json = JsonConvert.SerializeObject(root, Formatting.Indented);
        File.WriteAllText(currentFilePath, json);
    }
}