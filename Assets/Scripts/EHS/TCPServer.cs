using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text;
using System.Text.RegularExpressions;
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
    private bool streaming;
    private bool reqSent; 
    #endregion

    protected struct IMUMsg
    {
        public int seqID;
        public int x;
        public int y;
        public int z;
        public int xGyro;
        public int yGyro;
        public int zGyro;
    }

    #region public members
    public static string IPAddr = "127.0.0.1";
    public static string port = "6002";
    #endregion

    // Use this for initialization
    void Start() 
    {
        rstSvr = false;
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
        streaming = false;
        reqSent = false; 
        startStream();
    }

    // Update is called once per frame
    void Update() 
    {
        if (rstSvr)
        {
            restartServer();
        }
    }

    public void startStream()
    {
        streaming = true; 
    }
	
    public void stopStream()
    {
        streaming = false;
        SendMessage("STOP");
        restartServer();
        reqSent = false; 
    }

	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests () { 		
		try { 			 			
			tcpListener = new TcpListener(IPAddress.Parse(IPAddr), Int32.Parse(port)); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");              
			Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    //Connected -- wait for start msg from external source  
                    if (streaming)
                    {

                        //Start stream if not started 

                        if (!reqSent)
                        {
                            SendMessage("START");
                            reqSent = true; 
                        }

                        // Get a stream object for reading 					
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;
                            // Read incomming stream into byte arrary. 	
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incommingData = new byte[length];
                                Array.Copy(bytes, 0, incommingData, 0, length);
                                // Convert byte array to string message. 							
                                string clientMessage = Encoding.ASCII.GetString(incommingData);


                                //Process client string here 

                                //First I need to get my first <BEG> and my last full <EOF> so that I don't break

                                int idx = clientMessage.IndexOf("<BEG>");

                                if (idx != -1)
                                {
                                    clientMessage = clientMessage.Substring(idx);
                                    idx = clientMessage.LastIndexOf("<EOF>");
                                    if (idx != -1)
                                    {
                                        clientMessage = clientMessage.Substring(0, idx);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                Debug.Log("Formatted Msg: " + clientMessage);

                                //Every string will be wrapped in <BEG><EOF> tags so grab those to begin
                                string[] packets = clientMessage.Split(new string[] { "<EOF>" }, StringSplitOptions.None);

                                var messages = new List<string>();

                                foreach (var pack in packets)
                                {

                                    idx = pack.IndexOf("<BEG>");
                                    if (idx != -1)
                                    {
                                        messages.Add(pack.Substring(idx + 5));
                                    }
                                    else
                                    {
                                        //its the full message already 
                                        messages.Add(pack);
                                    }
                                }

                                //Now all that is contained is the data in the message itself --Note this section is what will change based on the format
                                int tmpSeqID = -1; //temp seq ID that stores the most up to date seq 
                                IMUMsg imuPacket = new IMUMsg();
                                foreach (var msg in messages)
                                {

                                    string[] tmp = msg.Split(new string[] { "$" }, StringSplitOptions.None);
                                    if (tmp.Length == 7)
                                    {
                                        if (Int32.Parse(tmp[0]) > tmpSeqID) //If it is a newer packet 
                                        {
                                            tmpSeqID = Int32.Parse(tmp[0]); //Update the current newest value 

                                            //Assign temp packet 
                                            IMUMsg tempPkt = new IMUMsg();
                                            tempPkt.seqID = Int32.Parse(tmp[0]);
                                            tempPkt.x = Int32.Parse(tmp[1]);
                                            tempPkt.y = Int32.Parse(tmp[2]);
                                            tempPkt.z = Int32.Parse(tmp[3]);
                                            tempPkt.xGyro = Int32.Parse(tmp[4]);
                                            tempPkt.yGyro = Int32.Parse(tmp[5]);
                                            tempPkt.zGyro = Int32.Parse(tmp[6]);

                                            imuPacket = tempPkt; //assign the most up to date packet 
                                        }
                                    }
                                }
                                //Data grabbed at this point hypothetically 
                                Debug.Log("Packet #" + imuPacket.seqID + " received");
                            }
                        }
                    }
                }
            }
		} 		
		catch (Exception IOException)
        { 			
			Debug.Log("SocketException 2" + IOException.ToString());
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
        reqSent = false; 
    }

	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	private void SendMessage(string message)
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
                string serverMsg = "<BEG>" + message + "<EOF>";
				// Convert string message to byte array.                 
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(message); 				
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