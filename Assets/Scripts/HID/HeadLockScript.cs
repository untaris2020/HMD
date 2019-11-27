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


    public void updateHIDwithIMU(IMUJson imuData)
    {
        Debug.Log("Updating HID...");

        // For Control, raw input is enough

        Vector3 tempAccel = new Vector3(imuData.xAccel, imuData.yAccel, imuData.zAccel);
        
        speed = Time.deltaTime * 12.0f;

        this.transform.position = Vector3.Slerp(this.transform.position, tempAccel, speed);

        Vector3 tempRot = new Vector3(imuData.xAccel, imuData.yAccel, imuData.zAccel);

        Quaternion rotation = Quaternion.Euler(tempRot);

        //Quaternion rot = (rotation.orientation * Quaternion.Inverse(DefaultRot));
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);
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