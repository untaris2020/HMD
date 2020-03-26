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

public class CameraHandler : tcpPacket
{
    // Loading Screen

    private packetICD.CAM_Mode MODE;
    private bool packetRecv; 


    public CameraHandler(TcpClient client, packetICD.CAM_Mode mode, string NAME): base(client, NAME)
    {
        MODE = mode;
        packetRecv = false;
    }

    public override int processPacket(string packet)
    {
        CamerasManager.Instance.updateFrame(packet);
        return 0;
    }
    
}
