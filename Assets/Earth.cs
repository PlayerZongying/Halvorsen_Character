using Unity.Collections;
using UnityEngine;

public class Earth : MonoBehaviour
{
    public GameObject moon;
    public TrailRenderer moonTrail;

    public float angularVel = 1;
    public float radius = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveMoon();
        MoveTrail();
    }

    void MoveMoon()
    {
        // Vector3 pos = new Vector3(Mathf.Cos(Time.time * angularVel), 0, Mathf.Sin(Time.time * angularVel)) * radius;
        // moon.transform.position = transform.TransformPoint(pos);

        Vector3 localPos = transform.InverseTransformPoint(moon.transform.position);
        moon.transform.position = transform.TransformPoint(localPos);
    }

    void MoveTrail()
    {
        
        
        int count = moonTrail.positionCount;
        Vector3[] vertices = new Vector3[count];
        moonTrail.GetPositions(vertices);
        print(count);
        for (int i = 0; i < count; i++)
        {
            Vector3 vertex = vertices[i] - transform.position ;
            // print(vertex);
            Vector3 newVertex = transform.TransformPoint(vertex);
            vertices[i] = newVertex;
        }
        // print("-----------------------------------");
        
        moonTrail.SetPositions(vertices);
    }
}
