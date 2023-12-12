using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Halvorsen : Attractor
{
    // Start is called before the first frame update
    [Header("Halvorsen Parameters")]
    public float a;

    void Start()
    {
        AllNote.transform.rotation *= Quaternion.AngleAxis(-Mathf.Atan(1/Mathf.Sqrt(2)) * Mathf.Rad2Deg, Vector3.right);;
        AllNote.transform.rotation *= Quaternion.AngleAxis(135, Vector3.up);
    }

    // Update is called once per frame
    void Update() { }

    public override void Move(Transform transform, GameObject note)
    {
        // print("Move Halvorsen");

        //float x = note.transform.position.x;
        //float y = note.transform.position.y;
        //float z = note.transform.position.z;

        //float dx = -param_Halvorsen * x - 4 * y - 4 * z - y * y;
        //float dy = -param_Halvorsen * y - 4 * z - 4 * x - z * z;
        //float dz = -param_Halvorsen * z - 4 * x - 4 * y - x * x;

        //note.transform.position += new Vector3(dx, dy, dz) * Time.deltaTime * speed;


        Vector3 worldPos = note.transform.position;
        Vector3 localPos = transform.InverseTransformPoint(worldPos);

        float x = localPos.x;
        float y = localPos.y;
        float z = localPos.z;

        float dx = -a * x - 4 * y - 4 * z - y * y;
        float dy = -a * y - 4 * z - 4 * x - z * z;
        float dz = -a * z - 4 * x - 4 * y - x * x;

        localPos += Time.deltaTime * speed * new Vector3(dx, dy, dz);

        worldPos = transform.TransformPoint(localPos);

        note.transform.position = worldPos;
    }
}
