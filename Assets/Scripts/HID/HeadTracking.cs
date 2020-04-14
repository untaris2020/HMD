using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;
using System;

public class HeadTracking : MonoBehaviour
{
    private Camera cam;

    private string currentCollider = null; 

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

    private Dictionary<string, Delegate> colliderList = new Dictionary<string, Delegate>();

    void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        source = GetComponent<AudioSource>();
    }

    //Future Edge case: Collider name changed. 

    public void registerCollider(string colliderName, Delegate colliderFunc)
    {
        //Debug.Log("Adding collider: " + colliderName);
        colliderList.Add(colliderName, colliderFunc);
    }


    void Update()
    {
        RaycastHit hit;

        //float lerp = Mathf.PingPong(Time.time, duration) / duration;
        //float lerp = 0;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            string collidedName = hit.collider.name;

            

            //Debug.Log("Collided Name: " + collidedName + " RES: " + colliderList.ContainsKey(collidedName));
            //Check dictionary for collided name here
            Cursor.transform.position = Vector3.Slerp(Cursor.transform.position, hit.point, speed); //Updates the position of the cursor to show object collided with
            Cursor.transform.rotation = Quaternion.Slerp(Cursor.transform.rotation, cam.transform.rotation, speed);
            Cursor.SetActive(true);

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
                if (colliderList.ContainsKey(collidedName))
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
                else
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
                    }
                }
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
                Cursor.SetActive(false);
                dial.fillAmount = 0;
                missCount = 0;
                elapsed = 0;
                pause = .25f;
            }
        }
    }

    public void forceClick()
    {
        //DebugManager.Instance.LogBoth(this.GetType().Name, "Force Click Registered");

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            string collidedName = hit.collider.name;

            if (colliderList.ContainsKey(collidedName))
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
    }
}