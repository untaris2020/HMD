using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class packetICD : MonoBehaviour
{
    public enum Type
    {
        system = 0,
        EHS_IMU = 1,
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
        
        
    public struct GloveIMU
    {
        public Header header;
 

        //Add vars here needed for the glove packet 
    };




}
