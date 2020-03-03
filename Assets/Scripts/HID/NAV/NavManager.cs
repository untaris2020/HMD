using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class NavManager : MonoBehaviour
{
    public GameObject rearviewONButton, rearviewOFFButton, gloveONButton, gloveOFFButton;
    public Material buttonMat, buttonHoverMat, headerMat, headerHoverMat;
    public CamerasManager camerasManager;

    public delegate void MyDelegate();

    public MLPersistentBehavior persistentBehavior;
    public GameObject _cube, _camera;
    private MLInputController _controller;

    //private IEnumerator coroutine;
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        counter = 0;
        //coroutine = SimulateButtons(2.0f);
        //StartCoroutine(coroutine);

        MyDelegate RearviewON = new MyDelegate(PressRearviewON);
        ht.registerCollider(rearviewONButton.GetComponent<Collider>().name,RearviewON);

        MyDelegate RearviewOFF = new MyDelegate(PressRearviewOFF);
        ht.registerCollider(rearviewOFFButton.GetComponent<Collider>().name,RearviewOFF);

        MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Left);
    }

    private void OnDestroy() {
        MLInput.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        //_cube.transform.position = SOMETHING...
        // PCF Testing
        if (_controller.TriggerValue > 0.2f) {
            _cube.transform.position = _camera.transform.position + _camera.transform.forward * 2.0f;
            _cube.transform.rotation = _camera.transform.rotation;
            persistentBehavior.UpdateBinding();
        }
    }

    private IEnumerator SimulateButtons(float waitTime)
    {

        while (true)
        {
            if (counter == 0)
            {
                PressGloveOFF();
                PressRearviewOFF();
            } else if (counter == 1)
            {
                PressGloveON();
            } else if (counter == 2)
            {
                PressGloveOFF();
            } else
            {
                PressRearviewON();
                //counter = -1;
            }

            counter++;
            //Debug.Log("TICK");
            yield return new WaitForSeconds(waitTime);

        }
    }


    public void PressRearviewON()
    {
        rearviewONButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewOFFButton.GetComponent<Renderer>().material = buttonMat;

        // spawn camera
        camerasManager.SpawnRearviewCam();
    }

    public void PressRearviewOFF()
    {
        rearviewOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewONButton.GetComponent<Renderer>().material = buttonMat;
        
        //Despawn camera
        camerasManager.DestroyRearviewCam();
    }

    public void PressGloveON()
    {
        gloveONButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveOFFButton.GetComponent<Renderer>().material = buttonMat;
    }

    public void PressGloveOFF()
    {
        gloveOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveONButton.GetComponent<Renderer>().material = buttonMat;
    }
}
