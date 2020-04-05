﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Timers;


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
