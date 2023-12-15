using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTest : MonoBehaviour
{
    public LineRenderer lineRenderer;

    int posCount;

    public float angularVel = 1;

    private Vector3[] verticesPos;

    // Start is called before the first frame update
    void Start()
    {
        posCount = lineRenderer.positionCount;
        verticesPos = new Vector3[posCount];
        for (int i = 0; i < posCount; i++)
        {
            verticesPos[i] = new Vector3(Mathf.Cos( Time.time * angularVel - 2 * Mathf.PI / posCount * i), 0,
                Mathf.Sin(Time.time * angularVel - 2 * Mathf.PI / posCount * i));
        }

        lineRenderer.SetPositions(verticesPos);
    }

    // Update is called once per frame
    void Update()
    {
        // lineRenderer.GetPositions(verticesPos);
        for (int i = 0; i < posCount; i++)
        {
            verticesPos[i] = new Vector3(Mathf.Cos( Time.time * angularVel - 2 * Mathf.PI / posCount * i), 0,
                Mathf.Sin(Time.time * angularVel - 2 * Mathf.PI / posCount * i));
        }
        for (int i = 0; i < posCount; i++)
        {
            Vector3 pos = verticesPos[i];
            verticesPos[i] = transform.TransformPoint(verticesPos[i]);
        }

        lineRenderer.SetPositions(verticesPos);
    }
}