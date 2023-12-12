using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    // Start is called before the first frame update
    public static Attractor instance;

    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject touchPrefab;

    [Header("Materils")]
    [ColorUsageAttribute(true, true)]
    public Color color1;

    [ColorUsageAttribute(true, true)]
    public Color color2;
    public Material nodeMaterial;
    public Material trailMaterial;

    [Header("Movement")]
    public float speed;
    public float maxSpeed;

    public Transform cameraTarget;
    public Transform AllNote;
    public Transform AllTouch;

    public void Awake()
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

    void Start() { }

    // Update is called once per frame
    void Update() { }

    public virtual void Move(Transform transform, GameObject note) { }

    public void UpdateMaterials()
    {
        nodeMaterial.SetVector("_Color1", color1);
        nodeMaterial.SetVector("_Color2", color2);
        nodeMaterial.SetFloat("_MaxSpeed", maxSpeed);

        trailMaterial.SetVector("_Color1", color1);
        trailMaterial.SetVector("_Color2", color2);
        trailMaterial.SetFloat("_MaxSpeed", maxSpeed);

        nodeMaterial.SetMatrix("_Transform", AllNote.transform.worldToLocalMatrix);
        trailMaterial.SetMatrix("_Transform", AllNote.transform.worldToLocalMatrix);
    }
}
