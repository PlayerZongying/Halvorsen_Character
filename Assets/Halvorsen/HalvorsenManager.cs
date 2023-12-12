using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalvorsenManager : MonoBehaviour
{
    public static HalvorsenManager instance;
    public enum AttractorType
    {
        Halvorsen,
        DequanLi,
        Aizawa,
        Lorenz
    };

    [Header("Type")]
    public AttractorType SelectedType;
    public delegate void MoveDelegate(Transform transform, GameObject note);
    public MoveDelegate moveDelegate;
    public float speed = 0.1f;

    [Header("Note")]
    public GameObject NotePrefab;
    public int NoteCount = 10;
    public Transform AllNotes;
    List<GameObject> NoteList = new List<GameObject>();

    // Start is called before the first frame update

    public Material Mat_trail;
    public Material Mat_note;

    // [Header("Touch")]
    // public GameObject TouchPrefab;
    // public int TouchCount = 10;
    // public Transform AllTouch;
    // List<GameObject> TouchList = new List<GameObject>();

    Attractor attractor;

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.visible = false;
        attractor = Attractor.instance;

        SetAttractorType();

        for (int i = 0; i < NoteCount; i++)
        {
            GameObject newNote = Instantiate(NotePrefab, RomdomPosAround(Vector3.zero), Quaternion.identity);
            newNote.transform.parent = AllNotes;
            NoteList.Add(newNote);
        }

        // for (int i = 0; i < TouchCount; i++)
        // {
        //     GameObject newTouch = Instantiate(TouchPrefab, RomdomPosAround(Vector3.zero), Quaternion.identity);
        //     newTouch.transform.parent = AllTouch;
        //     TouchList.Add(newTouch);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        //speed = Mathf.Sin(Time.realtimeSinceStartup) * 0.05f + 0.1f;

        foreach (GameObject note in NoteList)
        {
            if (note.transform.position.magnitude > 1000000)
            {
                Debug.Log(note.name + " is popped out");
                note.SetActive(false);
                note.transform.position = RomdomPosAround(Vector3.zero);
                note.SetActive(true);
            }
            moveDelegate(attractor.AllNote.transform, note);
            note.transform.LookAt(Camera.main.transform.position);
            // HalvorsenMove(note);
            //note.transform.LookAt( - Camera.main.transform.position);
            //Debug.Log(Camera.main.transform.position);
        }

        // foreach (GameObject touch in TouchList)
        // {
        //     if (touch.transform.position.magnitude > 1000000)
        //     {
        //         Debug.Log(touch.name + " is popped out");
        //         touch.SetActive(false);
        //         touch.transform.position = RomdomPosAround(Vector3.zero);
        //         touch.SetActive(true);
        //     }
        //     moveDelegate(attractor.transform, touch);
        //     touch.transform.LookAt(Camera.main.transform.position);
        //     // HalvorsenMove(note);
        //     //note.transform.LookAt( - Camera.main.transform.position);
        //     //Debug.Log(Camera.main.transform.position);
        // }

        //Debug.Log(transform.worldToLocalMatrix);

        // UpdateMaterials();
        attractor.UpdateMaterials();
    }

    Vector3 RomdomPosAround(Vector3 currentPos)
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        Vector3 randomPos = new Vector3(x, y, z);
        randomPos = transform.TransformPoint(randomPos) + currentPos;
        return randomPos;
    }

    void UpdateMaterials()
    {
        Mat_trail.SetMatrix("_Transform", transform.worldToLocalMatrix);
        Mat_note.SetMatrix("_Transform", transform.worldToLocalMatrix);
    }

    void SetAttractorType()
    {
        // AttractorMovement am = AttractorMovement.instance;

        // switch (SelectedType)
        // {
        //     case AttractorType.Halvorsen:
        //         print("shiHalvorsen");
        //         moveDelegate = am.HalvorsenMove;
        //         break;
        //         ;
        //     case AttractorType.DequanLi:
        //         print("shit2");
        //         moveDelegate = am.DequanLiMove;
        //         break;
        //         ;
        //     case AttractorType.Aizawa:
        //         print("shiAizawa");
        //         moveDelegate = am.AizawaMove;
        //         break;
        //         ;

        //     case AttractorType.Lorenz:
        //         print("Lorenz");
        //         moveDelegate = am.LorenzMove;
        //         break;
        //         ;
        // }

        moveDelegate = attractor.Move;
        NotePrefab = attractor.nodePrefab;
        // TouchPrefab = attractor.touchPrefab;
        // CinemachineFreeLook freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
        // freeLookCamera.LookAt = attractor.cameraTarget;
        // freeLookCamera.Follow = attractor.cameraTarget;
        AllNotes = attractor.AllNote;
        // AllTouch = attractor.AllTouch;
    }
}
