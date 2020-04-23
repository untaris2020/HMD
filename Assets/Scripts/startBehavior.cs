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

    public static startBehavior instance; 
    // Start is called before the first frame update
    void Awake()
    {
        instance = this; 
    }

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
    }
}
