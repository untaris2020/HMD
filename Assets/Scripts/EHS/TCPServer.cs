using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;


public class TCPServer : MonoBehaviour
{
    // Setting IP and PORT
    public string IP = "192.168.1.123";
    public int PORT = 6006;

    // Setting Byte Size Parameters
    private int minByteSize = 6000;
    private int maxByteSize = 30000;

    // TCPListener to listen for incomming TCP connection requests.
    private TcpListener tcpListener;

    // Background thread for TcpServer workload.
    private Thread tcpListenerThread;

    // Create handle to connected tcp client.
    private TcpClient connectedTcpClient;

    private bool rstSvr;
    private bool streaming;
    private bool reqSent; 

    private delegate void funcDelegate();

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

    // Use this for initialization
    void Start()
    {
        rstSvr = false;
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();	
        streaming = false;
        reqSent = false; 

        funcDelegate tmpDelegate0 = new funcDelegate(startStream);
        funcDelegate tmpDelegate1 = new funcDelegate(stopStream);
        functionDebug.Instance.registerFunction("start",tmpDelegate0);
        functionDebug.Instance.registerFunction("stop",tmpDelegate1);

        //Note: Uncomment the line below if the script is always active. Otherwise the controlling script will call this method to begin streaming
        //startStream();
    }

    public void startStream()
    {
        
        streaming = true; 
    }

    public void stopStream()
    {
        streaming = false;
        SendMsg("STOP");
        //restartServer();
        reqSent = false; 
    }

    // Runs in background TcpServerThread; Handles incomming TCPClient requests
    private void ListenForIncommingRequests()
    {
        try
        {
            // Create listener on localhost.
            tcpListener = new TcpListener(IPAddress.Parse(IP), PORT);
            tcpListener.Start();
            Debug.Log("Server is listening\n");
            Byte[] bytes = new Byte[maxByteSize];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    if (streaming)
                    {
                        if (!reqSent)
                        {
                            SendMsg("START");
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

                                /*
                                 * 
                                 * Insert Packet processing here 
                                 *               
                                */
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
    private void restartServer()
    {
        connectedTcpClient.Close();
        tcpListener.Stop();
        tcpListenerThread.Abort();
        tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start();
        rstSvr = false;
        reqSent = false; 
    }

     /// <summary>
     /// Send message function that takes in a message string and sends to active client if one is available
     /// </summary>
     /// <param name="message"></param>
    private void SendMsg(string message)
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
}// END Server Class
