using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;  

public class TCPServer : MonoBehaviour {  	
	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener; 
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;  	
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient;

    private bool rstSvr; 
    #endregion


    #region public members
    public static string IPAddr = "127.0.0.1";
    public static string port = "6002"; 
    #endregion

    // Use this for initialization
    void Start () {
        rstSvr = false; 
		// Start TcpServer background thread 		
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start(); 	
        
	}  	
	
	// Update is called once per frame
	void Update () { 		
	    if(rstSvr)
        {
            restartServer();
        }
	}  	
	
	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests () { 		
		try { 			
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse(IPAddr), Int32.Parse(port)); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");              
			Byte[] bytes = new Byte[1024];  			
			while (true) { 				
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 						
						// Read incomming stream into byte arrary. 	
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            Debug.Log("TEST");
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);  							
							// Convert byte array to string message. 							
							string clientMessage = Encoding.ASCII.GetString(incommingData); 							
							Debug.Log("client message received as: " + clientMessage); 						
						} 					
					} 				
				} 			
			} 		
		} 		
		catch (Exception exception)
        { 			
			Debug.Log("SocketException 2" + exception.ToString());
            rstSvr = true; 
		}     
	}

    /// <summary>
    /// close disconnected thread and reset the server in a new thread 
    /// </summary>
    private void restartServer()
    {
        Debug.Log("Restarting Server...");
        Debug.Log("Closing Client Socket...");
        connectedTcpClient.Close();
        Debug.Log("Stoping TCPListener...");
        tcpListener.Stop();
        Debug.Log("Lastly, Closing Thread...");
        tcpListenerThread.Abort();
        Debug.Log("Opening new listing process...");
        tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start();
        Debug.Log("Server Restarted...");
        rstSvr = false; 
    }

	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	private void SendMessage()
    { 		
		if (connectedTcpClient == null)
        {             
			return;         
		}  		
		
		try
        { 			
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite)
            {                 
				string serverMessage = "This is a message from your server."; 			
				// Convert string message to byte array.                 
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage); 				
				// Write byte array to socketConnection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
				Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException)
        {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
	} 
}