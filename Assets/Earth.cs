using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Earth : MonoBehaviour
{
    public bool initRotate = true;
    public bool setFPS = false;
    private const int StandardFPS = 300;
    [Range(15, 300)] public int maxFPS = 300;
    public GameObject moon;
    public TrailRenderer moonTrail;

    public float timeScale = 1;

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
        if (setFPS)
        {
            Application.targetFrameRate = maxFPS;
        }
    }

    void Start()
    {
        if (initRotate)
        {
            transform.rotation = Quaternion.Euler(159.2f, 23.2f, 27.16f);
        }

        Time.timeScale = timeScale;
        // SetupMoons();
    }

    // Update is called once per frame
    void Update()
    {
        MoveEarth();

        // order fucking matters
        MoveTrail();
        MoveMoon();


        // order fucking matters
        // MoveTrails();
        // MoveMoons();
    }

    // private void FixedUpdate()
    // {
    //     MoveEarth();
    //     
    //     // order fucking matters
    //     MoveTrail();
    //     MoveMoon();
    // }

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


    void SetupMoons()
    {
        // _moons.Add(moon);

        for (int i = 0; i < 5; i++)
        {
            GameObject newMoon = Instantiate(moon, Vector3.zero, Quaternion.identity);
            _moons.Add(newMoon);
            print(newMoon.name);
            TrailRenderer trailRenderer = newMoon.GetComponentInChildren<TrailRenderer>();
            _moonTrailRenderers.Add(trailRenderer);
        }
    }

    void MoveMoons()
    {
        for (int i = 0; i < _moons.Count; i++)
        {
            float phase = Mathf.PI * 2 / (300f / 60f) * i;
            Vector3 pos = new Vector3(Mathf.Cos(Time.time * angularVel - phase), 0,
                Mathf.Sin(Time.time * angularVel) - phase) * radius;
            _moons[i].transform.position = transform.TransformPoint(pos);
        }

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
        int realCount = moonTrail.GetPositions(vertices);
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

    void MoveTrails()
    {
        foreach (TrailRenderer trail in _moonTrailRenderers)
        {
            int count = trail.positionCount;
            Vector3[] vertices = new Vector3[count];
            int realCount = trail.GetPositions(vertices);
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
            trail.SetPositions(vertices);
        }
    }
}