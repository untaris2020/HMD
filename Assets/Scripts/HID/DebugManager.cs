using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;
    //public DebugManager Instance { get { return m_Instance; } }

    public enum State
    {
        LiveAR = 1,
        Simulator = 2
    }

    public State runState; 

    // sets runtime operation mode
    bool simulatorMode {
        get {
            if (State.Simulator == runState) {
                return true;
            } else {
                return false;
            }
        }
    }
    public bool GetSimulatorMode() {
        return simulatorMode;
    }

    private bool updateStatusesState; 

    public TextMeshProUGUI[] statuses;
    public static Hashtable debugHashtable = new Hashtable();
    public GameObject[] gestureMarkers;
    public GameObject sideScrollMarker;

    //private List<string> consoleEntries = new List<string>();

    public Hashtable duplicateCheckHashtable = new Hashtable();

    // 100 string que
    //private string[] consoleEntries = new string[100];
    private const int NUM_OF_OBJECTS = 100;
    private ConsoleEntrie[] consoleEntries = new ConsoleEntrie[NUM_OF_OBJECTS];

    public TextMeshProUGUI console;

    private void Awake()
    {
        //m_Instance = this;
        Instance = this;
        updateStatusesState = false; 
        // Moved from void Start()
        for (int i=0; i<consoleEntries.Length; i++) consoleEntries[i] = new ConsoleEntrie();

        debugHashtable.Add("telem_status","UN-INIT");
        debugHashtable.Add("chest_system","UN-INIT");
        debugHashtable.Add("glove_system","UN-INIT");
        debugHashtable.Add("chest_IMU","UN-INIT");
        debugHashtable.Add("glove_IMU","UN-INIT");
        debugHashtable.Add("head_cam","UN-INIT");
        debugHashtable.Add("glove_cam","UN-INIT");
        debugHashtable.Add("chest_toggle","UN-INIT");
        debugHashtable.Add("glove_toggle","UN-INIT");
        debugHashtable.Add("force_sensor","UN-INIT");
        debugHashtable.Add("microphone","UN-INIT");
        //debugHashtable.Add("z","UN-INIT");
        UpdateStatuses();
    }

    private void OnDestroy()
    {
        // m_Instance = null;
        Instance = null;
    }

    void Update()
    {
        if(updateStatusesState)
        {
            UpdateStatuses();
            updateStatusesState = false; 
        }
        
    }


    public void SetParam(string name, string value)
    {
        debugHashtable[name] = value;
        updateStatusesState = true; 
        return;
    }

    public void UpdateStatuses()
    {
        foreach (TextMeshProUGUI obj in statuses)
        {
            if (obj != null)
            {
                obj.SetText((string)debugHashtable[obj.name]);

                if (obj.text == "CON")
                {
                    obj.color = new Color32(0, 255, 0, 255);
                } else if (obj.text == "D-CON")
                {
                    obj.color = new Color32(255, 0, 0, 255);
                } else
                {
                    obj.color = new Color32(255, 255, 255, 255);
                }
            }
        }
    }

    public void LogSceneConsole(string msg)
    {
        //Debug.Log(fileName + ": " + msg);

        ShiftRight<ConsoleEntrie>(consoleEntries, 1);
        consoleEntries[0] = new ConsoleEntrie();

        if (duplicateCheckHashtable.Contains(msg))
        {
            // Duplicate message here
            for (int i=0; i<consoleEntries.Length; i++)
            {
                if (consoleEntries[i].msg == msg)
                {
                    consoleEntries[i].count++;

                    // Move duplicate entry to top of array
                    // copy to 0th position
                    ConsoleEntrie tmp = consoleEntries[i];
                    consoleEntries[0] = tmp;

                    // deleate duplicate
                    for (int j = i; j<consoleEntries.Length-1; j++)
                    {
                        consoleEntries[j] = consoleEntries[j + 1];
                    }

                    // break out of the for loop
                    i = consoleEntries.Length + 5;
                }
            }

        } else
        {
            duplicateCheckHashtable.Add(msg, 0);
            consoleEntries[0].msg = msg;
            consoleEntries[0].count = 1;
        }

        

        string buildString = "";


        for (int i=0; i<30; i++)
        { 
            buildString += String.Format("{{{0}}}: {1} \n", consoleEntries[i].count, consoleEntries[i].msg);
        }

        console.text = buildString;
    }

    public void LogUnityConsole(string fileName, string msg)
    {
        Debug.Log(fileName + ": " + msg);
    }

    public void LogUnityConsole(string msg)
    {
        Debug.Log(msg);
    }

    public void LogBoth(string fileName, string msg)
    {
        Debug.Log(fileName + ": " + msg);
        LogSceneConsole(msg);
    }

    public void LogBoth(string msg)
    {
        Debug.Log(msg);
        LogSceneConsole(msg);
    }

    // UTIL Functions

    public static void ShiftRight<ConsoleEntrie>(ConsoleEntrie[] arr, int shifts)
    {
        Array.Copy(arr, 0, arr, shifts, arr.Length - shifts);
        Array.Clear(arr, 0, shifts);
        //arr[0] = new ConsoleEntrie();
    }

   
}


public class ConsoleEntrie
{
    public string msg;
    public int count;

    public ConsoleEntrie()
    {
        msg = "";
        count = 0;
    }

}