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

    public TextMeshProUGUI[] statuses;
    public static Hashtable debugHashtable = new Hashtable();

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
    }

    private void OnDestroy()
    {
        // m_Instance = null;
        Instance = null;
    }



    void Start()
    {
        
        for (int i=0; i<consoleEntries.Length; i++) consoleEntries[i] = new ConsoleEntrie();

        debugHashtable.Add("telem_status","UN-INIT");
        debugHashtable.Add("chest_IMU","UN-INIT");
        debugHashtable.Add("hand_IMU","UN-INIT");
        debugHashtable.Add("hand_FS","UN-INIT");
        debugHashtable.Add("chest_cam","UN-INIT");
        debugHashtable.Add("hand_cam","UN-INIT");

        debugHashtable.Add("w","UN-INIT");
        debugHashtable.Add("chest_IMU_PC","UN-INIT");
        debugHashtable.Add("hand_IMU_PC","UN-INIT");
        debugHashtable.Add("x","UN-INIT");
        debugHashtable.Add("y","UN-INIT");
        debugHashtable.Add("z","UN-INIT");
        UpdateStatuses();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetParam(string name, string value)
    {
        debugHashtable[name] = value;
        UpdateStatuses();
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

    public void LogSceneConsole(string fileName, string msg)
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

    public void LogBoth(string fileName, string msg)
    {
        Debug.Log(fileName + ": " + msg);
        LogSceneConsole(fileName, msg);
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