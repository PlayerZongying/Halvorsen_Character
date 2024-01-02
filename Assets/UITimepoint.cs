using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITimepoint : MonoBehaviour
{
    public static UITimepoint instance;
    public TextMeshProUGUI TimePoint;
    public TextMeshProUGUI EulerAngle;

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
        
        Performer.instance.DisplayPerformInfo();
        
    }
}
