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
using System.Text.RegularExpressions;

#region TCPServer Class
public class TCPServer : MonoBehaviour
{
    public static TCPServer Instance;

    private string BEG = "<BEG>";
    private string EOF = "<EOF>";

    #region Public Variables
    public string IP;
    public int PORT;

        //Instatiated Object References
    public IMUHandler IMU_CHEST;
    public IMUHandler IMU_GLOVE;
    public CameraHandler HEAD_CAM; 
    public CameraHandler GLOVE_CAM;
    public ToggleHandler CHEST_TOGGLE;
    public ToggleHandler GLOVE_TOGGLE;
    public forceSensorManager FORCE_SENSOR;
    #endregion

    #region Private Variables
    private int maxByteSize = 30000; // Setting Byte Size Parameters
    private TcpListener tcpListener; // TCPListener to listen for incomming TCP connection requests.
    private Thread tcpListenerThread; // Background thread for TcpServer workload.
    private TcpClient tempTcpClient;




    //public GameObject SYSTEM;
    #endregion

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

            while (true)
            {
                tempTcpClient = tcpListener.AcceptTcpClient();
                DebugManager.Instance.LogBoth("New Client Connection: " + ((IPEndPoint)tempTcpClient.Client.RemoteEndPoint).Address.ToString());
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

            
            //Check if valid packet is received 
            parsed += clientMessage;

            Regex reg = new Regex(@"<BEG>(\d*)[$](.*)<EOF>");

            if (reg.IsMatch(clientMessage)) //Got a match
            {
                string[] remover = { BEG, EOF };
                string[] words = (parsed.Split(remover, System.StringSplitOptions.RemoveEmptyEntries));

                parsed = "";

                msg = words[0];

                /*PARSING COMPLETE*/

                //Identify Client Msg here

                //Parse out msg string
                string id = "";
                string body = "";
                int idx = msg.IndexOf("$");
                if (idx != -1)
                {
                    id = msg.Substring(0, idx);
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
                            if (body == "REG")
                            {
                                IMU_CHEST.initialize((TcpClient)client);
                            }
                            else
                            {
                                IMU_CHEST.processPacket(body);
                            }
                            break;
                        case (int)packetICD.Type.GLOVE_IMU:
                            if (body == "REG")
                            {
                                IMU_GLOVE.initialize((TcpClient)client);
                            }
                            else
                            {
                                IMU_GLOVE.processPacket(body);
                            }
                            break;
                        case (int)packetICD.Type.TOGGLE_SCREEN:
                            if (body == "REG")
                            {
                                CHEST_TOGGLE.initialize((TcpClient)client);
                                CHEST_TOGGLE.startStream(); 
                            }
                            else
                            {
                                CHEST_TOGGLE.processPacket(body);
                            }
                            break;
                        case (int)packetICD.Type.HEAD_CAM:
                            if (body == "REG")
                            {
                                HEAD_CAM.initialize((TcpClient)client);
                            }
                            else
                            {
                                HEAD_CAM.processPacket(body);
                            }
                            break;
                        case (int)packetICD.Type.GLOVE_CAM:
                            if (body == "REG")
                            {
                                GLOVE_CAM.initialize((TcpClient)client);
                            }
                            else
                            {
                                GLOVE_CAM.processPacket(body);
                            }
                            break;
                        case (int)packetICD.Type.FORCE_SENSOR:
                            if (body == "REG")
                            {
                                FORCE_SENSOR.initialize((TcpClient)client);
                                FORCE_SENSOR.startStream();
                            }
                            else
                            {
                                Debug.Log("Got body message");
                                FORCE_SENSOR.processPacket(body);
                            }
                            break;
                        case (int)packetICD.Type.TOGGLE_GLOVE:
                            if (body == "REG")
                            {
                                GLOVE_TOGGLE.initialize((TcpClient)client);
                                GLOVE_TOGGLE.startStream();
                            }
                            else
                            {
                                GLOVE_TOGGLE.processPacket(body);
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
    #endregion
}
#endregion