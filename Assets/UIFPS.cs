using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIFPS : MonoBehaviour
{
    public static UIFPS instance;
    public TextMeshProUGUI FPS;
    public TextMeshProUGUI TrailCount;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FPS.text = (1f/Time.deltaTime).ToString();
    }
}
