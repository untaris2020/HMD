﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
public class SceneManager : MonoBehaviour
{

    private static int TIMEOUT = 1000; 
    public GameObject HID;
    private GameObject HIDInstance;
    public GameObject EHS;
    private GameObject EHSInstance;

    private Dictionary<Socket, Delegate> receiveFunctions = new Dictionary<Socket, Delegate>();
    private static ArrayList readList = new ArrayList();


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting Scene...");
        Debug.Log("Initializing HID...");
        HIDInstance = Instantiate(HID, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log("Initializing EHS...");
        EHSInstance = Instantiate(EHS, new Vector3(0, 0, 0), Quaternion.identity);
       
    }

    // Update is called once per frame
    void Update()
    {
        ArrayList listenList = readList; 
        
        if (listenList.Count > 0)
        {
            Socket.Select(listenList, null, null, TIMEOUT);
        
            for (int i = 0; i < listenList.Count; i++)
            {
                  Debug.Log("Data Received");

                int retval = (int)receiveFunctions[(Socket)listenList[i]].DynamicInvoke((Socket)listenList[i]);
                if (retval != 0)
                {
                    Debug.Log("RECEIVED ERROR");
                }
            }
        }
        else{
            Debug.Log("Select Empty");
        }
          
    }

    public void RegisterDevice(Socket listener, string Name, bool Status, Delegate receiveFunction)
    {
        Debug.Log("Adding " + Name + " to scene manager list");

        // receiveFunction.DynamicInvoke(sock);

        receiveFunctions.Add(listener, receiveFunction);

        readList.Add(listener);
    }
}