using System;
using Unity.Collections;
using UnityEngine;

public class Earth : MonoBehaviour
{
    public GameObject moon;
    public TrailRenderer moonTrail;

    public float velocity = 10;

    public float angularVel = 1;
    public float radius = 5;

    public Vector3 lastPos = Vector3.zero;

    public Quaternion lastRot = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(159.2f, 23.2f, 27.16f);
    }

    // Update is called once per frame
    void Update()
    {
        MoveEarth();
        
        // order fucking matters
        MoveTrail();
        MoveMoon();


        // MoveTheCrazyFuckingTrail();
    }

    private void LateUpdate()
    {
        
    }

    void MoveEarth()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Time.deltaTime * velocity * Vector3.up;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Time.deltaTime * velocity * Vector3.down;
        }
    }

    void MoveMoon()
    {
        Vector3 pos = new Vector3(Mathf.Cos(Time.time * angularVel), 0, Mathf.Sin(Time.time * angularVel)) * radius;
        moon.transform.position = transform.TransformPoint(pos);

        //Vector3 localPos = transform.InverseTransformPoint(moon.transform.position);
        //moon.transform.position = transform.TransformPoint(localPos);
    }

    void MoveTheCrazyFuckingTrail()
    {
        int count = moonTrail.positionCount;
        Vector3[] vertices = new Vector3[count];
        moonTrail.GetPositions(vertices);
        // print(count);
        for (int i = 0; i < count; i++)
        {
            Vector3 vertex = vertices[i] - transform.position;
            // print(vertex);
            Vector3 newVertex = transform.TransformPoint(vertex);
            vertices[i] = newVertex;
        }
        // print("-----------------------------------");

        moonTrail.SetPositions(vertices);
    }

    void MoveTrail()
    {
        int count = moonTrail.positionCount;
        Vector3[] vertices = new Vector3[count];
        int realCount =moonTrail.GetPositions(vertices);
        // print(count);
        
        UIFPS.instance.TrailCount.text = count.ToString();
        
        Vector3 curPos = transform.position;
        for (int i = 0; i < count; i++)
        {
            Vector3 vertex = vertices[i] - curPos;
            vertex += curPos - lastPos;
            Vector3 newVertex = transform.TransformPoint(vertex);
            vertices[i] = newVertex;
        }

        // print("-----------------------------------");
        lastPos = curPos;
        moonTrail.SetPositions(vertices);
    }
}