using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public struct UdpState
{
    public UdpClient u;
    public IPEndPoint e;
}

public class functionDebug : MonoBehaviour
{
    public static functionDebug Instance;

    // Start is called before the first frame update

    public bool Verbose;

    public int listenPort = 10102;
    public string IP = "127.0.0.1";
    private bool messageReceived;
    private string recvMsg; 
    private Dictionary<string, Delegate> functions = new Dictionary<string, Delegate>();

    public UdpClient listener;
    public IPEndPoint groupEP;
    UdpState s;

    private void Awake()
    {
        Instance = this;
        messageReceived = false;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        ReceiveMessages();
    }

    // Update is called once per frame
    void Update()
    {
        if(messageReceived)
        {
           runFunction();
        }
    }

    public void registerFunction(string cmd, Delegate function)
    {    
        if(Verbose)
        {
            DebugManager.Instance.LogBoth("funcDebug:","Adding Function: " + cmd);
        }
        
        if(!(functions.ContainsKey(cmd)))
        {
            functions.Add(cmd, function);
        }
    }

    public void deregisterFunction(string cmd)
    {
        if(functions.ContainsKey(cmd))
        {
            functions.Remove(cmd);
        }
        else
        {
            DebugManager.Instance.LogUnityConsole("ERROR Removing Function: " + cmd);
        }
    }

    void ReceiveMessages()
    {
        listener = new UdpClient(listenPort);
        groupEP = new IPEndPoint(IPAddress.Parse(IP), listenPort);

        s = new UdpState();
        s.u = listener;
        s.e = groupEP;
        listener.BeginReceive(new System.AsyncCallback(ReceiveCallback), s); 
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        UdpClient u = ((UdpState)(ar.AsyncState)).u;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

        byte[] receiveBytes = u.EndReceive(ar, ref e);
        string received = Encoding.ASCII.GetString(receiveBytes);

        messageReceived = true;
        recvMsg = received;
    }

    void runFunction()
    {
        if (functions.ContainsKey(recvMsg))
        {
            functions[recvMsg].DynamicInvoke();
        }
        else
        {
            DebugManager.Instance.LogBoth("funcDebug:", "CMD \"" + recvMsg + "\" unknown");
        }
        messageReceived = false; 
        listener.BeginReceive(new System.AsyncCallback(ReceiveCallback), s); 
    }
}
