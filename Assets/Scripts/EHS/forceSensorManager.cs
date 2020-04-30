using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public class forceSensorManager : tcpPacket
{
    // Loading Screen
    public HeadTracking ht; 
    private string NAME; 
    private bool GloveActive;
    private bool newPacket;
    private bool newInput;
    private bool destroyHeadCamera = false;
    private bool destroyGloveCamera = false;
    fingerInput currentInput; 

    private delegate void functionDelegate();

    public struct fingerInput
    {
        public fingerInput(int newThumb, int newIndex, int newMiddle, int newRing, int newLittle)
        {
            thumb = newThumb;
            index = newIndex;
            middle = newMiddle;
            ring = newRing;
            little = newLittle; 
        }

        public void SetValues(int newThumb, int newIndex, int newMiddle, int newRing, int newLittle) {
            thumb = newThumb;
            index = newIndex;
            middle = newMiddle;
            ring = newRing;
            little = newLittle; 
        }

        public int thumb;
        public int index;
        public int middle;
        public int ring;
        public int little; 
    }

    
    public void Start()
    {
        base.Start();
        debugName = "force_sensor";
        connected = false;
        newInput = false; 
        NAME = "forceSensor";
        functionDelegate stopDelegate = new functionDelegate(stopStream);
        functionDelegate startDelegate = new functionDelegate(startStream);
        functionDebug.Instance.registerFunction(NAME + "start", startDelegate);
        functionDebug.Instance.registerFunction(NAME + "stop", stopDelegate);

        reportStatus(); 
    }

    protected override void handleDiscon()
    {
        ErrorHandler.Instance.HandleError(0, "FORCE SENSOR: ERROR LOST CONNECTION");
        DebugManager.Instance.LogBoth("FORCE SENSOR: ERROR LOST CONNECTION");
        //Toggle the cameras off 
        if(NavManager.Instance.getHeadCam())
        {
            NavManager.Instance.ToggleRearviewCam();
        }
        if(NavManager.Instance.getGloveCam())
        {
            NavManager.Instance.ToggleGloveCam();
        }

        //Toggle glove input off 
        InputSystemStatus.Instance.ChangeGloveStatus(false);
        base.handleDiscon();
    }

    void Update()
    {
        if (destroyHeadCamera) {
            NavManager.Instance.ToggleRearviewCam();
            destroyHeadCamera = false;
        }

        if (destroyGloveCamera) {
            NavManager.Instance.ToggleGloveCam();
            destroyGloveCamera = false;
        }
        base.Update(); 
        if(newInput)
        {
            //Call reworked force sensor here. 
            newInput = false;
            ht.forceClick(currentInput); 
        }
      
    }


    public override int processPacket(string packet)
    {
        if(NavManager.Instance.getHeadCam())
        {
            //We have an active frame 
            destroyHeadCamera = true;
        }
        else if(NavManager.Instance.getGloveCam())
        {
            destroyGloveCamera = true;
            
        }
        else
        {
            string[] tmp = packet.Split(new string[] { "$" }, StringSplitOptions.None);
            try
            {
                if (tmp.Length == 5)
                {
                    currentInput.thumb = (int.Parse(tmp[0]));
                    currentInput.index = (int.Parse(tmp[1]));
                    currentInput.middle = (int.Parse(tmp[2]));
                    currentInput.ring = (int.Parse(tmp[3]));
                    currentInput.little = (int.Parse(tmp[4]));
                    newInput = true; 
                }
            }
            catch (FormatException e)
            {
                DebugManager.Instance.LogUnityConsole(e.Message);
                return -1;
            }
        }

    
        return 0;
    }
}
