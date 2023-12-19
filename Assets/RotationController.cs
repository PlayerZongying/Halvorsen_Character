using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    private bool isRotating = false;

    private Vector3 initialPos = new Vector3();
    private Quaternion initialRot = new Quaternion();

    private Quaternion targetRot;
    private Vector3 oriantationInEulerAngle = new Vector3();

    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isRotating = true;
            initialPos = Input.mousePosition;
            initialRot = transform.rotation;
        }

        if (Input.GetMouseButton(0))
        {
            oriantationInEulerAngle = Input.mousePosition - initialPos;

            Vector3 upInViewSpace = camera.transform.up;
            Vector3 rightInViewSpace = camera.transform.right;

            Vector3 upInWorldSpace = transform.worldToLocalMatrix * upInViewSpace;
            Vector3 rightInWorldSpace = transform.worldToLocalMatrix * rightInViewSpace;


            targetRot = initialRot *
                        Quaternion.AngleAxis(oriantationInEulerAngle.y * Time.deltaTime, rightInWorldSpace) *
                        Quaternion.AngleAxis(- oriantationInEulerAngle.x * Time.deltaTime, upInWorldSpace);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            transform.rotation = targetRot;
        }
    }

    void RotateControllerfor(float x, float y)
    {
        transform.rotation = Quaternion.AngleAxis(y * Time.deltaTime, Vector3.right) *
                             Quaternion.AngleAxis(x * Time.deltaTime, Vector3.up);
    }
}