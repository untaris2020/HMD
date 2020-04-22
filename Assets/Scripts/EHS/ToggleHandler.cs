using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public class ToggleHandler : tcpPacket
{
    // Loading Screen
    private string NAME; 
    public packetICD.Toggle_Mode MODE;
    public GameObject startScene; 
    private bool GloveActive;
    public GameObject gloveObj;
    private bool newPacket;

    private delegate void functionDelegate();

    public void Start()
    {
        Debug.Log("Toggle Started");
        //base.Start();
        newPacket = false; 
        seqID = -1; 
        connected = false;
        if (MODE == packetICD.Toggle_Mode.CHEST)
        {
            NAME = "chestToggle";
            debugName = "chest_toggle"; 
        }
        else
        {
            NAME = "gloveToggle";
            debugName = "glove_toggle";
        }

        reportStatus(); 

        functionDelegate stopDelegate = new functionDelegate(stopStream);
        functionDelegate startDelegate = new functionDelegate(startStream);
        functionDebug.Instance.registerFunction(NAME + "start", startDelegate);
        functionDebug.Instance.registerFunction(NAME + "stop", stopDelegate);
    }

    void Update()
    {
        if(newPacket)
        {
            if(MODE == packetICD.Toggle_Mode.CHEST)
            { 
                startScene.GetComponent<startBehavior>().DisableHID(); 
            }
            else if(MODE == packetICD.Toggle_Mode.GLOVE)
            {
                //Disable glove scripts here 
                gloveObj.GetComponent<IMUHandler>().setConnected(false); 
            }
            newPacket = false; 
        }
    }


    public override int processPacket(string packet)
    {
        //Debug.Log("Message Receieved From CLIENT");
        if(packet == "INACTIVE")
        {
            newPacket = true; 
        }
        return 0;
    }
}
