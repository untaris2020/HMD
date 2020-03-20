using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class tcpPacket
{
    
    /// <summary>
    /// Private data types
    /// Connected value if stream is active
    /// Cli data type that stores active socket connection
    /// </summary>
    
    private bool connected;
    protected TcpClient cli;

    /// <summary>
    /// Default constructor 
    /// </summary>
    /// <param name="client"></param>
    public tcpPacket(TcpClient client)
    {
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
            Debug.Log("SEND MSG: CLient is null\n");
            return -1;
        }
        try
        {
            Debug.Log("Sending");
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
    public virtual void stopStream()
    {
        return;
    }

    /// <summary>
    /// Uncomment this function in start if you want it to stream right away. 
    /// </summary>
    public virtual void startStream()
    {
        return;
    }

    #region setters and getters
    public bool getConnected() { return connected; }
    public TcpClient getCli() { return cli; }

    public void setConnected(bool connect) { connected = connect; }
    public void setCli(TcpClient client) { cli = client; }
    #endregion
}


