using UnityEngine;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{

    public string TextoHorasAtuais;
    private GameObject textDisplay;

    void Start()
    {
        //procurar o game object com o nome vindo do parametro
        textDisplay = GameObject.Find(TextoHorasAtuais);
    }

    void Update()
    {
        string atualtime= System.DateTime.Now.ToString("HH:mm:ss");
        //Debug.Log(TextoHorasAtuais);
        //Debug.Log(atualtime);
        //Debug.Log(textDisplay);
        textDisplay.GetComponent<TextMeshPro>().text = atualtime;
    }
}
