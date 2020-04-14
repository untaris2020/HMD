using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class IMUHandler : tcpPacket
{
    public packetICD.IMU_Mode MODE;

    private string NAME; 

    //IMU Packet Information 
    private float x;
    private float y;
    private float z;
    private float w;
    private float xAccel;
    private float yAccel;
    private float zAccel; 

    private delegate void functionDelegate();

    public void start()
    {
        seqID = -1; 
        connected = false;
        if (MODE == packetICD.IMU_Mode.CHEST)
        {
            NAME = "chestIMU";
        }
        else
        {
            NAME = "chestIMU";
        }
        functionDelegate stopDelegate = new functionDelegate(stopStream);
        functionDelegate startDelegate = new functionDelegate(startStream);
        functionDebug.Instance.registerFunction(NAME + "start", startDelegate);
        functionDebug.Instance.registerFunction(NAME + "stop", stopDelegate);
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
         
        //In here the logic for which values get sent also need to be adjusted based on default. For instance the chest is tilted so the axis are all wrong 
        if(MODE == packetICD.IMU_Mode.CHEST)
        {
            //In here we need to convert the IMU data into the proper format 




            HeadLockScript.Instance.updateHIDwithIMU(w,x,y,z);
        }
        else if(MODE == packetICD.IMU_Mode.GLOVE)
        {
            //For now do nothing 
        }
       

    }
}