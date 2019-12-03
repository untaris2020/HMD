﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
public class SceneManager : MonoBehaviour
{
    public GameObject HID;
    public GameObject HIDInstance;
    public GameObject EHS;
    private GameObject EHSInstance;



    // Start is called before the first frame update
    void Start()
    {
        initalizeScene();   
    }

    void initalizeScene()
    {
        //Debug.Log("Starting Scene...");
        //Debug.Log("Initializing HID...");
        HIDInstance = Instantiate(HID, new Vector3(0, 0, 0), Quaternion.identity);
        //Debug.Log("Initializing EHS...");
        EHSInstance = Instantiate(EHS, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        //checkTimeout on sockets
    }
    
    public GameObject getHIDInstance()
    {
        return HIDInstance;
    }

    public HeadLockScript getHeadLockScriptInstance()
    {
        return (HeadLockScript)HIDInstance.GetComponent<HeadLockScript>();
    }

    public GameObject getEHSInstance()
    {
        return EHSInstance;
    }

    public void RegisterDevice(string Name, bool Status)
    {
        Debug.Log("Adding " + Name + " to scene manager list");


    }
}