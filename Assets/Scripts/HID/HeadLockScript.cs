using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
public class HeadLockScript : MonoBehaviour
{ 
    public static HeadLockScript Instance;

    #region Public Variables
    private GameObject Camera;
    private Vector3 Difference;
    private Quaternion DefaultRot;
    public float speed = 12f;
    public AudioClip reset;
    AudioSource source;
    private bool firstTime;

    private bool updateWithIMU;

    public float VOL = 1.7f;
    
    private bool buttonPressed = false;

    private Quaternion rot;
    public enum State
    {
        STATIC = 1,
        CONTROLLER = 2, 
        EHS = 3
    }

    public State runState; 

    private MLControllerConnectionHandlerBehavior _controllerConnectionHandler;

    #endregion

    private void Awake()
    { 
        Instance = this;
    }

    public void Start()
    {
        Camera = GameObject.Find("MainCamera");

        _controllerConnectionHandler = GetComponent<MLControllerConnectionHandlerBehavior>();
        MLInput.OnTriggerDown += HandleOnTriggerDown;
        
        source = GetComponent<AudioSource>();
        firstTime = true; 
    }

    #region Private Methods 
    public void Update()
    {
        if (runState == State.STATIC)  //For testing purpose only!
        {
            this.transform.position = Camera.transform.position;
            this.transform.rotation = Camera.transform.rotation;
        }
        else if (DebugManager.Instance.GetSimulatorMode()) {
            this.transform.position = Camera.transform.position;
            this.transform.rotation = Camera.transform.rotation;
        }
        else if (runState == State.CONTROLLER)
        {
            if (_controllerConnectionHandler.IsControllerValid())
            {
                MLInput.Controller controller = _controllerConnectionHandler.ConnectedController;

                //Debug.Log("Controller Position Data: " + controller.Position);

                if (buttonPressed)
                {
                    DefaultRot = controller.Orientation;
                    Difference = (Camera.transform.position - controller.Position);
                    this.transform.rotation = Camera.transform.rotation;
                    buttonPressed = false;
                }
                speed = Time.deltaTime * 12.0f;

                if (controller.Type == MLInput.Controller.ControlType.Control)
                {
                    // For Control, raw input is enough
                    Vector3 temp = controller.Position + Difference;

                    this.transform.position = Vector3.Slerp(this.transform.position, temp, speed);

                    //Debug.Log("Controller Rotation Data: " + controller.Orientation);

                    Quaternion rot = (controller.Orientation * Quaternion.Inverse(DefaultRot));
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, speed);
                }
            }
        }
        else if(updateWithIMU)
        {
            if(firstTime)
            {
                DefaultRot = rot;
                firstTime = false; 
            }
            speed = Time.deltaTime * 12.0f;
            this.transform.position = Vector3.Slerp(this.transform.position, Camera.transform.position, speed);

            rot = rot *  Quaternion.Inverse(DefaultRot);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, speed); 
            updateWithIMU = false;
        }
    }


    public void updateHIDwithIMU(float w, float x, float y, float z)
    { 
        rot = new Quaternion(-x,-z,-y,w);
        updateWithIMU = true; 
    }


    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles the event for trigger down.
    /// </summary>
    /// <param name="controller_id">The id of the controller.</param>
    /// <param name="value">The value of the trigger button.</param>
    private void HandleOnTriggerDown(byte controllerId, float value)
    {
        MLInput.Controller controller = _controllerConnectionHandler.ConnectedController;
        if (controller != null && controller.Id == controllerId)
        {
            source.PlayOneShot(reset, VOL);

            MLInput.Controller.FeedbackIntensity intensity = (MLInput.Controller.FeedbackIntensity)((int)(value * 2.0f));
            controller.StartFeedbackPatternVibe(MLInput.Controller.FeedbackPatternVibe.Buzz, intensity);
            buttonPressed = true;
        }
    }
    #endregion
}