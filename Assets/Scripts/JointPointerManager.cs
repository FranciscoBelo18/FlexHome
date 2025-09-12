using UnityEngine;
using System.Collections.Generic;

public class JointPointerManager : MonoBehaviour
{
    public GameObject redArrowPrefab;
    public GameObject yellowArrowPrefab;

    private Dictionary<Transform, GameObject> activeArrows = new Dictionary<Transform, GameObject>();

    public void CreatePointer(Transform baseJoint, Vector3 targetPosition, Color color)
    {
        if (baseJoint == null || targetPosition == null || color == null){
            return;
        }

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
            if (activeArrows.ContainsKey(baseJoint))
            {
                Destroy(activeArrows[baseJoint]);
                activeArrows.Remove(baseJoint);
            }
            return;
        }

        GameObject arrow;

        if (activeArrows.ContainsKey(baseJoint))
        {
            arrow = activeArrows[baseJoint];

            if (arrow == null || arrow.name.Contains("Red") != desiredPrefab.name.Contains("Red"))
            {
                Destroy(arrow);
                arrow = Instantiate(desiredPrefab, baseJoint.position, Quaternion.identity, baseJoint);
                activeArrows[baseJoint] = arrow;
            }
            else if (arrow.name.Contains("Yellow") != desiredPrefab.name.Contains("Yellow"))
            {
                Destroy(arrow);
                arrow = Instantiate(desiredPrefab, baseJoint.position, Quaternion.identity, baseJoint);
                activeArrows[baseJoint] = arrow;
            }
        }
        else
        {
            arrow = Instantiate(desiredPrefab, baseJoint.position, Quaternion.identity, baseJoint);
            activeArrows[baseJoint] = arrow;
        }

        arrow.transform.position = baseJoint.position;

        Vector3 dir = (targetPosition - baseJoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
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
