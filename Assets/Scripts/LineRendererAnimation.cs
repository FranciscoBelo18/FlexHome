using UnityEngine;
using System.Collections.Generic;

public class IconFlowBetweenObjects : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Sprite connectionSprite;
    private float iconScale = 0.01f;
    private float speed = 0.5f;
    private float spawnRate = 0.5f;

    private float timer = 0f;
    private List<GameObject> activeIcons = new List<GameObject>();

    void Update()
    {
        if (pointA == null || pointB == null || connectionSprite == null)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            SpawnIcon();
            timer = 0f;
        }

        for (int i = activeIcons.Count - 1; i >= 0; i--)
        {
            GameObject icon = activeIcons[i];
            if (icon == null) continue;

            icon.transform.position = Vector3.MoveTowards(
                icon.transform.position,
                pointB.position,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(icon.transform.position, pointB.position) < 0.01f)
            {
                Destroy(icon);
                activeIcons.RemoveAt(i);
            }
        }
    }

    void SpawnIcon()
    {
        GameObject iconObj = new GameObject("MovingIcon");
        SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
        sr.sprite = connectionSprite;
        sr.sortingOrder = 1;

        iconObj.transform.localScale = Vector3.one * iconScale;

        iconObj.transform.position = pointA.position;

        activeIcons.Add(iconObj);
    }
}
