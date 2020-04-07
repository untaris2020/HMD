using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;

//using MissionPanelManager;

public class SuperMissionScript : MonoBehaviour
{
    public MissionPanelManager MPMInstance;
    //public MissionContainer missionContainer;

    // Start is called before the first frame update
    void Start()
    {
        createColliders();
        MPMInstance.nextPage();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void createColliders()
    {


    }
}
