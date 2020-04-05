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

public class forceSensorManager : MonoBehaviour
{
    private HeadTracking headTrack;
     
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;

    private bool forceSensorPress;
    private bool debug;
    private string msg;
    private bool connection; 

    private SceneMan scene;

    EndPoint Remote; 

    byte[] data = new byte[1024];

    public int port = 6080;

    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        scene = (GameObject.Find("SceneManager")).GetComponent(typeof(SceneMan)) as SceneMan;
        headTrack = (GameObject.Find("SceneManager")).GetComponent(typeof(HeadTracking)) as HeadTracking;
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
        forceSensorPress = false;
        debug = false;
        connection = false; 
    }

    void Update()
    {
        if(connection)
        {
            DebugManager.Instance.SetParam("hand_FS", "CON");

            connection = false;
        }
        if(debug)
        {
            DebugManager.Instance.LogBoth(this.GetType().Name, msg);
            msg = "";
            debug = false;
        }
        if(forceSensorPress)
        {
            headTrack.forceClick();
            forceSensorPress = false;
        }
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
                        
                        msg = "Connection received from force Sensor...";
                        debug = true;
                      
                        int length;
                        // Read incomming stream into byte arrary. 						
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message.
                            string clientMessage = Encoding.ASCII.GetString(incommingData);;

                            if(clientMessage.CompareTo("PRESS") == 0)
                            {
                                forceSensorPress = true;
                            }


                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }
}
