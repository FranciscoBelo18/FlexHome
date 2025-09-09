using UnityEngine;
using System.Collections.Generic;

public class JointPointerManager : MonoBehaviour
{
    public GameObject arrowPrefab;
    private Dictionary<Transform, GameObject> activeArrows = new Dictionary<Transform, GameObject>();

    public void CreatePointer(Transform baseJoint, Vector3 targetPosition, Color color)
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning("Arrow prefab não atribuído no inspector!");
            return;
        }

        GameObject arrow;
        if (activeArrows.ContainsKey(baseJoint))
        {
            // já existe seta, atualizamos
            arrow = activeArrows[baseJoint];
        }
        else
        {
            // instanciamos nova seta
            arrow = Instantiate(arrowPrefab, baseJoint.position, Quaternion.identity, baseJoint);
            activeArrows[baseJoint] = arrow;
        }

        // Atualizar posição (junto ao joint base)
        arrow.transform.position = baseJoint.position;

        // Calcular direção para o target
        Vector3 dir = (targetPosition - baseJoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Aplicar rotação
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Atualizar cor
        var sr = arrow.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }

    public void ClearAllPointers()
    {
        foreach (var arrow in activeArrows.Values)
        {
            if (arrow != null) Destroy(arrow);
        }
        activeArrows.Clear();
    }
}
