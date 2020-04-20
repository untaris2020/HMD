﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Timers;
using UnityEngine.XR.MagicLeap;


public class SceneMan : MonoBehaviour
{
    public GameObject HID;
    private GameObject HIDInstance;
    public GameObject EHS;
    private GameObject EHSInstance;
    public string IP = "192.168.1.123"; 

    // Start is called before the first frame update
    void Start()
    {
        initalizeScene();

        if (DebugManager.Instance.GetSimulatorMode()) {
            DebugManager.Instance.LogBoth("RUNNING: Simulator");
            HID.GetComponent<HeadLockScript>().enabled = false; //f 
            HID.GetComponent<SimulatorHIDMovment>().enabled = true; // t
            HID.GetComponent<MLControllerConnectionHandlerBehavior>().enabled = false;
        } else {
            DebugManager.Instance.LogBoth("RUNNING: Live AR");
            HID.GetComponent<HeadLockScript>().enabled = true;
            HID.GetComponent<SimulatorHIDMovment>().enabled = false;
            HID.GetComponent<MLControllerConnectionHandlerBehavior>().enabled = true;
        }
    }

    private void Awake() {
        
    }

    void initalizeScene()
    {
        //HIDInstance = Instantiate(HID, new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
        //EHSInstance = Instantiate(EHS, new Vector3(0, 0, 0), Quaternion.identity);
        

    }

    // Update is called once per frame
    void Update()
    {
    }

}
