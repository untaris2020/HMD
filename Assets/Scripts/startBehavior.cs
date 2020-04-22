using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class startBehavior : MonoBehaviour
{
    public GameObject hid;
    public GameObject button;
    public GameObject text;
    
    private bool hidState; 
    private delegate void ButtonDelegate();
    public bool useStartRoutine;
    public bool ignoreIMU;
    public Collider startPG_col;
    public CameraHandler HEAD_CAM;
    public CameraHandler GLOVE_CAM;
    public IMUHandler IMU_CHEST;
    public IMUHandler IMU_GLOVE;
    public ToggleHandler CHEST_TOGGLE;
    public ToggleHandler GLOVE_TOGGLE;
    public forceSensorManager FORCE_SENSOR; 

    public ModelLoader ML;
    public audioManager AM;
    public NavManager NM; 
    // Start is called before the first frame update
    void Start()
    {
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();
        ButtonDelegate tmpDelegate = new ButtonDelegate(onClick);
        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        ht.registerCollider(this.GetComponentInChildren<Collider>().name, startPG_col.name, tmpDelegate, input);

        if(useStartRoutine)
        {
            hidState = false;
            toggleHID(hidState);
            
            button.SetActive(true);
            text.SetActive(true);
            startPG_col.enabled = true;

            this.GetComponentInChildren<TextMeshProUGUI>().SetText("CLICK TO START SCENE");
        }
        else
        {
            hidState = true;
            toggleHID(hidState);
            button.SetActive(false);
            text.SetActive(false);
            startPG_col.enabled = false;
        }
        
    }

    public void onClick()
    {
        if(IMU_CHEST.getConnected() || ignoreIMU)
        {
            hidState = true;
            toggleHID(hidState);
            IMU_CHEST.startStream();
            button.SetActive(false);
            text.SetActive(false);
            startPG_col.enabled = false;

        }
        else
        {
            this.GetComponentInChildren<TextMeshProUGUI>().SetText("Please Connect Chest IMU");
        }
        

    }

    public void toggleHID(bool mode)
    {
        if(mode == true)
        {
            hid.transform.localScale = new Vector3(1, 1, 1);
        }
        if(mode == false)
        {
            hid.transform.localScale = new Vector3(0, 0, 0);
        }
    }
    
    public void DisableHID()
    {
        hidState = false;
        toggleHID(hidState);
        button.SetActive(true);
        text.SetActive(true);
        startPG_col.enabled = true;
        this.GetComponentInChildren<TextMeshProUGUI>().SetText("CLICK TO START SCENE");
        
        //Toggle all hardware to off 
        HEAD_CAM.setConnected(false);
        GLOVE_CAM.setConnected(false);
        IMU_CHEST.setConnected(false);
        IMU_GLOVE.setConnected(false);
        CHEST_TOGGLE.setConnected(false);
        GLOVE_TOGGLE.setConnected(false);
        FORCE_SENSOR.setConnected(false); 

        //Toggle Camera inactive
        NM.PressRearviewOFF();
        NM.PressGloveOFF();

        //Toggle Waypoint incactive
        if(NM.getRTHStat())
        {
            NM.PressRTH();
        }
        if(NM.getSAStat())
        {
            NM.PressShowAll();
        }
        //Toggle 3D inactive 
        ML.ClearModelsButton();

        //Stop recording 
        if(AM.getRecording())
        {
            AM.recordingHit();
        }

        //Toggle the finger inputs to inactive
        InputSystemStatus.Instance.ChangeGloveStatus(false);

    }
}
