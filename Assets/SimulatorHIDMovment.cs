using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorHIDMovment : MonoBehaviour
{
    //public GameObject head;
    public GameObject body;
    private IEnumerator coroutine;
    public NavManager nav;
    float speed;

    bool hid_status = true;
    //public GameObject hid;

    Vector3 newPos;
    Vector3 offSet = new Vector3(0f, 1f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.transform.position = new Vector3(0f, 0f, 0f);
        //coroutine = TestLoop(1f);
        //StartCoroutine(coroutine);

        //nav.PressShowAll();
        //this.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //gameObject.transform.localPosition = new Vector3(0f, 10f, 0f);

        // turn off all shawdos in HID
        Renderer[] obj = GetComponentsInChildren<Renderer> (true);

        

        // get all buttons if any
        foreach (var ob in obj) {
            ob.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            ob.receiveShadows = false;
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        speed = Time.deltaTime * 12.0f;

        newPos = body.transform.position + offSet;

        this.transform.position = Vector3.Slerp(this.transform.position, newPos, speed + 8.0f);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, body.transform.rotation, speed); 

        //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, body.transform.position, 10f);
       // gameObject.transform.rotation = body.transform.rotation;

    }

    public void ToggleHIDVisibility() {
        hid_status = !hid_status;

        if (hid_status) {
            //this.transform.position = body.transform.position + offSet;
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            this.transform.localScale = new Vector3(0f, 0f, 0f);
        }
    }

    //int counter = 0;
    //private IEnumerator TestLoop(float time) {


    //    while (true) {
    //        if (counter < 10) {
    //            head.transform.Translate(new Vector3(25f,0f,0f));
    //        }
    //        if (counter == 10) {
    //            nav.PressRTH();
    //        }

    //        counter++;
    //        yield return new WaitForSeconds(time);
    //    }
    //}
}
