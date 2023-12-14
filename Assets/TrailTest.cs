using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTest : MonoBehaviour
{
    public GameObject sp0;
    public GameObject sp1;
    public GameObject sp2;

    public TrailRenderer tr0;
    public TrailRenderer tr1;
    public TrailRenderer tr2;

    public float radius = 5;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float theta = Mathf.CeilToInt(Time.time) * Mathf.PI / 2;

        sp0.transform.position = radius * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
        sp1.transform.position = radius * new Vector3(Mathf.Cos(theta - Mathf.PI / 4), Mathf.Sin(theta - Mathf.PI / 4), 0);

        sp2.transform.position = sp0.transform.position;
    }
}
