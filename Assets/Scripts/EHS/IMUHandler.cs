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

public class IMUHandler : MonoBehaviour
{
    private HeadLockScript headlock;
     
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;

    private bool updateSceneFlag;
    private bool debug;
    private string msg;
    private bool connection;
    private bool updateGloveData;
    private bool watchDogTimer; 

    private long packetCount;
    
    private IEnumerator coroutine;

    private System.Timers.Timer aTimer;
    private SceneManager scene;

    EndPoint Remote; 

    byte[] data = new byte[1024];

    private IMUJson currimuJsonPacket;

    public int port = 5070;
    public int TIMEOUT = 2000;
    private int recv;

    public enum subSystem
    {
        CHEST = 1,
        GLOVE = 2, 
    }

    public subSystem EHS_SUB_SYSTEM; 
    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        scene = (GameObject.Find("SceneManager")).GetComponent(typeof(SceneManager)) as SceneManager;
        headlock = (GameObject.Find("HID")).GetComponent(typeof(HeadLockScript)) as HeadLockScript;
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
        updateSceneFlag = false;
        connection = false;
        debug = false;
        updateGloveData = false;
        watchDogTimer = true; 
        packetCount = 0;

        coroutine = diconnectEvent(2.0f);
        StartCoroutine(coroutine);
    }

    void Update()
    {
        if(updateSceneFlag)
        {
            watchDogTimer = true;
            updateScene(currimuJsonPacket);
            updateSceneFlag = false;
            
        }
        if(connection)
        {
            if(EHS_SUB_SYSTEM == subSystem.CHEST)
            {
                DebugManager.Instance.SetParam("chest_IMU", "CON");
            }
            else if(EHS_SUB_SYSTEM == subSystem.GLOVE)
            {
                DebugManager.Instance.SetParam("hand_IMU", "CON");
            }
            
            connection = false;
        }
        if(EHS_SUB_SYSTEM == subSystem.CHEST)
        {
            DebugManager.Instance.SetParam("chest_IMU_PC", packetCount.ToString());
        }
        else if(EHS_SUB_SYSTEM == subSystem.GLOVE)
        {
            DebugManager.Instance.SetParam("hand_IMU_PC", packetCount.ToString());
        }
        if(debug)
        {
            DebugManager.Instance.LogBoth(this.GetType().Name, msg);
            msg = "";
            debug = false;
        }
        if(updateGloveData)
        {
            DebugManager.Instance.SetParam("w", currimuJsonPacket.wQuan.ToString("0.0000"));
            DebugManager.Instance.SetParam("x", currimuJsonPacket.xQuan.ToString("0.0000"));
            DebugManager.Instance.SetParam("y", currimuJsonPacket.yQuan.ToString("0.0000"));
            DebugManager.Instance.SetParam("z", currimuJsonPacket.zQuan.ToString("0.0000"));
        }
    }


    private IEnumerator diconnectEvent(float waitTime)
    {
        if(!watchDogTimer)
        {
            DebugManager.Instance.LogBoth(this.GetType().Name, "IMU DISCONNECTED...");

            //Disconnection event 
            tcpListenerThread.Abort();
            tcpListener.Stop();
            connectedTcpClient.Close();

            //resarting thread
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();

        }
        watchDogTimer = false;
        yield return new WaitForSeconds(waitTime);
    }

     // Runs in background TcpServerThread; Handles incomming TCPClient requests 	
    private void ListenForIncommingRequests()
    {
        try
        {
            // Create listener on localhost. 			
            tcpListener = new TcpListener(IPAddress.Parse(scene.IP), port);
            tcpListener.Start();
            Byte[] bytes = new Byte[20000];
            while (true)
            {
                using(connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    //Connected here
                    connection = true; 
                    // Get a stream object for reading 					
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        
                        msg = "Connection received from IMU...";
                        debug = true;
                      
                        int length;
                        // Read incomming stream into byte arrary. 						
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            //var incommingData = new byte[length];
                           // Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message.
                            string clientMessage = Encoding.UTF8.GetString(bytes);;

                            string[] packets = clientMessage.Split('}');

                            if(packets.Length > 0)
                            {
                                string packet = packets[0];

                                //Appending } back again 
                                packet = packet + "}";

                                packetCount++;

                                //Now we need to convert to a json from a string
                                currimuJsonPacket = JsonUtility.FromJson<IMUJson>(packet);

                                if(EHS_SUB_SYSTEM == subSystem.CHEST)
                                {
                                    updateSceneFlag = true; 
                                }
                                else if(EHS_SUB_SYSTEM == subSystem.GLOVE)
                                {
                                    updateGloveData = true; 
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            msg = "SocketException " + socketException.ToString();
            debug = true; 
            //Debug.Log("SocketException " + socketException.ToString());
        }
    }
   
    public void updateScene( IMUJson imuJsonPacket)
    {
        if (headlock != null)
        {
            if (headlock.updateHIDwithIMU(imuJsonPacket))
            {
                Debug.Log("ERROR");
            }
        }
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


