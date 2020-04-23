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

    public void Start()
    {
        base.Start();
        seqID = -1; 
        connected = false;
        if (MODE == packetICD.CAM_Mode.HEAD)
        {
            NAME = "headCAM";
            debugName = "head_cam";
        }
        else
        {
            NAME = "gloveCam";
            debugName = "glove_cam";
        }

        reportStatus(); 

        functionDelegate stopDelegate = new functionDelegate(stopStream);
        functionDelegate startDelegate = new functionDelegate(startStream);
        functionDebug.Instance.registerFunction(NAME + "start", startDelegate);
        functionDebug.Instance.registerFunction(NAME + "stop", stopDelegate);
    }

    protected override void handleDiscon()
    {
        //Toggle Camera inactive if camera goes down. 
        if((MODE == packetICD.CAM_Mode.HEAD))
        {
            ErrorHandler.Instance.HandleError(0, "HEAD CAM: ERROR LOST CONNECTION");
            DebugManager.Instance.LogBoth("HEAD CAM: ERROR LOST CONNECTION");
            if (NavManager.Instance.getHeadCam())
            {
                NavManager.Instance.ToggleRearviewCam();
            }
        }
        else
        {
            ErrorHandler.Instance.HandleError(0, "GLOVE CAM: ERROR LOST CONNECTION"); 
            DebugManager.Instance.LogBoth("GLOVE CAM: ERROR LOST CONNECTION");
            if(NavManager.Instance.getGloveCam())
            {
                NavManager.Instance.ToggleGloveCam();
            }
        }
        base.handleDiscon();
    }


    public override int processPacket(string packet)
    {
        Debug.Log("Message Receieved From CLIENT CAMERA");
        CamerasManager.Instance.updateFrame(packet);
        return 0;
    }
    
}
