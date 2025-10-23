using UnityEngine;

public class TestePointer : MonoBehaviour
{
    public Transform circle;        // joint base
    public Transform desiredCircle; // joint previsto
    public GameObject arrowPrefab;  // prefab da seta
    public float arrowScale = 0.45f; // tamanho fixo da seta
    public float offsetDistance = 0.1f; // afastamento do centro do círculo
    private GameObject arrowInstance;

    void Start()
    {
        if (arrowPrefab != null && circle != null && desiredCircle != null)
        {
            arrowInstance = Instantiate(arrowPrefab, circle.position, Quaternion.identity, circle);

            // aplica escala fixa
            arrowInstance.transform.localScale = Vector3.one * arrowScale;

            // cor opcional
            var sr = arrowInstance.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.red;
            }
        }
    }

    void Update()
    {
        if (arrowInstance == null || circle == null || desiredCircle == null) return;

        // calcula direção
        Vector3 dir = (desiredCircle.position - circle.position).normalized;

        // aplica offset para afastar a seta do círculo
        arrowInstance.transform.position = circle.position + dir * offsetDistance;

        // fixa escala independente do círculo
        arrowInstance.transform.localScale = Vector3.one * arrowScale;

        // aplica rotação para apontar para o target
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
