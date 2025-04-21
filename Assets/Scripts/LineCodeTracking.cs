using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineCodeTracking : MonoBehaviour
{

    LineRenderer lineRenderer;
    public Transform origin;
    public Transform destination;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, origin.localPosition);
        lineRenderer.SetPosition(1, destination.localPosition);
    }
}
