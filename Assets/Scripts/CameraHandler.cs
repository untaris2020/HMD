using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using UnityEngine.UI;
using System.IO;

public class CameraHandler : MonoBehaviour
{
    private TcpListener tcpListener; 
    private Thread tcpListenerThread;  	
    private TcpClient connectedTcpClient;

    public RawImage FrontCameraImage;
    public RawImage RearCameraImage;
    private byte[] pcxFile;
    private Texture2D target;

    // Start is called before the first frame update
    // Use this for initialization
    void Start () { 		
		// Start TcpServer background thread 		
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start(); 	
	}  	
	
	// Update is called once per frame
	void Update () { 		
		    	
	}  	
	
	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests () { 		
		try { 			
			// Create listener on localhost port 5060. 			
			tcpListener = new TcpListener(IPAddress.Parse("192.168.1.114"), 5060); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");              
			Byte[] bytes = new Byte[1024];  			
			while (true) { 	
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 	
                    Debug.Log("Client Connection"); 	
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 						
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);  							
							// Convert byte array to string message. 							

							string clientMessage = Encoding.UTF8.GetString(incommingData); 							
							Debug.Log("client message received as: " + clientMessage); 					

                            pcxFile = File.ReadAllBytes("Assets/5_ImageParser/bagit_icon.pcx");
                            //int startPoint = 128;
                            int height = 240; 
                            int width = 320; 
                            target = new Texture2D(height, width);
                            target.LoadRawTextureData(pcxFile);
                            target.Apply();
                            // target.EncodeToJPG();

                            FrontCameraImage.texture = target;
						} 					
					} 				
				} 			
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}     
	}  	

}