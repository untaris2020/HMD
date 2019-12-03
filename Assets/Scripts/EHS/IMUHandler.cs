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
    //private GameObject hid = null;

    private Socket tcpClient;
    private Socket tcpServer; 

    private System.Timers.Timer aTimer;
    private GameObject sceneMang;
    private SceneManager scene;

    private bool sceneUpdate;
    private IMUJson imuJson;

    public string IP = "192.168.1.123";
    public int port = 5070;
    public int TIMEOUT = 2000;

    // Start is called before the first frame update
    // Use this for initialization
    void Start() {

        scene = (GameObject.Find("SceneManager")).GetComponent(typeof(SceneManager)) as SceneManager;
    
        sceneUpdate = false; 

        aTimer = new System.Timers.Timer(TIMEOUT);
        aTimer.Elapsed += onTimeout;
        aTimer.AutoReset = true;
        aTimer.Enabled = false;

        createTcpServer();
    }

    void Update()
    {
        if (sceneUpdate)
        {
            sceneUpdate = false;

            try
            {
                updateScene(imuJson);
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
            }

        }
    }


    void createTcpServer()
    {
        IPAddress IPAddr = IPAddress.Parse(IP);
        IPEndPoint endPoint = new IPEndPoint(IPAddr, port);

        tcpServer = new Socket(IPAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            tcpServer.Bind(endPoint);
            tcpServer.Listen(10);

            tcpServer.BeginAccept(new AsyncCallback(asyncHandler), tcpServer);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void asyncHandler(IAsyncResult ar)
    {
        Socket tempSrv = (Socket)ar.AsyncState;
        Socket tempCli = tempSrv.EndAccept(ar);
        tcpClient = tempCli;

        StateObject state = new StateObject();
        state.workSock = tempCli;

        aTimer.Enabled = true;

        tempCli.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(recieveFromDevice), state);
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


    public void recieveFromDevice(IAsyncResult ar)
    {
        String content = string.Empty;

        StateObject state = (StateObject)ar.AsyncState;

        Socket tempCli = state.workSock;

        int bytesRead = tempCli.EndReceive(ar);

        if (bytesRead > 0)
        {
            state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
            
            //This contains the full package but could be multiple
            content = state.sb.ToString();

            string[] packets = content.Split('}');
            string packet = packets[0];

            //Appending } back again 
            packet = packet + "}";

            //Now we need to convert to a json from a string
            IMUJson imuJsonPacket = JsonUtility.FromJson<IMUJson>(packet);

            //updateScene(imuJsonPacket);
            sceneUpdate = true;
            imuJson = imuJsonPacket; 

            state.sb.Clear();

            Debug.Log("Starting begin receive");
            tempCli.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(recieveFromDevice), state);
            Debug.Log("Ending begin receive");
        }
    }

    public void onTimeout(System.Object source, ElapsedEventArgs e)
    {
        if(!(SocketExtensions.IsConnected(tcpClient)))
        {
            Debug.Log("Detected Disconnection");
            aTimer.Enabled = false;
            tcpServer.Dispose();
            createTcpServer();
        }
    }
}

static class SocketExtensions
{
    public static bool IsConnected(this Socket socket)
    {
    try
    {
        return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
    }
    catch (SocketException) { return false; }
    }
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


