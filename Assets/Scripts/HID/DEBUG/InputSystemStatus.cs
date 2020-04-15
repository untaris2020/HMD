using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputSystemStatus : MonoBehaviour
{
    public InputSystemStatus Instance;

    public GameObject GazeInputButton;
    public GameObject GestureInputButton;
    public GameObject showGestureIndicatorsButton;

    public TextMeshProUGUI useGaze_text;
    public TextMeshProUGUI useGesture_text;
    public TextMeshProUGUI showGestureIndicators_text;

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
        ht.registerCollider(GazeInputButton.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("nav_GazeButton",tmpDelegate0);

        tmpDelegate0 = new Button0Delegate(GestureButtonPress);
        ht.registerCollider(GestureInputButton.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("nav_GestureButton",tmpDelegate0);

        tmpDelegate0 = new Button0Delegate(ShowGestureIndicatorsButtonPress);
        ht.registerCollider(showGestureIndicatorsButton.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("nav_ShowGestureIndicatorsButton",tmpDelegate0);

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
        } else
        {
            useGaze_text.SetText("OFF");
            useGaze_text.color = new Color32(255, 0, 0, 255);   // red
        }

        if (useGestures)
        {
            useGesture_text.SetText("ON");
            useGesture_text.color = new Color32(0, 255, 0, 255);   // green
        } else
        {
            useGesture_text.SetText("OFF");
            useGesture_text.color = new Color32(255, 0, 0, 255);   // red
        }

        if (showGestureIndicators) 
        {
            showGestureIndicators_text.SetText("ON");
            showGestureIndicators_text.color = new Color32(0, 255, 0, 255);   // green
        } else
        {
            showGestureIndicators_text.SetText("OFF");
            showGestureIndicators_text.color = new Color32(255, 0, 0, 255);   // red
        }
    }

    void GazeButtonPress() {
        if (!useGaze) {
            ToggleSystem();
        }
    }

    void GestureButtonPress() {
        if (!useGestures) {
            ToggleSystem();
        }
    }

    void ToggleSystem() {
        if (isGloveReady) {
            useGestures = !useGestures;
            useGaze = !useGaze;
        } else {
            DebugManager.Instance.LogBoth("ERROR", "Glove not connected. Cannot switch inputs");
        }

        UpdateUI();
    }

    void ShowGestureIndicatorsButtonPress() {
        showGestureIndicators = !showGestureIndicators;
        UpdateUI();
    }
}
