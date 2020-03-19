using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class packetICD : MonoBehaviour
{
    public enum Type
    {
        SYSTEM = 0,
        CHEST_IMU = 1,
        GLOVE_IMU = 2,
        TOGGLE_SCREEN = 3, 
        HEAD_CAM = 4, 
        GLOVE_CAM = 5, 
        FORCE_SENSOR = 6
    }
    
    public struct Header
    {
        public Type type;   //Type of data packet
        public uint size;   //size of data packet in bits 
        public uint time;   //Unix epoch time
        public uint seqID;  //Sequence ID of the packet
    };


}

