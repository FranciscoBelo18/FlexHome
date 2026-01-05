using UnityEngine;
using System.Collections.Generic;

public class UserSession
{
    public string date;
    public string time;
    public string gameVersion;
    public string gameMode;
    public string exercise;
    public List<EventData> events = new();
}
