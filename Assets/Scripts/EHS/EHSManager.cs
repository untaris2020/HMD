using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHSManager : MonoBehaviour
{
    public GameObject IMUChestObj;
    private GameObject IMUChest; 

    public GameObject IMUGloveObj;
    private GameObject IMUGlove;

    public GameObject ForceSensorObj;
    private GameObject ForceSensor;

    // Start is called before the first frame update
    void Start()
    {
       IMUChest = Instantiate(IMUChestObj, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform) as GameObject;
       IMUGlove = Instantiate(IMUGloveObj, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform) as GameObject;
       ForceSensor = Instantiate(ForceSensorObj, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform) as GameObject;
    }
}
