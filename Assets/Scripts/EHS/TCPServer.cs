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

#region TCPServer Class
public class TCPServer : MonoBehaviour
{
    #region Public Variables
    public string IP;
    public int PORT;
    #endregion

    #region Private Variables
    private int maxByteSize = 30000; // Setting Byte Size Parameters
    private TcpListener tcpListener; // TCPListener to listen for incomming TCP connection requests.
    private Thread tcpListenerThread; // Background thread for TcpServer workload.
    private TcpClient tempTcpClient;
    private packetICD icd; //Packet ICD Script
    // Create handle to connected tcp client.
    private bool streaming;
    private bool reqSent;
   
    private delegate void funcDelegate1();
    private delegate void funcDelegate2();
    #endregion

    public chestIMU chest_IMU; 


    #region Unity Methods
    /// <summary>
    /// Start function that starts the TCP Thread and initializes variables 
    /// </summary>
    void Start()
    {
        //Get ICD Script also on EHS Obj
        icd = GetComponent<packetICD>();

        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
        streaming = false;
        reqSent = false;

        funcDelegate1 tmpDelegate0 = new funcDelegate1(startStream);
        funcDelegate2 tmpDelegate1 = new funcDelegate2(stopStream);
        functionDebug.Instance.registerFunction("start", tmpDelegate0);
        functionDebug.Instance.registerFunction("stop", tmpDelegate1);

        //Note: Uncomment the line below if the script is always active. Otherwise the controlling script will call this method to begin streaming
        //startStream();
    }

    /// <summary>
    /// Update function that checks for message needing to be sent
    /// </summary>
    private void Update()
    {
        if (streaming)
        {
            if (!reqSent)
            {
                Debug.Log("Sending message ");
                //SendMsg("START");
                reqSent = true;
            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// stopStream: sets the streaming flag to false and pauses the stream 
    /// Note this does not close the active stream just request the client not send packets 
    /// </summary>
    public void stopStream()
    {
        streaming = false;
        //SendMsg("STOP");
        //restartServer();
        reqSent = false;
    }

    /// <summary>
    /// startStream: sets the streaming flag to true and begins the streaming process. 
    /// Uncomment this function in start if you want it to stream right away. 
    /// </summary>
    public void startStream()
    {
        Debug.Log("Starting stream");
        streaming = true;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Runs in background TcpServerThread; Handles incomming TCPClient requests
    /// </summary>
    private void ListenForIncommingRequests()
    {
        try
        {
            // Create listener on localhost.
            tcpListener = new TcpListener(IPAddress.Parse(IP), PORT);
            tcpListener.Start();
            Debug.Log("Server is listening\n");

            while (true)
            {
                tempTcpClient = tcpListener.AcceptTcpClient();
                Debug.Log("Connection");
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(tempTcpClient);
                Debug.Log("Maybe?");

            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }


    private void HandleClientComm(object client)
    {
        Byte[] bytes = new Byte[maxByteSize];
        NetworkStream stream =  ((TcpClient)client).GetStream();
        int length;
        // Read incomming stream into byte arrary.
        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            var incommingData = new byte[length];
            Array.Copy(bytes, 0, incommingData, 0, length);

            // Convert byte array to string message.
            string clientMessage = Encoding.ASCII.GetString(incommingData);

            //Process client header here 

            //First I need to get my first <BEG> and my last full <EOF> so that I don't have discontinuity 

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

            //Identify Client Msg here
            foreach (var msg in messages)
            {
                //Parse out msg string
                string id = "";
                string body = "";
                idx = msg.IndexOf("$");
                if (idx != -1)
                {
                    id = msg.Substring(0, idx);
                    Debug.Log("ID: " + id);
                }
                //handle processing
                if (Int32.TryParse(id, out int msgID))
                {
                    body = msg.Substring(idx + 1);
                    switch (msgID)
                    {    
                        case (int)packetICD.Type.SYSTEM:
                            if(body == "REG")
                            {
                                //First time registration 
                                Debug.Log("Creating Class");
                                chest_IMU = new chestIMU((TcpClient)client); 
                            }
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (chest_IMU != null)
                                {
                                    Debug.Log("Alredy have connected cli");
                                    chest_IMU.processPacket(body);
                                }
                                else
                                {
                                    Debug.Log("BIG OOF...");
                                }
                            }
                            break;
                        case (int)packetICD.Type.CHEST_IMU:
                            if(body == "REG") //First time connection 
                                chest_IMU = new chestIMU((TcpClient)client); 
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (chest_IMU != null)
                                    chest_IMU.processPacket(body);
                                else
                                    Debug.Log("BIG OOF...");
                            }
                            break;
                        case (int)packetICD.Type.GLOVE_IMU:
                            if(body == "REG") //First time connection 
                                chest_IMU = new chestIMU((TcpClient)client); 
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (chest_IMU != null)
                                    chest_IMU.processPacket(body);
                                else
                                    Debug.Log("BIG OOF...");
                            }
                            break;
                        case (int)packetICD.Type.TOGGLE_SCREEN:
                            if(body == "REG") //First time connection 
                                chest_IMU = new chestIMU((TcpClient)client); 
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (chest_IMU != null)
                                    chest_IMU.processPacket(body);
                                else
                                    Debug.Log("BIG OOF...");
                            }
                            break;
                        case (int)packetICD.Type.HEAD_CAM:
                            if(body == "REG") //First time connection 
                                chest_IMU = new chestIMU((TcpClient)client); 
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (chest_IMU != null)
                                    chest_IMU.processPacket(body);
                                else
                                    Debug.Log("BIG OOF...");
                            }
                            break;
                        case (int)packetICD.Type.GLOVE_CAM:
                            if(body == "REG") //First time connection 
                                chest_IMU = new chestIMU((TcpClient)client); 
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (chest_IMU != null)
                                    chest_IMU.processPacket(body);
                                else
                                    Debug.Log("BIG OOF...");
                            }
                            break;
                        case (int)packetICD.Type.FORCE_SENSOR:
                            if(body == "REG") //First time connection 
                                chest_IMU = new chestIMU((TcpClient)client); 
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (chest_IMU != null)
                                    chest_IMU.processPacket(body);
                                else
                                    Debug.Log("BIG OOF...");
                            }
                            break; 
                        default:
                            DebugManager.Instance.LogUnityConsole("ID Unknown: " + msgID);
                            break;   
                    }   
                }
                else
                {
                    DebugManager.Instance.LogSceneConsole("ERR: ID " + id + " could not be parsed to int");
                }
            }
        }
    }

    /// <summary>
    /// Send message function that takes in a message string and sends to active client if one is available
    /// </summary>
    /// <param name="message"></param>
    private void SendMsg(TcpClient cli, string message)
    {
        if (cli == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = cli.GetStream();
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
    #endregion
}
#endregion