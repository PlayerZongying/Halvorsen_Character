using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.Serialization;

public class Performer : MonoBehaviour
{
    public static Performer instance;

    [Serializable]
    public struct PerformBehavior
    {
        public float time;
        public Vector3 eulerAngles;
    }

    public AudioSource bgm;
    
    [Header("Controller")]
    public RotationController rotationController;
    public float startTimeOffset = 0;
    public int startTimePoint = 0;
    public List<PerformBehavior> performBehaviors;
    public int index = 0;

    [Header("Camera")]
    public Transform cameraCenter;
    public float cameraStartRotatingTime = 0;
    public float cameraStopRotatingTime = 1;
    public float rotateCircleCount = 4;
    public Vector3 rotationAxis = Vector3.right;
    // Start is called before the first frame update

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        index = startTimePoint;
        bgm.time = performBehaviors[index].time;
    }

    // Update is called once per frame
    void Update()
    {
        RotateController();
        RotateCameraSmooth();
    }

    public void DisplayPerformInfo()
    {
        if (index == performBehaviors.Count) return;
        if (performBehaviors[index].time > Time.time - startTimeOffset + performBehaviors[startTimePoint].time) return;
        UITimepoint.instance.TimePoint.text = index.ToString();
        UITimepoint.instance.EulerAngle.text = performBehaviors[index].eulerAngles.ToString();
    }

    void RotateController()
    {
        if (index == performBehaviors.Count) return;
        if (performBehaviors[index].time > Time.time - startTimeOffset + performBehaviors[startTimePoint].time) return;

        if (index == startTimePoint)
        {
            bgm.Play();
        }
        // print(Time.time);

        // UITimepoint.instance.TimePoint.text = index.ToString();
        // UITimepoint.instance.EulerAngle.text = performBehaviors[index].eulerAngles.ToString();

        rotationController.transform.eulerAngles = performBehaviors[index].eulerAngles;
        index++;
    }

    void RotateCamera()
    {
        float t = Time.time - startTimeOffset + performBehaviors[startTimePoint].time;

        float rotationAngle =
            Mathf.Clamp(
                360 * rotateCircleCount / (cameraStopRotatingTime - cameraStartRotatingTime) *
                (t - cameraStartRotatingTime), 0, 360 * rotateCircleCount);

        cameraCenter.rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);
    }
    
    void RotateCameraSmooth()
    {
        float t = Time.time - startTimeOffset + performBehaviors[startTimePoint].time - cameraStartRotatingTime;
        t /=  (cameraStopRotatingTime - cameraStartRotatingTime);
        t = Mathf.Clamp(t, 0, 1);

        float rotationAngle = Mathf.SmoothStep(0, 360 * rotateCircleCount, t);

        cameraCenter.rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);
    }
}