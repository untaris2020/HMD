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

public class CameraHandler : tcpPacket
{

    // Loading Screen
    private string NAME; 
    public packetICD.CAM_Mode MODE;

    private delegate void functionDelegate();

    public void start()
    {
        seqID = -1; 
        connected = false;
        if (MODE == packetICD.CAM_Mode.HEAD)
        {
            NAME = "headCAM";
        }
        else
        {
            NAME = "gloveCam";
        }
        functionDelegate stopDelegate = new functionDelegate(stopStream);
        functionDelegate startDelegate = new functionDelegate(startStream);
        functionDebug.Instance.registerFunction(NAME + "start", startDelegate);
        functionDebug.Instance.registerFunction(NAME + "stop", stopDelegate);
    }



    public override int processPacket(string packet)
    {
        Debug.Log("Message Receieved From CLIENT CAMERA");
        CamerasManager.Instance.updateFrame(packet);
        return 0;
    }
    
}
