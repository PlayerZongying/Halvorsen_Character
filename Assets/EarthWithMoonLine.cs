using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EarthWithMoonLine : MonoBehaviour
{
    private const int LineLength = 1500;
    private const int StandardFPS = 300;

    [FormerlySerializedAs("vertexArrayLength")]
    public int threadCount;
    public bool setFPS = false;
    [Range(15, 300)] public int maxFPS = 300;
    public bool initRotate = true;
    public GameObject rotationController;
    public GameObject realMoon;
    public float velocity = 10;
    public float angularVel = 1;
    public float radius = 5;


    // for lines update

    [Header("For Line Renderer")] private Vector3[] firstVertices;
    private List<Vector3> allVertices = new List<Vector3>();
    public LineRenderer realMoonLine;
    private Quaternion rotationInOneFrame;


    // Start is called before the first frame update
    private void Awake()
    {
        // #if UNITY_EDITOR
        //         QualitySettings.vSyncCount = 2; // VSync must be disabled
        //         // Application.targetFrameRate = 60;
        // #endif
        if (setFPS)
        {
            Application.targetFrameRate = maxFPS;
            threadCount = Mathf.RoundToInt((float)StandardFPS / maxFPS);
            firstVertices = new Vector3[threadCount];
            rotationInOneFrame = Quaternion.identity;
            for (int i = 0; i < threadCount; i++)
            {
                rotationInOneFrame *= transform.rotation;
            }
        }
    }

    void Start()
    {
        if (initRotate)
        {
            rotationController.transform.rotation = transform.rotation = Quaternion.Euler(159.2f, 23.2f, 27.16f);
        }

        // generate new vertices in the ring plane.
        GenerateNewVertices();

        // rotate the vertices in one segment for up scaling 
        RotateNewVertices();

        // perpare for all vertices for the initial line;
        for (int i = 0; i < LineLength; i++)
        {
            allVertices.Add(firstVertices[threadCount - 1]);
        }


        // add new vertex, remove old vertex;
        UpdateAllVertices();

        RotateAllVertices();

        // set all the vertices into line renderer
        realMoonLine.SetPositions(allVertices.ToArray());
    }

    private void RotateAllVertices()
    {
        rotationInOneFrame = Quaternion.identity;
        for (int i = 0; i < threadCount; i++)
        {
            rotationInOneFrame *= transform.rotation;
        }
        
        for (int i = 0; i < LineLength; i++)
        {
            Vector3 vertex = allVertices[i];
            vertex = rotationInOneFrame * vertex;
            allVertices[i] = vertex;
        }
    }

    private void UpdateAllVertices()
    {
        for (int i = 0; i < threadCount; i++)
        {
            allVertices.Add(firstVertices[threadCount - 1 - i]);
            allVertices.RemoveAt(0);
        }
    }

    private void RotateNewVertices()
    {
        for (int i = 0; i < threadCount; i++)
        {
            Vector3 pos = firstVertices[i];

            for (int j = 0; j < i; j++)
            {
                pos = transform.rotation * pos;
            }

            firstVertices[i] = pos;
        }
    }

    private void GenerateNewVertices()
    {
        for (int i = 0; i < threadCount; i++)
        {
            float phase = Time.deltaTime * angularVel * ((float)maxFPS / StandardFPS) * i;

            // print($"Move moon prefab at: {Time.time}");
            Vector3 pos = new Vector3(Mathf.Cos((Time.time - Time.deltaTime) * angularVel - phase), 0,
                Mathf.Sin((Time.time - Time.deltaTime) * angularVel - phase)) * radius;

            pos = transform.rotation * pos;
            firstVertices[i] = pos;
        }
    }


    // Update is called once per frame
    void Update()
    {
        RotateEarth();
        MoveEarth();
        
        MoveMoon();
        
        // generate new vertices in the ring plane.
        GenerateNewVertices();
        // rotate the vertices in one segment for up scaling 
        RotateNewVertices();
        
        UpdateAllVertices();
        RotateAllVertices();
        // set all the vertices into line renderer
        realMoonLine.SetPositions(allVertices.ToArray());
    }

    void RotateEarth()
    {
        transform.rotation =
            Quaternion.Slerp(transform.rotation, rotationController.transform.rotation, Time.deltaTime);
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
        realMoon.transform.position = transform.TransformPoint(pos);
    }

}