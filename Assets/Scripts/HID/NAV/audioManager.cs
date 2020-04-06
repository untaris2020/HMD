﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class audioManager : MonoBehaviour
{
    public GameObject recordButton;
    public TextMeshProUGUI recordButtonText;
    public GameObject liveObjects;
    public GameObject recordingIndicator; 

    private bool recording;
    private float updateTime;
    private float waitTime = 1.0f;
    private float startTime;
    private float prevTime;
    private delegate void functionDelegate();

    // Start is called before the first frame update
    void Start()
    {
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();
        functionDelegate rec = new functionDelegate(recordingHit);    
        ht.registerCollider(recordButton.GetComponent<Collider>().name, rec);
        Time.timeScale = 1.0f;
        updateTime = 0f;
        prevTime = 0f;
        liveObjects.SetActive(false);
        recordingIndicator.SetActive(false);
    }

   
    // Update is called once per frame
    void Update()
    {
        if(recording)
        {
            updateTime = Time.time - prevTime;

            if(updateTime > waitTime)
            {
                prevTime = Time.time;
                float timeElapsed = Time.time - startTime; 
                string minutes = Mathf.Floor(timeElapsed/ 60).ToString("00");
                string seconds = Mathf.Floor(timeElapsed % 60).ToString("00");

                liveObjects.GetComponent<TextMeshProUGUI>().SetText("REC TIME: " + minutes + ":" + seconds);
                updateTime = 0f; 
            }
        }
    }

    void recordingHit()
    {
        if(recording)
        {
            liveObjects.SetActive(false);
            recordingIndicator.SetActive(false);
            recordButtonText.SetText("START");
            recording = false; 
        }
        else
        {
            liveObjects.SetActive(true); 
            recordingIndicator.SetActive(true);
            recordButtonText.SetText("STOP");
            liveObjects.GetComponent<TextMeshProUGUI>().SetText("REC TIME: 00:00");
            recording = true;
            startTime = Time.time;
            prevTime = Time.time;
        }
    }   
}
