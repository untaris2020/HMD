using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject HID;
    private GameObject HIDInstance;
    public GameObject EHS;
    private GameObject EHSInstance; 

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting Scene...");
        Debug.Log("Initializing EHS...");
        EHSInstance = Instantiate(EHS, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log("Initializing HID...");
        HIDInstance = Instantiate(HID, new Vector3(0, 0, 0), Quaternion.identity);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
