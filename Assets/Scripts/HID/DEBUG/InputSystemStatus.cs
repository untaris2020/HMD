using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputSystemStatus : MonoBehaviour
{
    public static InputSystemStatus Instance;

    public GameObject GazeInputButton;
    public GameObject GestureInputButton;
    public GameObject showGestureIndicatorsButton;

    public Collider GazeInputCollider;
    public Collider GestureInputCollider;
    public Collider showGestureIndicatorsCollider;

    public GameObject debugPG2_Col; 

    public TextMeshProUGUI useGaze_text;
    public TextMeshProUGUI useGesture_text;
    public TextMeshProUGUI showGestureIndicators_text;

    public forceSensorManager FORCE_SENSOR; 
    delegate void Button0Delegate();
    bool useGaze;
    bool useGestures;
    bool showGestureIndicators;
    bool isGloveReady;
    
    public bool GetUseGaze() {
        return useGaze;
    }
    public bool GetUseGestures() {
        return useGestures;
    }
    public bool GetShowGestureIndicators() {
        return showGestureIndicators;
    }
    public bool GetGloveReady() {
        return isGloveReady;
    }
    public void ChangeGloveStatus(bool status) {
        if (useGestures && status == false) {
            useGestures = false;
            useGaze = true;
            showGestureIndicators = false;
        }

        isGloveReady = status;
        UpdateUI();
    }

    void Start()
    {
        Instance = this;

        // defuat system status
        showGestureIndicators = true;
        useGestures = false;
        useGaze = true;
        isGloveReady = true; // reset this to false

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        Button0Delegate tmpDelegate0 = new Button0Delegate(GazeButtonPress);
        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        ht.registerCollider(GazeInputButton.GetComponent<Collider>().name, debugPG2_Col.name, tmpDelegate0, input);
        functionDebug.Instance.registerFunction(GazeInputCollider.name,tmpDelegate0);

        input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        tmpDelegate0 = new Button0Delegate(GestureButtonPress);
        ht.registerCollider(GestureInputButton.GetComponent<Collider>().name, debugPG2_Col.name, tmpDelegate0, input);
        functionDebug.Instance.registerFunction(GestureInputCollider.name,tmpDelegate0);

        input = new forceSensorManager.fingerInput(0, 0, 0, 1, 0);
        tmpDelegate0 = new Button0Delegate(ShowGestureIndicatorsButtonPress);
        ht.registerCollider(showGestureIndicatorsButton.GetComponent<Collider>().name, debugPG2_Col.name, tmpDelegate0, input);
        functionDebug.Instance.registerFunction(showGestureIndicatorsCollider.name,tmpDelegate0);

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // m_Instance = null;
        Instance = null;
    }

    private void UpdateUI()
    {

        if (useGaze)
        {
            useGaze_text.SetText("ON");
            useGaze_text.color = new Color32(0, 255, 0, 255);   // green
        }
        else
        {
            useGaze_text.SetText("OFF");
            useGaze_text.color = new Color32(255, 0, 0, 255);   // red
        }

        if (useGestures)
        {
            useGesture_text.SetText("ON");
            useGesture_text.color = new Color32(0, 255, 0, 255);   // green
        }
        else
        {
            useGesture_text.SetText("OFF");
            useGesture_text.color = new Color32(255, 0, 0, 255);   // red
        }

        if (showGestureIndicators) 
        {
            showGestureIndicators_text.SetText("ON");
            showGestureIndicators_text.color = new Color32(0, 255, 0, 255);   // green
        }
        else
        {
            showGestureIndicators_text.SetText("OFF");
            showGestureIndicators_text.color = new Color32(255, 0, 0, 255);   // red
        }
    }

    void GazeButtonPress() {
        if (!useGaze) {
            useGaze = !useGaze;
        }
        else
        {
            if(FORCE_SENSOR.getConnected())
            {
                useGaze = !useGaze;
                useGestures = true; //make sure at least 1 active 
            }
            else
            {
                DebugManager.Instance.LogBoth("ERROR", "Glove not connected. Cannot disable input");
            }
        }
        UpdateUI();
    }

    void GestureButtonPress() {
        if(FORCE_SENSOR.getConnected())
        {
            useGestures = !useGestures; 
            if(!useGestures)
            {
                useGaze = true; 
            }
        }
        UpdateUI();
    }
    void ShowGestureIndicatorsButtonPress() {
        showGestureIndicators = !showGestureIndicators;
        UpdateUI();
    }
}
