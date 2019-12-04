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
        Debug.Log("Initializing Navagation Object...");
        Vector3 pos;  

        Quaternion rotation = Quaternion.Euler(0, navRot, 0);

        pos.x = gameObject.transform.position.x + (radius * Mathf.Sin(navRot * Mathf.Deg2Rad));

        Debug.Log("SIN RES" + Mathf.Sin(navRot * Mathf.Deg2Rad));
        Debug.Log("Radius " + radius);
        Debug.Log("Res: " + pos.x);
        

        pos.z = gameObject.transform.position.z + radius * Mathf.Cos(navRot * Mathf.Deg2Rad);
        pos.y = 0;

        NAVInstance = Instantiate(NAV,pos , rotation, gameObject.transform);
        Debug.Log("Initializing Telem Object...");

        rotation = Quaternion.Euler(0, telemRot, 0);

        pos.x = gameObject.transform.position.x + (radius * Mathf.Sin(telemRot * Mathf.Deg2Rad));

        Debug.Log("SIN RES" + Mathf.Sin(telemRot * Mathf.Deg2Rad));
        Debug.Log("Radius " + radius);
        Debug.Log("Res: " + pos.x);
        

        pos.z = gameObject.transform.position.z + radius * Mathf.Cos(telemRot * Mathf.Deg2Rad);
        pos.y = 0;

        
        TELEMInstance = Instantiate(TELEM, pos, rotation, gameObject.transform);
        Debug.Log("Initializing Mission Object...");
        MISSIONInstance = Instantiate(MISSION, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
