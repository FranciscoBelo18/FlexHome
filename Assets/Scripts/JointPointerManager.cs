using System.Collections.Generic;
using UnityEngine;

public class JointPointerManager : MonoBehaviour
{
    public GameObject redPointerPrefab;
    public GameObject yellowPointerPrefab;

    private Dictionary<int, GameObject> pointers = new Dictionary<int, GameObject>();

    public void CreatePointer(Transform currentJoint, Vector3 targetPosition, Color color)
    {
        if (currentJoint == null) return;

        int jointId = currentJoint.gameObject.GetInstanceID();

        if (color == Color.green)
        {
            RemovePointer(jointId);
            return;
        }

        GameObject prefabToUse = null;
        if (color == Color.red) {
            prefabToUse = redPointerPrefab;
        } else if (color == Color.yellow) {
            prefabToUse = yellowPointerPrefab;
        }

        if (prefabToUse == null) return;

        if (!pointers.ContainsKey(jointId) || pointers[jointId] == null)
        {
            GameObject pointer = GameObject.Instantiate(prefabToUse, transform);
            pointers[jointId] = pointer;
        }

        UpdatePointer(pointers[jointId], currentJoint.position, targetPosition);
    }

    private void UpdatePointer(GameObject pointer, Vector3 start, Vector3 end)
    {
        if (pointer == null) return;

        Vector3 direction = end - start;
        float distance = direction.magnitude;

        pointer.transform.position = start;
        pointer.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        pointer.transform.localScale = new Vector3(0.02f, distance, 0.02f);
    }

    private void RemovePointer(int jointId)
    {
        if (pointers.ContainsKey(jointId) && pointers[jointId] != null)
        {
            GameObject.Destroy(pointers[jointId]);
            pointers.Remove(jointId);
        }
    }

    public void ClearAllPointers()
    {
        foreach (var kvp in pointers)
        {
            if (kvp.Value != null)
            {
                GameObject.Destroy(kvp.Value);
            }
        }
        pointers.Clear();
    }
}
