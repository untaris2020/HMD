using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHSManager : MonoBehaviour
{
    public GameObject IMUChestObj;
    public GameObject IMUChest; 

    // Start is called before the first frame update
    void Start()
    {
       IMUChest = Instantiate(IMUChestObj, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform) as GameObject;
    }
}
