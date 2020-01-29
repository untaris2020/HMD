using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIDManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float radius;
    public float telemRot; 
    public float navRot;
    
        
    public GameObject NAV;
    private GameObject NAVInstance;

    public GameObject TELEM;
    private GameObject TELEMInstance;

    public GameObject MISSION;
    private GameObject MISSIONInstance;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
