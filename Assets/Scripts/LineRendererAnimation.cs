using UnityEngine;
using System.Collections.Generic;

public class LineRendererAnimation : MonoBehaviour
{
    public Sprite connectionSprite;
    public float iconScale = 0.01f;
    public float speed = 0.5f;
    public float spawnRate = 0.5f;

    private float timer = 0f;

    private class LineData
    {
        public Transform startPoint;   // joint real
        public Vector3 endPoint;       // posição prevista
        public List<GameObject> icons = new List<GameObject>();
    }

    private List<LineData> activeLines = new List<LineData>();

    // 🔹 Chamado pelo JointPrediction
    public void CreateLine(Transform start, Vector3 end)
    {
        // verifica se já existe uma linha para este joint
        foreach (var line in activeLines)
        {
            if (line.startPoint == start)
            {
                line.endPoint = end; // só atualiza o destino
                return;
            }
        }

        var newLine = new LineData { startPoint = start, endPoint = end };
        activeLines.Add(newLine);
    }

    void Update()
    {
        if (connectionSprite == null || activeLines.Count == 0)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            foreach (var line in activeLines)
            {
                SpawnIcon(line);
            }
            timer = 0f;
        }

        foreach (var line in activeLines)
        {
            for (int i = line.icons.Count - 1; i >= 0; i--)
            {
                GameObject icon = line.icons[i];
                if (icon == null) continue;

                icon.transform.position = Vector3.MoveTowards(
                    icon.transform.position,
                    line.endPoint,
                    speed * Time.deltaTime
                );

                if (Vector3.Distance(icon.transform.position, line.endPoint) < 0.01f)
                {
                    Destroy(icon);
                    line.icons.RemoveAt(i);
                }
            }
        }
    }

    void SpawnIcon(LineData line)
    {
        if (line.startPoint == null) return;

        GameObject iconObj = new GameObject("MovingIcon");
        SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
        sr.sprite = connectionSprite;
        sr.sortingOrder = 1;

        iconObj.transform.localScale = Vector3.one * iconScale;
        iconObj.transform.position = line.startPoint.position;

        line.icons.Add(iconObj);
    }
}
