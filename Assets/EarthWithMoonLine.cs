using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    public bool isSetPhase;
    public float setPhase = 10;
    public float timeScale = 1;
    public bool initRotate = true;

    public GameObject rotationController;


    public GameObject realMoon;
    public TrailRenderer realMoonTrail;

    public GameObject moonPrefab;
    public List<GameObject> _moons;
    public List<TrailRenderer> _moonTrailRenderers;

    public float velocity = 10;

    public float angularVel = 1;
    public float radius = 5;

    public Vector3 lastPos = Vector3.zero;

    public Quaternion lastRot = Quaternion.identity;


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
        Time.timeScale = timeScale;
        SetupMoons();
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
            if (isSetPhase) phase = setPhase * i;

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

        // order fucking matters
        MoveTrails();

        MoveMoons();
        MoveMoon();

        SetNewVertexInTrails();
        
        
        // generate new vertices in the ring plane.
        GenerateNewVertices();
        // rotate the vertices in one segment for up scaling 
        RotateNewVertices();
        
        UpdateAllVertices();
        RotateAllVertices();
        // set all the vertices into line renderer
        realMoonLine.SetPositions(allVertices.ToArray());
    }

    private void LateUpdate()
    {
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


    void SetupMoons()
    {
        // _moons.Add(moon);
        int moonsCount = Mathf.RoundToInt((float)StandardFPS / maxFPS);

        for (int i = 0; i < moonsCount; i++)
        {
            GameObject newMoon = Instantiate(moonPrefab, Vector3.zero, Quaternion.identity);
            _moons.Add(newMoon);
            TrailRenderer trailRenderer = newMoon.GetComponentInChildren<TrailRenderer>();
            _moonTrailRenderers.Add(trailRenderer);
        }
    }

    void MoveMoonWeired()
    {
        realMoon.transform.position = _moons[0].transform.position;
        // Vector3 pos = new Vector3(Mathf.Cos(Time.time * angularVel), 0, Mathf.Sin(Time.time * angularVel)) * radius;
        // realMoon.transform.position = transform.TransformPoint(pos);
    }

    void MoveMoon()
    {
        // print($"Move real moon at: {Time.time}");

        realMoon.transform.position = _moons[0].transform.position;

        // Vector3 pos = new Vector3(Mathf.Cos(Time.time * angularVel), 0, Mathf.Sin(Time.time * angularVel)) * radius;
        // realMoon.transform.position = transform.TransformPoint(pos);
    }

    // Fucking Amazing
    void MoveMoonsWrong()
    {
        for (int i = 0; i < _moons.Count; i++)
        {
            float phase = angularVel / StandardFPS * i;
            Vector3 pos = new Vector3(Mathf.Cos(Time.time * angularVel - phase), 0,
                Mathf.Sin(Time.time * angularVel) - phase) * radius;
            _moons[i].transform.position = transform.TransformPoint(pos);
        }

        //Vector3 localPos = transform.InverseTransformPoint(moon.transform.position);
        //moon.transform.position = transform.TransformPoint(localPos);
    }

    void MoveMoons()
    {
        for (int i = 0; i < _moons.Count; i++)
        {
            float phase = Time.deltaTime * angularVel * ((float)maxFPS / StandardFPS) * i;
            if (isSetPhase) phase = setPhase * i;

            // print($"Move moon prefab at: {Time.time}");
            Vector3 pos = new Vector3(Mathf.Cos((Time.time - Time.deltaTime) * angularVel - phase), 0,
                Mathf.Sin((Time.time - Time.deltaTime) * angularVel - phase)) * radius;
            _moons[i].transform.position = transform.TransformPoint(pos);
        }

        //Vector3 localPos = transform.InverseTransformPoint(moon.transform.position);
        //moon.transform.position = transform.TransformPoint(localPos);
    }


    void MoveTrails()
    {
        Vector3[] allVertices = Array.Empty<Vector3>();

        for (int index = 0; index < _moons.Count; index++)
        {
            TrailRenderer trail = _moonTrailRenderers[index];
            int count = trail.positionCount;
            Vector3[] vertices = new Vector3[count];
            int realCount = trail.GetPositions(vertices);
            // print(count);


            Vector3 curPos = transform.position;
            for (int i = 0; i < count; i++)
            {
                Vector3 vertex = vertices[i] - curPos;
                vertex += curPos - lastPos;

                // Vector3 newVertex = transform.TransformPoint(vertex);;

                Vector3 newVertex = transform.rotation * vertex;

                for (int j = 1; j < _moons.Count; j++)
                {
                    newVertex = transform.rotation * newVertex;
                }

                vertices[i] = newVertex;
            }

            lastPos = curPos;
            trail.SetPositions(vertices);
            allVertices = allVertices.Concat(vertices).ToArray();
        }

        // foreach (TrailRenderer trail in _moonTrailRenderers)
        // {
        //     int count = trail.positionCount;
        //     Vector3[] vertices = new Vector3[count];
        //     int realCount = trail.GetPositions(vertices);
        //     // print(count);
        //
        //
        //     Vector3 curPos = transform.position;
        //     for (int i = 0; i < count; i++)
        //     {
        //         Vector3 vertex = vertices[i] - curPos;
        //         vertex += curPos - lastPos;
        //
        //         // Vector3 newVertex = transform.TransformPoint(vertex);;
        //         
        //         Vector3 newVertex = transform.rotation * vertex;
        //
        //         for (int j = 1; j < _moons.Count; j++)
        //         {
        //             newVertex = transform.rotation * newVertex;
        //         }
        //
        //         vertices[i] = newVertex;
        //     }
        //
        //     lastPos = curPos;
        //     trail.SetPositions(vertices);
        //     allVertices = allVertices.Concat(vertices).ToArray();
        // }

        Vector3[] reorderedVertices = new Vector3[allVertices.Length];
        // print(reorderedVertices.Length);

        int verticeInOneTrail = allVertices.Length / _moons.Count;
        // for (int i = 0; i < verticeInOneTrail; i++)
        // {
        //     for (int j = 0; j < _moons.Count; j++)
        //     {
        //         reorderedVertices[(i * _moons.Count + _moons.Count - 1 - j)] =
        //             allVertices[(_moons.Count - 1 - j) * verticeInOneTrail + i];
        //     }
        // }

        for (int i = 0; i < _moons.Count; i++)
        {
            for (int j = 0; j < verticeInOneTrail; j++)
            {
                reorderedVertices[j * _moons.Count + _moons.Count - 1 - i] = allVertices[i * verticeInOneTrail + j];
            }
        }

        int vertexCount = realMoonTrail.positionCount;
        if (reorderedVertices.Length > vertexCount)
        {
            Vector3[] enlarge = new Vector3[reorderedVertices.Length - vertexCount];
            realMoonTrail.AddPositions(enlarge);
        }

        // //direction test
        // if (reorderedVertices.Length > _moons.Count * 2 + 2)
        // {
        //     for (int i = 0; i < _moons.Count * 2; i++)
        //     {
        //         // print(reorderedVertices[i]);
        //         Vector3 d1 = reorderedVertices[i + 1] - reorderedVertices[i];
        //         Vector3 d2 = reorderedVertices[i + 2] - reorderedVertices[i + 1];
        //         print(Vector3.Dot(d1.normalized, d2.normalized));
        //     }
        // }

        // if (reorderedVertices.Length > realMoonLine.positionCount)
        // {
        //     Vector3[] enlarge = new Vector3[reorderedVertices.Length - realMoonLine.positionCount];
        //     realMoonTrail.AddPositions(enlarge);
        // }

        // realMoonLine.SetPositions(reorderedVertices);

        realMoonTrail.SetPositions(reorderedVertices);


        // Vector3[] array1 = new Vector3[reorderedVertices.Length];
        // for (int i = 0; i < reorderedVertices.Length; i++)
        // {
        //     array1[i] = Vector3.zero;
        // }
        // realMoonTrail.SetPositions(array1);

        UIFPS.instance.TrailCount.text = realMoonTrail.positionCount.ToString();
        // print(reorderedVertices.Length);
        // print("-----------------------------------");
    }

    void SetNewVertexInTrails()
    {
        for (int i = 0; i < _moons.Count; i++)
        {
            TrailRenderer trail = _moonTrailRenderers[i];
            int count = trail.positionCount;
            Vector3 newestVertex = trail.GetPosition(count - 1);

            for (int j = 0; j < i; j++)
            {
                newestVertex = transform.rotation * newestVertex;
            }

            trail.SetPosition(count - 1, newestVertex);
        }
    }
}