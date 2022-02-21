using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform[] points;
    public Transform intiPos;
    public Transform endPos;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setUpLine(Transform[] points)
    {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
