using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
public class HeadLockScript : MonoBehaviour
{ 

    #region Public Variables
    private GameObject Camera;
    private Vector3 Difference;
    private Quaternion DefaultRot;
    public float speed = 12f;
    public AudioClip reset;
    AudioSource source;
    private bool firstTime; 

    public float VOL = 1.7f;

    private bool buttonPressed = false;

    public enum State
    {
        STATIC = 1,
        CONTROLLER = 2, 
        EHS = 3
    }

    public State runState; 

    private ControllerConnectionHandler _controllerConnectionHandler;

    #endregion

    public void Start()
    {
        Camera = GameObject.Find("Main Camera");
        _controllerConnectionHandler = GetComponent<ControllerConnectionHandler>();
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
        else if (runState == State.CONTROLLER)
        { 
            if (_controllerConnectionHandler.IsControllerValid())
            {
                MLInputController controller = _controllerConnectionHandler.ConnectedController;

                Debug.Log("Controller Position Data: " + controller.Position);

                //Difference = (Camera.transform.position - controller.Position); unnessecary??

                if (buttonPressed)
                {
                    //Debug.Log("Button Pressed");
                    DefaultRot = controller.Orientation;
                    Difference = (Camera.transform.position - controller.Position);
                    this.transform.rotation = Camera.transform.rotation;
                    buttonPressed = false;
                }
                speed = Time.deltaTime * 12.0f;

                if (controller.Type == MLInputControllerType.Control)
                {
                    // For Control, raw input is enough
                    Vector3 temp = controller.Position + Difference;

                    this.transform.position = Vector3.Slerp(this.transform.position, temp, speed);

                    Debug.Log("Controller Rotation Data: " + controller.Orientation);

                    Quaternion rot = (controller.Orientation * Quaternion.Inverse(DefaultRot));
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, speed);
                }
            }
        }
    }


    public bool updateHIDwithIMU(float xQuan, float yQuan, float zQuan, float wQuan)
    {
        //Switching this to event driven 
        //This can be expanded to an event driven system for runtime resets. 
        if(firstTime)
        {
            //Get the default rotation value from the headset  
            //Quaternion temprot = new Quaternion(-imuData.yQuan, -imuData.zQuan, imuData.xQuan, imuData.wQuan);

            //Get the difference that should be applied every time. 
            DefaultRot = Camera.transform.rotation;
            firstTime = false; 
        }

        //Debug.Log("Updating HID...");

        // For Control, raw input is enough

        //Vector3 tempAccel = new Vector3(imuData.xAccel, imuData.yAccel, imuData.zAccel);

        speed = Time.deltaTime * 12.0f;

        this.transform.position = Vector3.Slerp(this.transform.position, Camera.transform.position, speed);

        Quaternion rot = new Quaternion(xQuan, yQuan, zQuan, wQuan);

        rot *= Quaternion.Euler(-90, 0, 0);

        rot = rot *  Quaternion.Inverse(DefaultRot); 

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, speed);

        return false;
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
        MLInputController controller = _controllerConnectionHandler.ConnectedController;
        if (controller != null && controller.Id == controllerId)
        {
            source.PlayOneShot(reset,VOL);
            
            MLInputControllerFeedbackIntensity intensity = (MLInputControllerFeedbackIntensity)((int)(value * 2.0f));
            controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, intensity);
            buttonPressed = true;
        }
    }
    #endregion
}