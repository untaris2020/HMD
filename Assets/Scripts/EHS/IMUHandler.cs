using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine.Networking;


using System;
using System.IO;

public class IMUHandler : MonoBehaviour
{
    private TcpListener tcpListener2; 
    private Thread tcpListenerThread2;  	
    private TcpClient connectedTcpClient2;
    private HeadLockScript headlock;
    private GameObject hid = null;

    private GameObject sceneMang;
    private SceneManager scene;

    public delegate int MyDelegate();

    private bool connected; 

    public string IP = "127.0.0.1";
    public int port = 5070;

    // Start is called before the first frame update
// Use this for initialization
	void Start () {
        // Start TcpServer background thread 		
        scene = (GameObject.Find("SceneManager")).GetComponent(typeof(SceneManager)) as SceneManager;
        connected = false; 
        createTcpServer();
	}  	
	
	
	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests2 () { 		
		try { 			
			// Create listener on localhost port 5070. 			
			tcpListener2 = new TcpListener(IPAddress.Parse(IP), port); 			
			tcpListener2.Start();              
			Debug.Log("Server is listening");      
			Byte[] bytes = new Byte[1024];  			
			while (true) { 	
				using (connectedTcpClient2 = tcpListener2.AcceptTcpClient()) { 	
                    Debug.Log("Client Connection"); 	
                    //Log the connection here... 

					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient2.GetStream()) { 						
						int length;
                        // Read incomming stream into byte arrary. 				

                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
                            var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 	

							string clientMessage = Encoding.UTF8.GetString(bytes,0,length);
                            //This contains the full package but could be multiple

                            string[] packets = clientMessage.Split('}');
                            string packet = packets[0];
                            
                            //Appending } back again 
                            packet = packet + "}";

                            //Now we need to convert to a json from a string
                            IMUJson imuJsonPacket = JsonUtility.FromJson<IMUJson>(packet);

                            
                            //hid = GameObject.Find("HID");
   
                            //if(hid != null)
                            //{
                            //    headlock = hid.GetComponent(typeof(HeadLockScript)) as HeadLockScript;

                            //    //Packet is done and now we need to call the update HID function 
                            //    headlock.updateHIDwithIMU(imuJsonPacket);
                            //}


						} 					
					} 				
				} 			
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}   
	}  	
    void createTcpServer()
    {
        tcpListener2 = new TcpListener(IPAddress.Parse(IP), port); 			
		tcpListener2.Start();              
		Debug.Log("Server is listening");

        MyDelegate d = new MyDelegate(recieveFromDevice);

        //Register socket to scene manager and wait to accept

        scene.RegisterDevice(tcpListener2.Server, "CHEST IMU", false, d);
        while (!connected)
        {
            using (connectedTcpClient2 = tcpListener2.AcceptTcpClient())
            {
                connected = connectedTcpClient2.Connected;
                Debug.Log("Client Connected: " + connected);
            }
        }

    }

    public int recieveFromDevice()
    {
        Byte[] bytes = new Byte[1024];  	

        

        if (connected)
        {
            using (NetworkStream stream = connectedTcpClient2.GetStream())
            {
                int length;
                // Read incomming stream into byte arrary. 				

                while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    var incommingData = new byte[length];
                    Array.Copy(bytes, 0, incommingData, 0, length);
                    // Convert byte array to string message. 	

                    string clientMessage = Encoding.UTF8.GetString(bytes, 0, length);
                    //This contains the full package but could be multiple

                    string[] packets = clientMessage.Split('}');
                    string packet = packets[0];

                    //Appending } back again 
                    packet = packet + "}";

                    Debug.Log("Packet: " + packet);
                    //Now we need to convert to a json from a string
                    IMUJson imuJsonPacket = JsonUtility.FromJson<IMUJson>(packet);


                }
            }
        }
        return 0;
    }
}


[System.Serializable]
public class IMUJson
{
    public float xAccel;
    public float yAccel;
    public float zAccel;
    public float xGyro;
    public float yGyro;
    public float zGyro;
}


