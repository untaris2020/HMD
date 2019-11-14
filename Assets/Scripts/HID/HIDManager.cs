using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIDManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject NAV;
    private GameObject NAVInstance;

    public GameObject TELEM;
    private GameObject TELEMInstance;

    public GameObject MISSION;
    private GameObject MISSIONInstance;

    void Start()
    {
        Debug.Log("Initializing Navagation Object...");
        NAVInstance = Instantiate(NAV, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
        Debug.Log("Initializing Telem Object...");
        TELEMInstance = Instantiate(TELEM, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
        Debug.Log("Initializing Mission Object...");
        MISSIONInstance = Instantiate(MISSION, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
