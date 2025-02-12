using UnityEngine;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{

    public TMP_Text textDisplay;

    void Start()
    {
        textDisplay = GetComponent<TMP_Text>();
    }

    void Update()
    {
        string atualtime= System.DateTime.Now.ToString("HH:mm:ss");
        Debug.Log(atualtime);
        Debug.Log(textDisplay);
    }
}
