using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WriteJSONLogsToFile : MonoBehaviour
{
    private string filename = "MainTestingLogs.json";
    private string filePath;
    private UserSessions root;
    private UserSession currentSession;
    private string currentUser;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, filename);

        Debug.LogWarning("JSON log file path: " + filePath);

        Load();
    }

    private void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            try
            {
                root = JsonConvert.DeserializeObject<UserSessions>(json);

                if (root == null)
                    root = new UserSessions();
            }
            catch
            {
                Debug.LogError("Erro ao ler JSON. Criando novo ficheiro...");
                root = new UserSessions();
            }
        }
        else
        {
            root = new UserSessions();
        }
    }

    public void StartNewSession()
    {
        currentUser = ApplicationVariables.userLoggedName;

        if (!root.sessions.ContainsKey(currentUser))
            root.sessions[currentUser] = new List<UserSession>();

        string actualGameMode = ApplicationVariables.isDemoVersion ? "Demo" : "Normal";

        currentSession = new UserSession()
        {
            date = System.DateTime.Now.ToString("dd/MM/yyyy"),
            time = System.DateTime.Now.ToString("HH:mm:ss"),
            gameVersion = ApplicationVariables.GameVersion,
            gameMode = actualGameMode,
            exercise = ApplicationVariables.ActualExercise,
            events = new List<EventData>()
        };

        root.sessions[currentUser].Add(currentSession);

        Save();
    }

    public void LogEvent(string message, int? rep = null, int? joint = null, float? jointX = null, float? jointY = null, float? jointZ = null, float? actual = null, float? desired = null)
    {
        if (currentSession == null)
        {
            Debug.LogError("Tentaste gravar um evento sem sessão iniciada!");
            return;
        }

        EventData evt = new EventData()
        {
            time = System.DateTime.Now.ToString("HH:mm:ss"),
            rep = rep,
            joint = joint,
            jointX = jointX,
            jointY = jointY,
            jointZ = jointZ,
            actualAngle = actual,
            desiredAngle = desired,
            message = message
        };

        currentSession.events.Add(evt);
        Save();
    }

    private void Save()
    {
        string json = JsonConvert.SerializeObject(root, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
}