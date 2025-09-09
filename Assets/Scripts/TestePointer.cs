using UnityEngine;

public class TestePointer : MonoBehaviour
{
    public Transform circle;        // joint base
    public Transform desiredCircle; // joint previsto
    public GameObject arrowPrefab;  // prefab da seta

    private GameObject arrowInstance;

    void Start()
    {
        if (arrowPrefab != null && circle != null && desiredCircle != null)
        {
            // Instancia a seta na posição do círculo base
            arrowInstance = Instantiate(arrowPrefab, circle.position, Quaternion.identity, circle);

            // Ajusta a cor se quiseres
            var sr = arrowInstance.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.red;
                sr.sortingOrder = 1;
            }
        }
    }

    void Update()
    {
        if (arrowInstance == null || circle == null || desiredCircle == null) return;

        // Mantém a seta na posição do círculo base
        arrowInstance.transform.position = circle.position;

        // Calcula a direção para o target
        Vector3 dir = (desiredCircle.position - circle.position).normalized;

        // Calcula ângulo em graus
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Aplica rotação na seta
        arrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
