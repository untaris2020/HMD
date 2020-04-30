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
    public ModelLoader ML;
    public audioManager AM; 
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

    protected override void handleDiscon()
    {

        if (MODE == packetICD.Toggle_Mode.CHEST)
        {
            ErrorHandler.Instance.HandleError(0, "CHEST TOGGLE: ERROR LOST CONNECTION");
            DebugManager.Instance.LogBoth("CHEST TOGGLE: ERROR LOST CONNECTION");

            //We need to hide the HID until connection is back as well as stop playback, clear models and camera
            //Toggle 3D inactive 
            ML.ClearModelsButton();

            AM.stopPlayBack();

            if(AM.getRecording())
            {
                AM.recordingHit(); //This should stop recording
            }

            //Toggle Camera inactive
            if (NavManager.Instance.getHeadCam())
            {
                NavManager.Instance.ToggleRearviewCam();
            }
            if (NavManager.Instance.getGloveCam())
            {
                NavManager.Instance.ToggleGloveCam();
            }

            //Toggle HID invis 
            startBehavior.instance.DisableHID(); 

            // Toggle gaze active
            InputSystemStatus.Instance.ChangeGazeStatus(true);
        }
        else
        {
            ErrorHandler.Instance.HandleError(0, "GLOVE TOGGLE: ERROR LOST CONNECTION");
            DebugManager.Instance.LogBoth("GLOVE TOGGLE: ERROR LOST CONNECTION");
            
            //If glove is down clear all models 
            ML.ClearModelsButton();

            if(NavManager.Instance.getGloveCam())
            {
                NavManager.Instance.ToggleGloveCam();
            }

            InputSystemStatus.Instance.ChangeGloveStatus(false);
        }


        base.handleDiscon();
    }


    void Update()
    {
        base.Update();
        if(newPacket)
        {
            if(MODE == packetICD.Toggle_Mode.CHEST)
            {
                Debug.Log("this is calling");
                startScene.GetComponent<startBehavior>().DisableHID(); 
            }
            else if(MODE == packetICD.Toggle_Mode.GLOVE)
            {
                //Disable glove scripts here 
                //Debug.Log("Setting glove to discon");
                if(gloveObj.GetComponent<IMUHandler>().getConnected())
                {
                    gloveObj.GetComponent<IMUHandler>().setConnected(false); 

                    ErrorHandler.Instance.HandleError(0, "GLOVE IMU: ERROR LOST CONNECTION");
                    DebugManager.Instance.LogBoth("GLOVE IMU: ERROR LOST CONNECTION");
                    //If glove is down clear all models 
                     ML.ClearModelsButton();
                }
               

            }
            newPacket = false; 
        }
    }


    public override int processPacket(string packet)
    {
        //Debug.Log("Message Receieved From CLIENT: " + packet);
        if(packet == "INACTIVE")
        {
            newPacket = true; 
        }
        return 0;
    }
}
