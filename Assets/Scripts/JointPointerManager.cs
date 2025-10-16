using UnityEngine;
using System.Collections.Generic;

public class JointPointerManager : MonoBehaviour
{
    public GameObject redArrowPrefab;
    public GameObject yellowArrowPrefab;

    // Armazena setas ativas associadas a cada joint
    private Dictionary<Transform, GameObject> activeArrows = new Dictionary<Transform, GameObject>();

    public void CreatePointer(Transform baseJoint, Vector3 targetPosition, Color color, bool feetOnTheGround)
    {
        if (baseJoint == null) return;

        // Escolhe o prefab correto
        GameObject desiredPrefab = null;
        if (color == Color.red)
        {
            desiredPrefab = redArrowPrefab;
        }
        else if (color == Color.yellow)
        {
            desiredPrefab = yellowArrowPrefab;
        }
        else
        {
            // Para cores que não têm prefab, destrói seta existente
            if (activeArrows.ContainsKey(baseJoint))
            {
                Destroy(activeArrows[baseJoint]);
                activeArrows.Remove(baseJoint);
            }
            return;
        }

        GameObject arrow;

        // Verifica se já existe uma seta
        if (activeArrows.TryGetValue(baseJoint, out arrow))
        {
            // Se o prefab não corresponder à cor desejada, troca
            if (arrow == null || !IsSamePrefab(arrow, desiredPrefab))
            {
                Destroy(arrow);
                arrow = Instantiate(desiredPrefab, baseJoint.position, Quaternion.identity); // não parenta
                activeArrows[baseJoint] = arrow;
            }
        }
        else
        {
            arrow = Instantiate(desiredPrefab, baseJoint.position, Quaternion.identity); // não parenta
            activeArrows[baseJoint] = arrow;
        }

        // Atualiza posição da seta
        arrow.transform.position = baseJoint.position;

        // Calcula direção
        Vector3 dir;
        if (feetOnTheGround)
        {
            // Ignora o Y — só olha para a direção no eixo X
            Vector3 horizontalTarget = new Vector3(targetPosition.x, baseJoint.position.y, targetPosition.z);
            dir = (horizontalTarget - baseJoint.position).normalized;
        }
        else
        {
            dir = (targetPosition - baseJoint.position).normalized;
        }

        // Atualiza rotação
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private bool IsSamePrefab(GameObject arrow, GameObject prefab)
    {
        if (arrow == null || prefab == null) return false;
        return arrow.name.Replace("(Clone)", "") == prefab.name;
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