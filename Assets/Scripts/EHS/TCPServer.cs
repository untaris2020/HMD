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


    //Instatiated Object References
    private IMUHandler IMU_CHEST;

    #endregion


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
                                if (IMU_CHEST == null)
                                {
                                    IMU_CHEST = new IMUHandler((TcpClient)client);
                                }
                                else
                                {
                                    IMU_CHEST.restartIMUHandler((TcpClient)client);
                                }
                            }
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (IMU_CHEST != null)
                                {
                                    Debug.Log("Alredy have connected cli");
                                    IMU_CHEST.processPacket(body);
                                }
                            }
                            break;

                        case (int)packetICD.Type.CHEST_IMU:
                            if(body == "REG")
                            {
                                if (IMU_CHEST == null)
                                {
                                    IMU_CHEST = new IMUHandler((TcpClient)client);
                                }
                                else
                                {
                                    IMU_CHEST.restartIMUHandler((TcpClient)client);
                                }
                            }
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (IMU_CHEST != null)
                                {
                                    Debug.Log("Alredy have connected cli");
                                    IMU_CHEST.processPacket(body);
                                }
                            }
                            break;

                        case (int)packetICD.Type.GLOVE_IMU:
                            if(body == "REG")
                            {
                                if (IMU_CHEST == null)
                                {
                                    IMU_CHEST = new IMUHandler((TcpClient)client);
                                }
                                else
                                {
                                    IMU_CHEST.restartIMUHandler((TcpClient)client);
                                }
                            }
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (IMU_CHEST != null)
                                {
                                    Debug.Log("Alredy have connected cli");
                                    IMU_CHEST.processPacket(body);
                                }
                            }
                            break;

                        case (int)packetICD.Type.TOGGLE_SCREEN:
                            if(body == "REG")
                            {
                                if (IMU_CHEST == null)
                                {
                                    IMU_CHEST = new IMUHandler((TcpClient)client);
                                }
                                else
                                {
                                    IMU_CHEST.restartIMUHandler((TcpClient)client);
                                }
                            }
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (IMU_CHEST != null)
                                {
                                    Debug.Log("Alredy have connected cli");
                                    IMU_CHEST.processPacket(body);
                                }
                            }
                            break;

                        case (int)packetICD.Type.HEAD_CAM:
                            if(body == "REG")
                            {
                                if (IMU_CHEST == null)
                                {
                                    IMU_CHEST = new IMUHandler((TcpClient)client);
                                }
                                else
                                {
                                    IMU_CHEST.restartIMUHandler((TcpClient)client);
                                }
                            }
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (IMU_CHEST != null)
                                {
                                    Debug.Log("Alredy have connected cli");
                                    IMU_CHEST.processPacket(body);
                                }
                            }
                            break;

                        case (int)packetICD.Type.GLOVE_CAM:
                            if(body == "REG")
                            {
                                if (IMU_CHEST == null)
                                {
                                    IMU_CHEST = new IMUHandler((TcpClient)client);
                                }
                                else
                                {
                                    IMU_CHEST.restartIMUHandler((TcpClient)client);
                                }
                            }
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (IMU_CHEST != null)
                                {
                                    Debug.Log("Alredy have connected cli");
                                    IMU_CHEST.processPacket(body);
                                }
                            }
                            break;

                        case (int)packetICD.Type.FORCE_SENSOR:
                           if(body == "REG")
                           {
                                if (IMU_CHEST == null)
                                {
                                    IMU_CHEST = new IMUHandler((TcpClient)client);
                                }
                                else
                                {
                                    IMU_CHEST.restartIMUHandler((TcpClient)client);
                                }
                            }
                            else
                            {
                                //normal packet -- check if its already connected 
                                if (IMU_CHEST != null)
                                {
                                    Debug.Log("Alredy have connected cli");
                                    IMU_CHEST.processPacket(body);
                                }
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