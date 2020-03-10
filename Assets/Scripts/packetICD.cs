using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class packetICD : MonoBehaviour
{
    public enum Type
    {
        SYSTEM = 0,
        CHEST_IMU = 1,
        GLOVE_IMU = 2,
        TOGGLE_SCREEN = 3, 
        HEAD_CAM = 4, 
        GLOVE_CAM = 5, 
        FORCE_SENSOR = 6
    }
    
    public struct Header
    {
        public Type type;   //Type of data packet
        public uint size;   //size of data packet in bits 
        public uint time;   //Unix epoch time
        public uint seqID;  //Sequence ID of the packet
    };


}




public class tcpPacket
{
    public static int QUEUE_SIZE = 10;

    /// <summary>
    /// Private data types
    /// streaming if data should be sent 
    /// Connected value if stream is active
    /// Cli data type that stores active socket connection
    /// </summary>
    private bool streaming;
    private bool connected;
    private TcpClient cli;

    /// <summary>
    /// Default constructor 
    /// </summary>
    /// <param name="client"></param>
    public tcpPacket(TcpClient client)
    {
        streaming = false;
        connected = false;
        cli = client;
    }

    /// <summary>
    /// processPacket virtual function that must be override to parse TCPPacket
    /// </summary>
    /// <returns>returns -1 until implemented</returns>
    public virtual int processPacket(string packet)
    {
        return -1; 
    }

    /// <summary>
    /// sendMsg function that sends a message to the connected client if there is one. 
    /// </summary>
    /// <param name="message"></param>
    /// <returns>returns -1 on failure 1 on success</returns>
    public virtual int sendMsg(string message)
    {
        if (cli == null)
        {
            return -1;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = cli.GetStream();
            if (stream.CanWrite)
            {
                string serverMsg = "<BEG>" + message + "<EOF>";
                // Convert string message to byte array.                 
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(message);
                // Write byte array to socketConnection stream.               
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                DebugManager.Instance.LogUnityConsole("Server sent his message - should be received by client");
            }
        }
        catch (SocketException socketException)
        {
            DebugManager.Instance.LogBoth("Socket exception: " + socketException);
            return -1;
        }
        return 1;
    }

    #region setters and getters
    public bool getStreaming() { return streaming; }
    public bool getConnected() { return connected; }
    public TcpClient getCli() { return cli; }

    public void setStreaming(bool stream) { streaming = stream; }
    public void setConnected(bool connect) { connected = connect; }
    public void setCli(TcpClient client) { cli = client; }
    #endregion
}


#region CHEST IMU
public class chestIMU : tcpPacket
{
    //IMU Packet Information 
    private int seqID; 
    private Queue x;
    private Queue y;
    private Queue z;
    private Queue xGyro;
    private Queue yGyro;
    private Queue zGyro;

    public chestIMU(TcpClient client) : base(client)
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

    public override int processPacket(string packet)
    {
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
}
#endregion