using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class EarthWithMoons : MonoBehaviour
{
    private const int StandardFPS = 300;
    [Range(15, 300)] public int maxFPS = 300;
    public bool isSetPhase;
    public float setPhase = 10;
    public float timeScale = 1;
    public bool initRotate = true;


    public GameObject realMoon;
    public TrailRenderer realMoonTrail;
    public LineRenderer realMoonLine;

    public GameObject moonPrefab;
    public List<GameObject> _moons;
    public List<TrailRenderer> _moonTrailRenderers;

    public float velocity = 10;

    public float angularVel = 1;
    public float radius = 5;

    public Vector3 lastPos = Vector3.zero;

    public Quaternion lastRot = Quaternion.identity;

    // Start is called before the first frame update
    private void Awake()
    {
// #if UNITY_EDITOR
//         QualitySettings.vSyncCount = 2; // VSync must be disabled
//         // Application.targetFrameRate = 60;
// #endif
        Application.targetFrameRate = maxFPS;
    }

    void Start()
    {
        Time.timeScale = timeScale;
        SetupMoons();
        if (initRotate) transform.rotation = Quaternion.Euler(159.2f, 23.2f, 27.16f);
    }

    // Update is called once per frame
    void Update()
    {
        MoveEarth();

        // order fucking matters
        MoveTrails();
        MoveMoons();

        MoveMoon();
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

    void MoveMoon()
    {
        Vector3 pos = new Vector3(Mathf.Cos(Time.time * angularVel), 0, Mathf.Sin(Time.time * angularVel)) * radius;
        realMoon.transform.position = transform.TransformPoint(pos);

        //Vector3 localPos = transform.InverseTransformPoint(moon.transform.position);
        //moon.transform.position = transform.TransformPoint(localPos);
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

        foreach (TrailRenderer trail in _moonTrailRenderers)
        {
            int count = trail.positionCount;
            Vector3[] vertices = new Vector3[count];
            int realCount = trail.GetPositions(vertices);
            // print(count);


            Vector3 curPos = transform.position;
            for (int i = 0; i < count; i++)
            {
                Vector3 vertex = vertices[i] - curPos;
                vertex += curPos - lastPos;
                Vector3 newVertex = transform.TransformPoint(vertex);
                vertices[i] = newVertex;
            }

            lastPos = curPos;
            trail.SetPositions(vertices);
            allVertices = allVertices.Concat(vertices).ToArray();
        }

        Vector3[] reorderedVertices = new Vector3[allVertices.Length];
        print(reorderedVertices.Length);

        int verticeInOneTrail = allVertices.Length / _moons.Count;
        for (int i = 0; i < verticeInOneTrail; i++)
        {
            for (int j = 0; j < _moons.Count; j++)
            {
                reorderedVertices[(i * _moons.Count + _moons.Count - 1 - j)] =
                    allVertices[(_moons.Count - 1 - j) * verticeInOneTrail + i];
            }
        }

        int vertexCount = realMoonTrail.positionCount;
        if (reorderedVertices.Length > vertexCount)
        {
            Vector3[] enlarge = new Vector3[reorderedVertices.Length - vertexCount];
            realMoonTrail.AddPositions(enlarge);
        }

        if (reorderedVertices.Length > _moons.Count * 2 + 2)
        {
            for (int i = 0; i < _moons.Count * 2; i++)
            {
                // print(reorderedVertices[i]);
                Vector3 d1 = reorderedVertices[i + 1] - reorderedVertices[i];
                Vector3 d2 = reorderedVertices[i + 2] - reorderedVertices[i + 1];
                print(Vector3.Dot(d1.normalized, d2.normalized));
            }
        }

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
        print("-----------------------------------");
    }
}