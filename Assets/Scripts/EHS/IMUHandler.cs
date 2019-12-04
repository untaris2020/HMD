using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine.Networking;
using System.Timers;

using System;
using System.IO;

// State object for reading client data asynchronously  
public class StateObject {  
    // Client  socket.  
    public Socket workSock = null;  
    // Size of receive buffer.  
    public const int BufferSize = 1024;  
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];  
// Received data string.  
    public StringBuilder sb = new StringBuilder();    
}

public class IMUHandler : MonoBehaviour
{
    private HeadLockScript headlock;

    private Socket udpServer; 

    private System.Timers.Timer aTimer;
    private SceneManager scene;

    EndPoint Remote; 

    byte[] data = new byte[1024];

    private IMUJson imuJson;

    public string IP = "192.168.1.123";
    public int port = 5070;
    public int TIMEOUT = 2000;


     ~IMUHandler()
     {
        udpServer.Dispose();
     }

    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        scene = (GameObject.Find("SceneManager")).GetComponent(typeof(SceneManager)) as SceneManager;
        createUdpServer();
    }

    void Update()
    {
        if (udpServer.Available > 0) // Only read if we have some data 
        {                           // queued in the network buffer. 
            int recv = udpServer.ReceiveFrom(data, ref Remote);

            //This contains the full package but could be multiple
            string message = (Encoding.ASCII.GetString(data, 0, recv));

            string[] packets = message.Split('}');
            string packet = packets[0];

            //Appending } back again 
            packet = packet + "}";

            Debug.Log("PACKET RECEIVED");

            //Now we need to convert to a json from a string
            IMUJson imuJsonPacket = JsonUtility.FromJson<IMUJson>(packet);

            updateScene(imuJsonPacket);
        }
    }


    void createUdpServer()
    {
        IPAddress IPAddr = IPAddress.Parse(IP);
        IPEndPoint endPoint = new IPEndPoint(IPAddr, port);

        udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try
        {
            udpServer.Bind(endPoint);
            Debug.Log("Listening for client...");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            Remote = (EndPoint)(sender);

        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

   
    public void updateScene( IMUJson imuJsonPacket)
    {
        try
        {
            headlock = scene.getHeadLockScriptInstance();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        if (headlock != null)
        {
            if (headlock.updateHIDwithIMU(imuJsonPacket))
            {
                Debug.Log("ERROR");
            }
        }
    }


    //public void recieveFromDevice(IAsyncResult ar)
    //{
    //    String content = string.Empty;

    //    StateObject state = (StateObject)ar.AsyncState;

    //    Socket tempCli = state.workSock;

    //    int bytesRead = tempCli.EndReceive(ar);

    //    if (bytesRead > 0)
    //    {
    //        state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
            
    //        //This contains the full package but could be multiple
    //        content = state.sb.ToString();

    //        string[] packets = content.Split('}');
    //        string packet = packets[0];

    //        //Appending } back again 
    //        packet = packet + "}";

    //        Debug.Log("PACKET RECEIVED");

    //        //Now we need to convert to a json from a string
    //        IMUJson imuJsonPacket = JsonUtility.FromJson<IMUJson>(packet);

    //        //updateScene(imuJsonPacket);
    //        sceneUpdate = true;
    //        imuJson = imuJsonPacket; 

    //        state.sb.Clear();

    //        Debug.Log("Starting begin receive");
    //        tempCli.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(recieveFromDevice), state);
    //        Debug.Log("Ending begin receive");
    //    }
    //}

   
}



[System.Serializable]
public class IMUJson
{
    public float xAccel;
    public float yAccel;
    public float zAccel;
    public float wQuan;
    public float xQuan;
    public float yQuan;
    public float zQuan;
}


