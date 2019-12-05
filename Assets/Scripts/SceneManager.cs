﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Timers;


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
        HIDInstance = Instantiate(HID, new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
        EHSInstance = Instantiate(EHS, new Vector3(0, 0, 0), Quaternion.identity);
        

    }

    // Update is called once per frame
    void Update()
    {
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
}
