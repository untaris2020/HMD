using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class IMUHandler : tcpPacket
{
    public static int QUEUE_SIZE = 10;

    private delegate void functionDelegate();

    //IMU Packet Information 
    private int seqID; 
    private Queue x;
    private Queue y;
    private Queue z;
    private Queue xGyro;
    private Queue yGyro;
    private Queue zGyro;

    IMUHandler()
    { 
        //Fixme -- I need to update this to values that are guaranteed not to be produced from IMU 
        seqID = -1;
        x = new Queue();
        y = new Queue();
        z = new Queue();
        xGyro = new Queue();
        yGyro = new Queue();
        zGyro = new Queue();
    }

    ~IMUHandler()
    {
        functionDebug.Instance.deregisterFunction("chestIMUstart");
        functionDebug.Instance.deregisterFunction("chestIMUstop");
    }

    public override void initialize(TcpClient client)
    {
        base.initialize(client);
        functionDelegate stopDelegate = new functionDelegate(stopStream);
        functionDelegate startDelegate = new functionDelegate(startStream);
        functionDebug.Instance.registerFunction("chestIMUstart", startDelegate);
        functionDebug.Instance.registerFunction("chestIMUstop", stopDelegate);\
    }

    public override int processPacket(string packet)
    {
        Debug.Log("MSG FOR IMU: " + packet);

        string[] tmp = packet.Split(new string[] { "$" }, StringSplitOptions.None);
        try
        {
            if (tmp.Length == 7)
            {               
                //If seqID has overflown we need to reset
                if(seqID >= 2147483647)
                {
                    seqID = -1;
                }

                if (Int32.Parse(tmp[0]) > seqID) //If it is a newer packet 
                {
                    //Update class to most recent values 
                    seqID = Int32.Parse(tmp[0]);
                    x.Enqueue(float.Parse(tmp[1]));
                    y.Enqueue(float.Parse(tmp[2]));
                    z.Enqueue(float.Parse(tmp[3]));
                    xGyro.Enqueue(float.Parse(tmp[4]));
                    yGyro.Enqueue(float.Parse(tmp[5]));
                    zGyro.Enqueue(float.Parse(tmp[6]));

                    //Dequeue old elements 
                    if (x.Count > QUEUE_SIZE)
                        x.Dequeue();
                    if (y.Count > QUEUE_SIZE)
                        y.Dequeue();
                    if (z.Count > QUEUE_SIZE)
                        z.Dequeue();
                    if (xGyro.Count > QUEUE_SIZE)
                        xGyro.Dequeue();
                    if (yGyro.Count > QUEUE_SIZE)
                        yGyro.Dequeue();
                    if (zGyro.Count > QUEUE_SIZE)
                        zGyro.Dequeue();
                }
            }
        }
        catch(FormatException e)
        {
            DebugManager.Instance.LogUnityConsole(e.Message);
            return -1;
        }
       
        return 1;
    }
    public override void stopStream()
    {
        streaming = false;
        sendMsg("STOP");
    }
    public override void startStream()
    {
        Debug.Log("Starting stream");
        streaming = true;
    }
}