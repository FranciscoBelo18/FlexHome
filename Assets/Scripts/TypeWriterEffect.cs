//faz um typewriter effect no texto
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    private float typingSpeed = 0.05f;
    private string fullText;

    void Start()
    {
        fullText = GetComponent<TextMeshProUGUI>().text;
        GetComponent<TextMeshProUGUI>().text = "";
        StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        foreach (char letter in fullText.ToCharArray())
        {
            GetComponent<TextMeshProUGUI>().text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
