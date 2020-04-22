using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class tcpPacket:MonoBehaviour
{
    
    /// <summary>
    /// Private data types
    /// Connected value if stream is active
    /// Cli data type that stores active socket connection
    /// </summary>
    
    protected bool connected;
    protected TcpClient cli;
    protected int seqID;
    protected string debugName;
    protected bool setParam = true; 

    protected void Start()
    {
        setParam = true;
    }


    public virtual void initialize(TcpClient client)
    {
        seqID = -1;
        cli = client;
        connected = true;
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
                Debug.Log("Stream Write Returned");
            }
        }
        catch (SocketException socketException)
        {
            DebugManager.Instance.LogBoth("Socket exception: " + socketException);
            return -1;
        }
        return 1;
    }
    public virtual void stopStream()
    {
        sendMsg("STOP");
    }

    /// <summary>
    /// Uncomment this function in start if you want it to stream right away. 
    /// </summary>
    public virtual void startStream()
    {
        sendMsg("START");
    }

    public void reportStatus()
    {
        //Debug.Log("SETTING PARAM FOR " + debugName);
        string report; 
        if(connected)
        {
            report = "CON";
        }
        else
        {
            report = "D-CON";
        }
        DebugManager.Instance.SetParam(debugName, report);
    }

    #region setters and getters
    public bool getConnected() { return connected; }
    public TcpClient getCli() { return cli; }

    public void setConnected(bool connect) { connected = connect; }
    public void setCli(TcpClient client) { cli = client; }
    #endregion
}


