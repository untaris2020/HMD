﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;
using System;

public class HeadTracking : MonoBehaviour
{
    private Camera cam;

    private string currentCollider; 

   // public Image Cursor; //parent object to move
    public GameObject Cursor;

    public Image dial; // image of the dial portion

    public AudioClip click;
    AudioSource source;

    public float VOL = 1.7f;

    public float speed = 1.2f;

    private float elapsed;

    public float timerSpeed =.85f;

    private float pause = 0; //this value forces a wait after click *Assigned after click to allow normal behavior on startup*

    public int threshold = 3;
    private int missCount; 

    struct forceClickData
    {
        public string pageCol; 
        public Delegate del;
        public forceSensorManager.fingerInput input; 
    }


    private Dictionary<string, Delegate> colliderList = new Dictionary<string, Delegate>();
    private Dictionary<string, forceClickData> pageColliderList = new Dictionary<string, forceClickData>();

    void Start()
    {
        currentCollider = null; 
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        source = GetComponent<AudioSource>();
    }

    //This is going away. Move away from this
    public void registerCollider(string colliderName, Delegate colliderFunc)
    {
        colliderList.Add(colliderName, colliderFunc);
    }


    public void registerCollider(string colliderName, string pageColliderName, Delegate colliderFunc, forceSensorManager.fingerInput inputs)
    {
        colliderList.Add(colliderName, colliderFunc);
        forceClickData temp;
        temp.del = colliderFunc;
        temp.input = inputs;
        temp.pageCol = pageColliderName;
        pageColliderList.Add(colliderName, temp);
    }


    void Update()
    {
        RaycastHit[] hits;
    
        //float lerp = Mathf.PingPong(Time.time, duration) / duration;
        //float lerp = 0;
        hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 100.0f);


        if (hits.Length > 0)
        {
            //Debug.Log("Collided Name: " + collidedName + " RES: " + colliderList.ContainsKey(collidedName));
            //Check dictionary for collided name here
            Cursor.transform.position = Vector3.Slerp(Cursor.transform.position, hits[0].point, speed); //Updates the position of the cursor to show object collided with
            Cursor.transform.rotation = Quaternion.Slerp(Cursor.transform.rotation, cam.transform.rotation, speed);
            Cursor.SetActive(true);

            foreach (RaycastHit hit in hits)
            {
                string collidedName = hit.collider.name;
                if (colliderList.ContainsKey(collidedName))
                {
                    if (collidedName != currentCollider && currentCollider != null)
                    {
                        if (missCount < threshold)
                        {
                            missCount++;
                        }
                        else
                        {
                            Cursor.SetActive(false);
                            dial.fillAmount = 0;
                            missCount = 0;
                            elapsed = 0;
                            pause = .25f;
                            currentCollider = null;
                        }
                    }
                    else
                    {
                        //Change texture of material if appliciable here 
                        currentCollider = collidedName;
                        missCount = 0; //reset the miss count becuase we have hit the object 

                        if (elapsed > pause)
                        {
                            if (dial.fillAmount != 1f) //if we have not clicked Start animation
                            {
                                dial.fillAmount = dial.fillAmount + Time.deltaTime * timerSpeed;
                            }
                            else //we have clicked 
                            {
                                //Debug.Log("Button pressed\n");
                                source.PlayOneShot(click, VOL);

                                //Call the delegate method here 
                                colliderList[collidedName].DynamicInvoke();

                                pause = .5f; //setting our delay
                                dial.fillAmount = 0; //reset the dial amount
                                elapsed = 0;
                            }
                        }
                        elapsed += Time.deltaTime; //checking number of seconds elapsed
                    }
                }
                else
                {
                    if (missCount < threshold)
                    {
                        missCount++;
                    }
                    else
                    {
                        dial.fillAmount = 0;
                        missCount = 0;
                        elapsed = 0;
                        pause = .25f;
                    }
                }
            }
        }
        else
        {
            Cursor.SetActive(false);
            dial.fillAmount = 0;
            missCount = 0;
            elapsed = 0;
            pause = .25f;
            currentCollider = null;
        }
    }

    public void forceClick(forceSensorManager.fingerInput input)
    {
        //DebugManager.Instance.LogBoth(this.GetType().Name, "Force Click Registered");

        RaycastHit[] hits;

        //float lerp = Mathf.PingPong(Time.time, duration) / duration;
        //float lerp = 0;
        hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 100.0f);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                string collidedName = hit.collider.name;

                foreach (forceClickData dat in pageColliderList.Values)
                {
                    if ((dat.pageCol == collidedName) && (dat.input.Equals(input)))
                    {
                        //We found the guy we are looking for 
                        dat.del.DynamicInvoke();
                    }
                }
            }
        }
    }
}