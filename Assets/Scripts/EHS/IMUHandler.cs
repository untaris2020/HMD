using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Linq;

public class IMUHandler : tcpPacket
{
    public packetICD.IMU_Mode MODE;
    public int maxListSize; 

    public ModelLoader ML;
    public audioManager AM; 
    private string NAME; 

    //IMU Packet Information 
    private float x;
    private float y;
    private float z;
    private float w;
    private float xAccel;
    private float yAccel;
    private float zAccel;
    private float xAverage;
    private float yAverage;
    private float zAverage;
    private float rotThreshold = 30.0f; 
    private Quaternion startingVal;
    private Quaternion prevVal;
    private float maxAngle;
    private int count;
    private bool shift;
    private const int standardDeviation = 3;
    private bool startValSet = false;
    
    private delegate void functionDelegate();

    public void Start()
    {
        shift = true;
        count = 0;
        base.Start();
        //Debug.Log("START IMU");
        seqID = -1; 
        connected = false;

        maxAngle = 0; 

        if (MODE == packetICD.IMU_Mode.CHEST)
        {
            NAME = "chestIMU";
            debugName = "chest_IMU";
        }
        else
        {
            NAME = "gloveIMU";
            debugName = "glove_IMU";
        }

        reportStatus(); 

        functionDelegate stopDelegate = new functionDelegate(stopStream);
        functionDelegate startDelegate = new functionDelegate(startStream);
        functionDebug.Instance.registerFunction(NAME + "start", startDelegate);
        functionDebug.Instance.registerFunction(NAME + "stop", stopDelegate);
    }

    protected override void handleDiscon()
    {
        //Debug.Log("Discon IMU: " + MODE.ToString());

        if(MODE == packetICD.IMU_Mode.CHEST)
        {
            ErrorHandler.Instance.HandleError(0, "CHEST IMU: ERROR LOST CONNECTION");
            DebugManager.Instance.LogBoth("CHEST IMU: ERROR LOST CONNECTION");
            //We need to hide the HID until connection is back as well as stop playback, clear models and camera
            //Toggle 3D inactive 
            ML.ClearModelsButton();

            AM.stopPlayBack(); 
            if(AM.getRecording())
            {
                AM.recordingHit(); //This should stop recording
            }

            //Toggle Camera inactive
            if(NavManager.Instance.getHeadCam())
            {
                NavManager.Instance.ToggleRearviewCam();
            }
            if(NavManager.Instance.getGloveCam())
            {
                NavManager.Instance.ToggleGloveCam();
            }

            // Toggle gaze active
            InputSystemStatus.Instance.ChangeGazeStatus(true);


            //Toggle HID invis 
            startBehavior.instance.DisableHID();
        }
        else
        {
            ErrorHandler.Instance.HandleError(0, "GLOVE IMU: ERROR LOST CONNECTION");
            DebugManager.Instance.LogBoth("GLOVE IMU: ERROR LOST CONNECTION");
            //If glove is down clear all models 
            ML.ClearModelsButton();
        }

        startValSet = false;
        base.handleDiscon();
    }

    public override int processPacket(string packet)
    {
        string[] tmp = packet.Split(new string[] { "$" }, StringSplitOptions.None);
        try
        {
            if (tmp.Length == 8)
            {
                //If seqID has overflown we need to reset
                if (seqID >= 2147483647)
                {
                    seqID = -1;
                }

                if (Int32.Parse(tmp[0]) > seqID) //If it is a newer packet 
                {
                    //Update class to most recent values 
                    seqID = Int32.Parse(tmp[0]);
                    xAccel = (float.Parse(tmp[1]));
                    yAccel = (float.Parse(tmp[2]));
                    zAccel = (float.Parse(tmp[3]));
                    w = (float.Parse(tmp[4]));
                    x = (float.Parse(tmp[5]));
                    y = (float.Parse(tmp[6]));
                    z = (float.Parse(tmp[7]));

                    if (MODE == packetICD.IMU_Mode.CHEST)
                    {
                        //if (count % 200 == 0)
                        //    DebugManager.Instance.LogUnityConsole("Chest IMU, xAccel: " + xAccel + " yAccel: " + yAccel + " zAccel: " + zAccel + " QuaW: " + w + " QuaX: " + x + " Quay: " + y + " QuaZ " + z);
                    } else
                    {
                        if (count % 200 == 0)
                            DebugManager.Instance.LogUnityConsole("Glove IMU, xAccel: " + xAccel + " yAccel: " + yAccel + " zAccel: " + zAccel + " QuaW: " + w + " QuaX: " + x + " Quay: " + y + " QuaZ " + z);
                    }
                    count++;
                    imuDataSmoothing();
                }
            }
        }
        catch (FormatException e)
        {
            DebugManager.Instance.LogUnityConsole(e.Message);
            return -1;
        }

        return 1;
    }

    private void imuDataSmoothing()
    {
        //For now this function simply takes the freshest packet and updates frame with it but custom smoothing logic inserts here 
        Quaternion newData = new Quaternion(x, y, z, w);
        newData = newData.normalized;
        

        if(!startValSet)
        {
            startingVal = newData;
            prevVal = newData;
            startValSet = true;
        }
        else
        {
            newData = newData * Quaternion.Inverse(startingVal); //subtract off default orientation
            

            //Here we need to check if it is greater then prev 
            //Debug.Log("DIFF = " + Quaternion.Angle(newData, prevVal));
            //Debug.Log("START = " + startingVal);
            

            if(Quaternion.Angle(newData, prevVal) > rotThreshold) //catches the twitches 
            {
                
                //Debug.Log("Twitch"); 
                
            } else {

                //Note still might need to do somme fucking around to get the coordinate system to line up but hopefully not. 

                if (MODE == packetICD.IMU_Mode.CHEST)
                {
                    HeadLockScript.Instance.updateHIDwithIMU(newData.w, newData.y, -newData.z, -newData.x); 
                }
                else if(MODE == packetICD.IMU_Mode.GLOVE)
                {
                    //new Quaternion(x,-y,-z,w);
                    ML.updateModelwithIMU(newData.w,newData.x,-newData.y,-newData.z, xAccel, yAccel, zAccel); 
                }
            }

            prevVal = newData; 


        }
        //In here the logic for which values get sent also need to be adjusted based on default. For instance the chest is tilted so the axis are all wrong 
    }
}