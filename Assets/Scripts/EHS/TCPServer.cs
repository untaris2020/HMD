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
    public static TCPServer Instance;

    private string BEG = "<BEG>";
    private string EOF = "<EOF>";

    #region Public Variables
    public string IP;
    public int PORT;
    #endregion

    #region Private Variables
    private int maxByteSize = 30000; // Setting Byte Size Parameters
    private TcpListener tcpListener; // TCPListener to listen for incomming TCP connection requests.
    private Thread tcpListenerThread; // Background thread for TcpServer workload.
    private TcpClient tempTcpClient;


    //Instatiated Object References
    private IMUHandler IMU_CHEST;
    private IMUHandler IMU_GLOVE;
    private CameraHandler HEAD_CAM; 
    private CameraHandler GLOVE_CAM; 
    private TcpClient SYSTEM;
    #endregion

    public CameraHandler getHeadCam()
    {
        return HEAD_CAM;
    }
        
    public CameraHandler getGloveCam()
    {
        return GLOVE_CAM;
    }

    #region Unity Methods
    /// <summary>
    /// Start function that starts the TCP Thread and initializes variables 
    /// </summary>
    private void Awake()
    { 
        Instance = this;
    }


    void Start()
    {
        //Get ICD Script also on EHS Obj
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
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
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }


    private void HandleClientComm(object client)
    {
        //VARIABLE DECLARATION FOR PARSE
        string parsed = "";
        string Beginning = "";
        string Middle = "";
        string End = "";
        string msg;

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

            //Process client msg
            // START OF THE PARSER CODE FOR THE CAMERA TO PUT EACH OF THE FRAMES TOGETHER

            // 1st Stage of Parsing. Checks if each of the packets has a <BEG> or <EOF>.
            // If either one is detected it is split into smaller packets to be put back together.
            if(clientMessage.Contains(BEG) == true)
            {
                string[] eof = {EOF};
                string[] words = clientMessage.Split(eof, System.StringSplitOptions.RemoveEmptyEntries);
                parsed = words[0];

            }
            else
            {
                string[] beg = {BEG};
                string[] words = clientMessage.Split(beg, System.StringSplitOptions.RemoveEmptyEntries);
                parsed = words[0];

            }

            // 2nd Stage of Parsing. Checks if the smaller packet starts with a "<BEG>". If it does start with a "<BEG>"
            // It is stored into the Beginning Temporary variable. If the smaller packet does not begin with a "<BEG>" it
            // moves to the else statement. In the else statement we then check if the smaller packet doesnt have a "<EOF>".
            // If it doesnt have a "<EOF>" it is stored in the temp varaible Middle. (This continues to concantinate until
            // a smaller packet with a "<EOF>" is detected. If a packet is detected with a "<EOF>" then we know that it was
            // the last of the frame. Therefore we can put the frame together and send it to be displayed. We also reset the
            // temporary variables for the next frame.

            if (parsed.Contains(BEG))
            {
                Beginning = parsed;
            }
            else
            {
                if(!parsed.Contains(EOF))
                {
                    Middle += parsed;
                }
                else
                {
                    End = parsed;

                    string[] remover = { BEG, EOF };
                    string[] words = (Beginning + Middle + End).Split(remover, System.StringSplitOptions.RemoveEmptyEntries);

                    msg = words[0]; 

                    Beginning = "";
                    Middle = "";
                    End = "";

                    /*PARSING COMPLETE*/ 

                    //Identify Client Msg here

                    //Parse out msg string
                    string id = "";
                    string body = "";
                    int idx = msg.IndexOf("$");
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
                                break;
                            case (int)packetICD.Type.CHEST_IMU:
                                if(body == "REG")
                                {
                                    if (IMU_CHEST == null)
                                    {
                                        IMU_CHEST = new IMUHandler((TcpClient)client, packetICD.IMU_Mode.CHEST, "chestIMU");
                                    }
                                    else
                                    {
                                        IMU_CHEST.restartHandler((TcpClient)client);
                                    }
                                }
                                else
                                {
                                    //normal packet -- check if its already connected 
                                    if (IMU_CHEST != null)
                                    {
                                        IMU_CHEST.processPacket(body);
                                    }
                                }
                                break;
                            case (int)packetICD.Type.GLOVE_IMU:
                                if(body == "REG")
                                {
                                    if (IMU_GLOVE == null)
                                    {
                                        IMU_GLOVE = new IMUHandler((TcpClient)client, packetICD.IMU_Mode.GLOVE, "gloveIMU");
                                    }
                                    else
                                    {
                                        IMU_GLOVE.restartHandler((TcpClient)client);
                                    }
                                }
                                else
                                {
                                    //normal packet -- check if its already connected 
                                    if (IMU_GLOVE != null)
                                    {
                                        IMU_GLOVE.processPacket(body);
                                    }
                                }
                                break;
                            case (int)packetICD.Type.TOGGLE_SCREEN:
                                break;
                            case (int)packetICD.Type.HEAD_CAM:
                                if(body == "REG")
                                {
                                    if (HEAD_CAM == null)
                                    {
                                        HEAD_CAM = new CameraHandler((TcpClient)client, packetICD.CAM_Mode.HEAD, "headCAM");
                                    }
                                    else
                                    {
                                        HEAD_CAM.restartHandler((TcpClient)client);
                                    }
                                }
                                else
                                {
                                    //normal packet -- check if its already connected 
                                    if (HEAD_CAM != null)
                                    {
                                        HEAD_CAM.processPacket(body);
                                    }
                                }
                                break;
                            case (int)packetICD.Type.GLOVE_CAM:
                                if(body == "REG")
                                {
                                    if (GLOVE_CAM == null)
                                    {
                                        GLOVE_CAM = new CameraHandler((TcpClient)client, packetICD.CAM_Mode.HEAD, "gloveCAM");
                                    }
                                    else
                                    {
                                        GLOVE_CAM.restartHandler((TcpClient)client);
                                    }
                                }
                                else
                                {
                                    //normal packet -- check if its already connected 
                                    if (GLOVE_CAM != null)
                                    {
                                        GLOVE_CAM.processPacket(body);
                                    }
                                }
                                break;
                            case (int)packetICD.Type.FORCE_SENSOR:                           
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

            
    }
    #endregion
}
#endregion