using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// TODO 
// 1) Update error checking code to use new Database
// 2) add common names to database
// 3) add bool flag to disable console debug logs
// 4) heart_rate -> heart_bpm ,  also check telem values are consistent with server


public class TelemWebRequest : MonoBehaviour
{
    public const int NUM_OF_TELEM_VALUES = 15;

    public ErrorHandler errorScript;

    string JSONString, JSONString2;
    public TelemObject[] telemObjects;
    public StableCheckObject[] stableCheckObjects;
    public Hashtable telemHashtable = new Hashtable();
    public TelemDatabaseEntry[] telemDatabase = new TelemDatabaseEntry[NUM_OF_TELEM_VALUES];

    /*[SerializeField]*/
    public string telemServerURL, switchServerURL;
    private int counter;


    // const vatiables
    private const float tickTime = 1f;          // seconds
    private const int connectionLossTimer = 10; // measured in ticks
    public const int numOfStoredValues = 60;   // ticks - store last 2 minutes of data
    public const int numOfSuitValues = 12;
    //private const double errorWindowResetTime = 20000;      // number of miliseconds between error windows poppint up again
    public const int errorWindowDebounceTime = 3;      // number of ticks (seconds) of constant error before user is alerted

    public bool connectionStatus = false; //added to allow for external tracking of status 

    // Telem Flags


    // Switch Flags
    static public bool sop_on_Flag = false;
    static public bool sspe_Flag = false;
    static public bool fan_error_Flag = false;
    static public bool vent_error_Flag = false;
    static public bool vehicle_power_Flag = false;
    static public bool h2o_off_Flag = false;
    static public bool o2_off_Flag = false;

    // flags
    // True = flag has tripped
    // False = ready to be tripped
    // Timer Flags
    static public bool t_batteryFlagTripped0 = false;
    static public bool t_batteryFlagTripped1 = false;
    static public bool t_batteryFlagTripped2 = false;
    static public bool t_batteryFlagTripped3 = false;
    static public string t_batteryStatus = "green";
    static public bool oxFlagTripped0 = false;
    static public bool oxFlagTripped1 = false;
    static public bool oxFlagTripped2 = false;
    static public bool oxFlagTripped3 = false;
    static public string t_oxStatus = "green";
    static public bool waterFlagTripped0 = false;
    static public bool waterFlagTripped1 = false;
    static public bool waterFlagTripped2 = false;
    static public bool waterFlagTripped3 = false;
    static public string t_waterStatus = "green";

    public void ResetAllFlags()
    {
        t_batteryFlagTripped0 = false;
        t_batteryFlagTripped1 = false;
        t_batteryFlagTripped2 = false;
        t_batteryFlagTripped3 = false;
        oxFlagTripped0 = false;
        oxFlagTripped1 = false;
        oxFlagTripped2 = false;
        oxFlagTripped3 = false;
        waterFlagTripped0 = false;
        waterFlagTripped1 = false;
        waterFlagTripped2 = false;
        waterFlagTripped3 = false;


        sop_on_Flag = false;
        sspe_Flag = false;
        fan_error_Flag = false;
        vent_error_Flag = false;
        vehicle_power_Flag = false;
        h2o_off_Flag = false;
        o2_off_Flag = false;
    }

    public void ResetNonTimeFlags()
    { 
        sop_on_Flag = false;
        sspe_Flag = false;
        fan_error_Flag = false;
        vent_error_Flag = false;
        vehicle_power_Flag = false;
        h2o_off_Flag = false;
        o2_off_Flag = false;
    }

    public string GetDataFromString(string searchString)
    {
        if (!(telemHashtable[0] is null))
        {
            int index = (int)telemHashtable[searchString];
            Debug.Log("Index: " + index);

            if (!(telemObjects[1] is null)) {
                if (telemObjects[1].suit_populated)
                {
                    return telemObjects[1].values[index];
                } else
                {
                    return "Data not populated";
                }
            }
        }

        return "ERROR"; // error
    }


    void Start()
    {
        // Initalize database
        // Suit values
        
        telemDatabase[0] = new TelemDatabaseEntry("heart_rate", 60, 100, 0);
        telemDatabase[1] = new TelemDatabaseEntry("cap_battery", 0, 30, 1);
        telemDatabase[2] = new TelemDatabaseEntry("t_sub", -250, 250, 2);
        telemDatabase[3] = new TelemDatabaseEntry("p_o2", 750, 950, 3);
        telemDatabase[4] = new TelemDatabaseEntry("p_h2o_l", 14, 16, 4);
        telemDatabase[5] = new TelemDatabaseEntry("p_h2o_g", 14, 16, 5);
        telemDatabase[6] = new TelemDatabaseEntry("p_sop", 750, 950, 6);
        telemDatabase[7] = new TelemDatabaseEntry("p_suit", 2, 4, 7);
        telemDatabase[8] = new TelemDatabaseEntry("p_sub", 2, 4, 8);
        telemDatabase[9] = new TelemDatabaseEntry("rate_o2", 0.5, 1, 9);
        telemDatabase[10] = new TelemDatabaseEntry("rate_sop", 60, 100, 11);
        telemDatabase[11] = new TelemDatabaseEntry("v_fan", 10000, 40000, 10);
        

        // Times
        telemDatabase[12] = new TelemDatabaseEntry("t_battery", 1800, 12);
        telemDatabase[13] = new TelemDatabaseEntry("t_oxygen", 1800, 13);
        telemDatabase[14] = new TelemDatabaseEntry("t_water", 1800, 14);
        

        telemHashtable.Add(0, "heart_rate");
        telemHashtable.Add(1, "cap_battery");
        telemHashtable.Add(2, "t_sub");
        telemHashtable.Add(3, "p_o2");
        telemHashtable.Add(4, "p_h2o_l");
        telemHashtable.Add(5, "p_h2o_g");
        telemHashtable.Add(6, "p_sop");
        telemHashtable.Add(7, "p_suit");
        telemHashtable.Add(8, "p_sub");
        telemHashtable.Add(9, "rate_o2");
        telemHashtable.Add(10, "rate_sop");
        telemHashtable.Add(11, "v_fan");
        telemHashtable.Add(12, "t_battery");
        telemHashtable.Add(13, "t_oxygen");
        telemHashtable.Add(14, "t_water");

        telemHashtable.Add("heart_rate", 0);
        telemHashtable.Add("cap_battery", 1);
        telemHashtable.Add("t_sub", 2);
        telemHashtable.Add("p_o2", 3);
        telemHashtable.Add("p_h2o_l", 4);
        telemHashtable.Add("p_h2o_g", 5);
        telemHashtable.Add("p_sop", 6);
        telemHashtable.Add("p_suit", 7);
        telemHashtable.Add("p_sub", 8);
        telemHashtable.Add("rate_o2", 9);
        telemHashtable.Add("rate_sop", 10);
        telemHashtable.Add("v_fan", 11);
        telemHashtable.Add("t_battery", 12);
        telemHashtable.Add("t_oxygen", 13);
        telemHashtable.Add("t_water", 14);

        telemObjects = new TelemObject[numOfStoredValues];
        
        for (int i=0; i<numOfStoredValues; i++)
        {
            telemObjects[i] = new TelemObject(errorScript);
        }

        stableCheckObjects = new StableCheckObject[numOfSuitValues];
        for (int i=0; i<numOfSuitValues; i++)
        {
            stableCheckObjects[i] = new StableCheckObject(errorScript);
        }

        stableCheckObjects[0].name = "p_sub";
        stableCheckObjects[0].id = 0;
        stableCheckObjects[1].name = "t_sub";
        stableCheckObjects[1].id = 1;
        stableCheckObjects[2].name = "v_fan";
        stableCheckObjects[2].id = 2;
        stableCheckObjects[3].name = "p_o2";
        stableCheckObjects[3].id = 3;
        stableCheckObjects[4].name = "rate_o2";
        stableCheckObjects[4].id = 4;
        stableCheckObjects[5].name = "cap_battery";
        stableCheckObjects[5].id = 5;
        stableCheckObjects[6].name = "p_h2o_g";
        stableCheckObjects[6].id = 6;
        stableCheckObjects[7].name = "p_h2o_l";
        stableCheckObjects[7].id = 7;
        stableCheckObjects[8].name = "p_sop";
        stableCheckObjects[8].id = 8;
        stableCheckObjects[9].name = "rate_sop";
        stableCheckObjects[9].id = 9;
        stableCheckObjects[10].name = "heart_bpm";
        stableCheckObjects[10].id = 10;
        stableCheckObjects[11].name = "p_suit";
        stableCheckObjects[11].id = 11;

        //stableCheckObjects[7].name = "t_battery";

        //telemServerURL = "http://davidw.engineer:3000/api/suit/recent";
        //switchServerURL = "http://davidw.engineer:3000/api/suitswitch/recent";
        //telemServerURL = "http://davidw.engineer/ARIS/telem/suit.txt";
        //switchServerURL = "http://davidw.engineer/ARIS/telem/switch.txt";

        counter = 0;

        Debug.Log("Telem URL: " + telemServerURL);
        Debug.Log("Switch URL: " + switchServerURL);

        // start loop
        Tick();
    }

    // Pull and store data from server
    IEnumerator GetRequest(string uri, string uri2)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
                connectionStatus = false;
            }
            else
            {
                connectionStatus = true;
                 //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                JSONString = webRequest.downloadHandler.text;

                if (JSONString == "[]")
                {
                    // ERROR: Telem server not started
                    Debug.Log("ERROR: Telem server connected but not started");
                } else
                {
                    // remove [ and ]
                    //JSONString = JSONString.Substring(1, JSONString.Length - 2);
                    string pattern = @"[\[\]]";
                    JSONString = Regex.Replace(JSONString, pattern, "");

                    // remove all " to convert string to float
                    string pattern2 = @"""([\d\.]+)""";
                    JSONString = Regex.Replace(JSONString, pattern2, "$1");

                    //Debug.Log("FIXED: " + JSONString); 
                }
            }
        } 

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri2))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri2.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);

                TelemObject telemObject = new TelemObject(errorScript);
                telemObjects[0] = telemObject;
            }
            else
            {
                JSONString2 = webRequest.downloadHandler.text;

                if (JSONString2 == "[]")
                {
                    TelemObject telemObject = new TelemObject(errorScript);
                    telemObjects[0] = telemObject;
                } else
                {
                    // Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    //JSONString2 = JSONString2.Substring(1, JSONString2.Length - 2);
                    string pattern = @"[\[\]]";
                    JSONString2 = Regex.Replace(JSONString2, pattern, "");

                    // remove all " to convert string to float
                    string pattern2 = @"""([\d\.]+)""";
                    JSONString2 = Regex.Replace(JSONString2, pattern2, "$1");

                    //Debug.Log("FIXED: " + JSONString);

                    // link 2 JSON files

                    // take out last '}'
                    //JSONString = JSONString.Remove(JSONString.Length - 1, 1);
                    string pattern3 = @"}";
                    JSONString = Regex.Replace(JSONString, pattern3, "");

                    // take out first '{'
                    //JSONString2 = JSONString2.Remove(0, 1);
                    string pattern4 = @"{";
                    JSONString2 = Regex.Replace(JSONString2, pattern4, "");

                    JSONString = JSONString + "," + JSONString2;
                    //Debug.Log("Both: " + JSONString);

                    TelemObject telemObject = TelemObject.CreateFromJSON(JSONString);
                    telemObjects[0] = telemObject;
                    // TODO 6
                    telemObjects[0].suit_populated = true;
                    telemObjects[0].switch_populated = true;
                    telemObjects[0].errorScript = errorScript;
                } 
            }
        }

        

        yield return new WaitForSeconds(tickTime);

        Tick();
    }

    private void Tick()
    {
       // Debug.Log("Loop         : " + counter);

        // counter == 0 is the script connecting to the server
        // PICKUP HERE - debugging loss of connection
        if (counter > 0)
        {
            // make sure object is populated
            if (!telemObjects[0].suit_populated || !telemObjects[0].switch_populated)
            {
                // Packet loss here
                // check prev values for loss of signal
                if (connectionLossTimer >= numOfStoredValues)
                {
                    // SETUP ERROR, numOfStoredValues must be greater than connectionLoss Timer
                }
                else
                {
                    //Debug.Log("TEST")
                    int suitTimeOutCounter = 0;
                    int switchTimeOutCounter = 0;
                    for (int i = 0; i < connectionLossTimer; i++)
                    {
                        if (!telemObjects[i].suit_populated)
                        {
                            //Debug.Log("INFO: timeout counter: " + suitTimeOutCounter);
                            suitTimeOutCounter++;
                        }
                        if (!telemObjects[0].switch_populated)
                        {
                            switchTimeOutCounter++;
                        }
                    }

                    Debug.Log("Timeout: " + suitTimeOutCounter);
                    if (suitTimeOutCounter == connectionLossTimer)
                    {
                        connectionStatus = false;
                        // Critical Warning: Connection to telem suit server lost
                        Debug.Log("Critical Warning: Connection to telem SUIT server lost");
                    }
                    if (switchTimeOutCounter == connectionLossTimer)
                    {
                        connectionStatus = false;
                        // Critical Warning: Connection to telem suit server lost
                        Debug.Log("Critical Warning: Connection to telem SWITCH server lost");
                    }
                }
            }
            else
            {
                // check the new data for non-nominal values
                telemObjects[0].CheckVals();

                // convert times to seconds
                telemObjects[0].ConvertTimes();

                // add JSON parsed values to array (indexed by hashtable)
                telemObjects[0].AddValues();

                // reset vars
                foreach (StableCheckObject obj in stableCheckObjects)
                {
                    obj.Reset();
                }

                // check for stability
                foreach (StableCheckObject obj in stableCheckObjects)
                {
                    foreach (TelemObject telemObj in telemObjects)
                    {
                        obj.NestedUpdate(telemObj);
                    }
                }


               
                foreach (StableCheckObject obj in stableCheckObjects)
                {
                    obj.SingleUpdate(telemObjects[0]);
                }


                // spawn error windows if error for XXX seconds

                // Test logs
                // telemObjects[0].PrintVals();
            }
        }
        

        

        // FIFO array
        ShiftRight<TelemObject>(telemObjects, 1);

        counter++;

        // Get the most reccent Telem/switch data and add to head of telemObjects array
        StartCoroutine(GetRequest(telemServerURL, switchServerURL));
    }

    // shift Telem Objects
    public static void ShiftRight<Telemobject>(TelemObject[] arr, int shifts)
    {
        Array.Copy(arr, 0, arr, shifts, arr.Length - shifts);
        Array.Clear(arr, 0, shifts);
        arr[0] = new TelemObject();
    }
}

public class StableCheckObject
{
    public ErrorHandler errorScript;

    // vars
    public string name = "EMPTY";
    public int id = -1;
    public int counter = 0;
    public string status = "EMPTY";
    public int errorCounterTemp = 0;
    public bool errorReady = true;


    public void NestedUpdate(TelemObject obj)
    {
        //for each
        if (obj.flags[id] != 0)
        {
            counter++;
            errorCounterTemp++;
            status = "red";
        }
        else
        {
            errorCounterTemp = 0;
        }

        if (errorCounterTemp > TelemWebRequest.errorWindowDebounceTime)
        {
            status = "red";
            //Debug.Log(name + " Status: " + status);
            //Debug.Log("Ready: " + errorReady);
            if (errorReady)
            {
                // spawn error window

                if (obj.flags[id] == -1)
                {
                    Debug.Log("Warning: " + name + " low");
                    errorScript.HandleError(0, "Warning: " + name + " low");
                    errorReady = false;
                }
                else if (obj.flags[id] == 1)
                {
                    Debug.Log("Warning: " + name + " high");
                    errorScript.HandleError(0, "Warning: " + name + " high");
                    errorReady = false;
                }

            }
        }

    }


    public void SingleUpdate(TelemObject obj)
    {
        if (counter == 0 || counter == TelemWebRequest.numOfStoredValues)
        {
            // value is stable

        }
        else if (obj.flags[id] == 0)
        {
            // Unstable, and the most recent value is not error

            status = "yellow";
            //Debug.Log(name + " Status: " + status);
        }


        if (counter == 0)
        {
            // Reset
            status = "green";
            //Debug.Log(name + " Status: " + status);
            errorReady = true;
        }
    }



    public void Reset()
    {
        counter = 0;

        errorCounterTemp = 0;
    }

    public StableCheckObject(ErrorHandler _errorScript)
    {
        errorScript = _errorScript;
    }
}

public class TelemDatabaseEntry
{
    private string name, type;
    private double lowVal, highVal;
    private int key;
    private string paramIsNominal;      // this means the value is in/out of expected range - "nominal" "low" "high" "unsteady"
    private int errorTime;

    public TelemDatabaseEntry(string _name, double _lowVal, double _highVal, int _key)
    {
        // Suit entry
        name = _name;
        lowVal = _lowVal;
        highVal = _highVal;
        key = _key;
        paramIsNominal = "nominal";
        type = "suit";

        // Dont use these
        errorTime = -1;
    }

    public TelemDatabaseEntry(string _name, int _errorTime, int _key)
    {
        // Time entry
        name = _name;
        errorTime = _errorTime;
        key = _key;
        paramIsNominal = "nominal";
        type = "time";

        // Time entries dont use these params
        lowVal = -1;
        highVal = -1;
    }

    public TelemDatabaseEntry(string _name, int _key)
    {
        // Switch entry - unused as of 12/3/2019
        name = _name;
        key = _key;
        paramIsNominal = "nominal";
        type = "suitswitch";

        // Time entries dont use these params
        lowVal = -1;
        highVal = -1;
        errorTime = -1;
    }
}




public class TelemObject
 {

    public ErrorHandler errorScript;
    // Stablility flags
    //static public bool heart_bpm_Flag = false;
    public int[] flags = new int[TelemWebRequest.NUM_OF_TELEM_VALUES];
    public string[] values = new string[TelemWebRequest.NUM_OF_TELEM_VALUES];       //NOTE: this is an array of strings to account for multiple data types


    // NOTE: could make all these private for increased security
    // Telem variables

    public bool suit_populated;

    //public string _id;
    public double p_sub;
    public int t_sub;
    public int v_fan;
    public int p_o2;
    public double rate_o2;
    public int cap_battery;
    public int p_h2o_g;
    public int p_h2o_l;
    public int p_sop;
    public double rate_sop;
    public double p_suit;

    // old vals
    public string t_oxygen;
    public string t_water;
    public string create_date;          // might have issues with overwriting this from the two JSONs
    public string t_battery;
    public int heart_bpm;              // timeout variable to check

    // Switch variables
    // public DateTime create_date { get; set; }
    public bool switch_populated;
    public bool sop_on;                // timeout variable to check
    public bool sspe;
    public bool fan_error;
    public bool vent_error;
    public bool vehicle_power;
    public bool h2o_off;
    public bool o2_off;

    // Other Variables
    public double t_battery_dbl;
    public double t_oxygen_dbl;
    public double t_water_dbl;
       

    public void AddValues()
    {
        values[0] = Convert.ToString(heart_bpm);
        values[1] = Convert.ToString(cap_battery);
        values[2] = Convert.ToString(t_sub);
        values[3] = Convert.ToString(p_o2);
        values[4] = Convert.ToString(p_h2o_l);
        values[5] = Convert.ToString(p_h2o_g);
        values[6] = Convert.ToString(p_sop);
        values[7] = Convert.ToString(p_suit);
        values[8] = Convert.ToString(p_sub);
        values[9] = Convert.ToString(rate_o2);
        values[10] = Convert.ToString(rate_sop);
        values[11] = Convert.ToString(v_fan);
        values[12] = Convert.ToString(t_battery);
        values[13] = Convert.ToString(t_oxygen);
        values[14] = Convert.ToString(t_water);
    }

    public void ConvertTimes()
    {
        if (!t_oxygen.Contains("-"))
        {
            t_oxygen_dbl = TimeSpan.Parse(t_oxygen).TotalSeconds;
        }

        if (!t_battery.Contains("-"))
        {
            t_battery_dbl = TimeSpan.Parse(t_battery).TotalSeconds;
        }

        if (!t_water.Contains("-"))
        {
            t_water_dbl = TimeSpan.Parse(t_water).TotalSeconds;
        }
        // TEST CODE
        /*
        string testString = "9:59:59";
        double testDbl;
        testDbl = TimeSpan.Parse(testString).TotalSeconds;
        Debug.Log("Seconds  :" + testDbl);
        */
    }

    public void CheckVals()
    {
        // TIMES ------------------------------------------------------------------------------------
        // t_battery 
            if (t_battery.Contains("-"))
            {
            // ERROR: Battery time is negative
            TelemWebRequest.t_batteryStatus = "red";
            Debug.Log("ERROR: t_battery is negative");
            }
            else 
            {
                if (t_battery_dbl > 36000 && !TelemWebRequest.t_batteryFlagTripped0)
                {
                    // Warning: battery time is higher than 10 hours
                   // errorScript.HandleError(0,  "Warning: battery time is higher than 10 hours");
                    TelemWebRequest.t_batteryFlagTripped0 = true;
                }
                if (t_battery_dbl < 7200 && !TelemWebRequest.t_batteryFlagTripped1)
                {
                    // Warning: battery time has 2 hours left
                   // errorScript.HandleError(0,  "Warning: battery time has 2 hours left");
                    TelemWebRequest.t_batteryFlagTripped1 = true;
                }
                if (t_battery_dbl < 3600 && !TelemWebRequest.t_batteryFlagTripped2)
                {
                    // Warning: battery time has 1 hour left
                  //  errorScript.HandleError(0,  "Warning: battey time has 1 hours left");
                    TelemWebRequest.t_batteryFlagTripped2 = true;
                }
                if (t_battery_dbl < 1800 && !TelemWebRequest.t_batteryFlagTripped3)
                {
                    // Warning: battery time has 30 minutes left
                 //   errorScript.HandleError(0,  "Warning: battey time has 30 hours left");
                    TelemWebRequest.t_batteryFlagTripped3 = true;
                    TelemWebRequest.t_batteryStatus = "red";
                }
            }
            

        // t_oxygen
            if (t_oxygen.Contains("-"))
            {
            // ERROR: Oxygen time is negative
            TelemWebRequest.t_oxStatus = "red";
            }
            else
            {
                if (t_oxygen_dbl > 36000 && !TelemWebRequest.oxFlagTripped0)
                {
                    TelemWebRequest.oxFlagTripped0 = true;
                    // Warning: oxygen time is higher than 10 hours
                   // errorScript.HandleError(0,  "Warning: oxygen time is higher than 10 hours");
                }
                if (t_oxygen_dbl < 7200 && !TelemWebRequest.oxFlagTripped1)
                {
                    TelemWebRequest.oxFlagTripped1 = true;
                    // Warning: oxygen time has 2 hours left
                   // errorScript.HandleError(0,  "Warning: oxygen time has 2 hours left");
                }
                if (t_oxygen_dbl < 3600 && !TelemWebRequest.oxFlagTripped2)
                {
                    TelemWebRequest.oxFlagTripped2 = true;
                    // Warning: oxygen time has 1 hour left
                  //  errorScript.HandleError(0,  "Warning: oxygen time has 1 hour left");
                }
                if (t_oxygen_dbl < 1800 && !TelemWebRequest.oxFlagTripped3)
                {
                    TelemWebRequest.oxFlagTripped3 = true;
                    TelemWebRequest.t_oxStatus = "red";
                    // Warning: oxygen time has 30 minutes left
                   // errorScript.HandleError(0,  "Warning: oxygen time has 30 minutes left");
                }
            }


        //// t_water
            if (t_water.Contains("-"))
            {
            // ERROR: Water time is negative
                TelemWebRequest.t_waterStatus = "red";
            }
            else
            {
                if (t_water_dbl > 36000 && !TelemWebRequest.waterFlagTripped0)
                {
                    TelemWebRequest.waterFlagTripped0 = true;
                    // Warning: water time is higher than 10 hours
                   // errorScript.HandleError(0,  "Warning: water timer is higher than 10 hours");
                }
                if (t_water_dbl < 7200 && !TelemWebRequest.waterFlagTripped1)
                {
                    TelemWebRequest.waterFlagTripped1 = true;
                    // Warning: water time has 2 hours left
                   // errorScript.HandleError(0,  "Warning: water time has 2 hours left");
                }
                if (t_water_dbl < 3600 && !TelemWebRequest.waterFlagTripped2)
                {
                    TelemWebRequest.waterFlagTripped2 = true;
                    // Warning: water time has 1 hour left
                  //  errorScript.HandleError(0,  "Warning: water time has 1 hour left");
                }
                if (t_water_dbl < 1800 && !TelemWebRequest.waterFlagTripped3)
                {
                    TelemWebRequest.waterFlagTripped3 = true;
                    TelemWebRequest.t_waterStatus = "red";
                    // Warning: water time has 30 minutes left
                    //  errorScript.HandleError(0,  "Warning: water time has 30 minutes left");
                }
            }


        // TELEM ---------------------------------------------------------------------------------------------------------
        // heart_bpm
                if (heart_bpm > 100)
                {
                    // Warning: Heart rate too high
                    //TelemWebRequest.heart_bpm_Flag = true;
                    //errorScript.HandleError(0, "Warning: Heart rate too high");
                    flags[10] = -1;
                }
                if (heart_bpm < 60)
                {
                    // Warning: Heart rate too low
                    //TelemWebRequest.heart_bpm_Flag = true;
                    //errorScript.HandleError(0,  "Warning: Heart rate too low");
                    flags[10] = 1;
                }

        // p_suit
            if (p_suit < 2.0)
            {
                // Warning: Heart rate too high
                //TelemWebRequest.heart_bpm_Flag = true;
                //errorScript.HandleError(0, "Warning: Heart rate too high");
                flags[11] = -1;
            }
            if (p_suit > 4.0)
            {
                // Warning: Heart rate too low
                //TelemWebRequest.heart_bpm_Flag = true;
                //errorScript.HandleError(0,  "Warning: Heart rate too low");
                flags[11] = 1;
            }


        // p_sub - float
        // NASA CHECK
        // External Environment Pressure
        // Units: psia
                if (p_sub < 2.0)
                {
                    // Warning: external Pressure low
                    //errorScript.HandleError(0,  "Warning: external Pressure low");
                    flags[0] = -1;
                }
                else if (p_sub > 4.0)
                {
                    // Warning: external pressure high
                    //errorScript.HandleError(0,  "Warning: external pressure high");
                    flags[0] = 1;
                }
            

        // t_sub
            // External Environment Tempature
            // Units: degrees fahrenheit
                if (t_sub < -250)
                {
                    // Warning: Sub Tempature too low
                    //errorScript.HandleError(0,  "Warning: Sub Tempature too low");
                    flags[1] = -1;
                }
                else if (t_sub > 250)
                {
                    // Warning: Sub Tempature too high
                    //errorScript.HandleError(0,  "Warning: Sub Tempature too high");
                    flags[1] = 1;
                }

            

        // v_fan
            // Fan Tachometer
            // Units: rpm
                if (v_fan < 10000)
                {
                    // Warning: Fan Tachometer low
                    //errorScript.HandleError(0,  "Warning: Fan Tachometer low");
                    flags[2] = -1;
                }
                else if (v_fan > 40000)
                {
                    // Warning: Fan Tachometer high
                    //errorScript.HandleError(0,  "Warning: Fan Tachometer high");
                    flags[2] = 1;
                }
            

        // p_o2
            // NASA CHECK
            // Oxygen Pressure
            // Units: psia
                if (p_o2 < 750)
                {
                    // Warning: oxygen pressure low
                    //errorScript.HandleError(0,  "Warning: oxygen pressure low");
                    flags[3] = -1;
                }
                else if (p_o2 > 950)
                {
                    // Warning: oxygen pressure high
                    //errorScript.HandleError(0,  "Warning: oxygen pressure high");
                    flags[3] = 1;
                }
            

        // rate_o2
            // Oxygen Rate
            // Units: psi/mine
                if (rate_o2 < 0.5)
                {
                    // Warning: oxygen rate low
                    //errorScript.HandleError(0,  "Warning: oxygen rate low");
                    flags[4] = -1;
                }
                else if (rate_o2 > 1.0)
                {
                    // Warning: oxygen rate high
                    //errorScript.HandleError(0,  "Warning: oxygen rate high");
                    flags[4] = 1;
                }
            

        // cap_battery
            // Battery Capacity
            // Units: amp-h
                if (cap_battery < 0)
                {
                    // Warning: battery cap low
                    //errorScript.HandleError(0,  "Warning: battery cap low");
                    flags[5] = -1;
                }
                else if (cap_battery > 30)
                {
                    // Warning: battery cap high
                    //errorScript.HandleError(0,  "Warning: battery cap high");
                    flags[5] = 1;
                }
            

        // p_h2o_g
            // H2O Gas Pressure
            // Units: psia
                if (p_h2o_g < 14)
                {
                    // Warning: H2O Gas Pressure low
                    //errorScript.HandleError(0,  "Warning: H2O gas pressure low");
                    flags[6] = -1;
                }
                else if (p_h2o_g > 16)
                {
                    // Warning: H2O Gas Pressure high
                    //errorScript.HandleError(0,  "Warning: H2O gas pressure high");
                    flags[6] = 1;
                }
            

        // p_h2o_l
            // H2O Liquid Pressure
            // Units: psia
                if (p_h2o_l < 14)
                {
                    // Warning: H2O Liquid Pressure low
                    //errorScript.HandleError(0,  "Warning: H2O liquid pressure low");
                    flags[7] = -1;
                }
                else if (p_h2o_l > 16)
                {
                    // Warning: H2O Liquid Pressure high
                    //errorScript.HandleError(0,  "Warning: H2O liquid pressure high");
                    flags[7] = 1;
                }
            

        // p_sop
            // SOP Pressure
            // Units: psia
                if (p_sop < 750)
                {
                    // Warning: SOP Pressure low
                    //errorScript.HandleError(0,  "Warning: SOP pressure low");
                    flags[8] = -1;
                }
                else if (p_sop > 950)
                {
                    // Warning: SOP Pressure high
                    //errorScript.HandleError(0,  "Warning: SOP pressure high");
                    flags[8] = 1;
                }
            

        // rate_sop
            // SOP Rate
            // Units: psia/min
                if (rate_sop < 0.5)
                {
                    // Warning: SOP Rate low
                    //errorScript.HandleError(0,  "Warning: SOP rate low");
                    flags[9] = -1;
                }
                else if (rate_sop > 1.0)
                {
                    // Warning: SOP Rate high
                    //errorScript.HandleError(0,  "Warning: SOP rate high");
                    flags[9] = 1;
                }

        // SWITCHES -------------------------------------------------------------------
        // sop_on
            // Secondary oxygen pack is active
            // Units: switch
            if (!TelemWebRequest.sop_on_Flag)
            {
                
                if (sop_on)
                {
                    // Warning: SOP is active
                    // TODO / NASA CHECK - is this logic correct?
                    //errorScript.HandleError(0,  "Warning: SOP error");
                    TelemWebRequest.sop_on_Flag = true;
                }
            }
        
        // sspe
            // Space suit pressure emergency
            // Units: switch
            if (!TelemWebRequest.sspe_Flag)
            {
                
                if (sspe)
                {
                    // Warning: Space suit pressure emergency
                    //errorScript.HandleError(0,  "Warning: space suit pressure emergency");
                    TelemWebRequest.sspe_Flag = true;
                }
            }

        // fan_error
            // Space suit pressure emergency
            // Units: switch
            if (!TelemWebRequest.fan_error_Flag)
            {
                
                if (fan_error)
                {
                    // Warning: Fan faliure
                    //errorScript.HandleError(0,  "Warning: fan faliure");
                    TelemWebRequest.fan_error_Flag = true;
                }
            }

        // vent_error
            // Ventilation flow
            // Units: switch
            if (!TelemWebRequest.vent_error_Flag)
            {
                
                if (vent_error)
                {
                    // Warning: No ventailation flow is detected
                    //errorScript.HandleError(0,  "Warning: no ventailation flow is detected");
                    TelemWebRequest.vent_error_Flag = true;
                }
            }

        // vehicle_power
            // Power provided from the spacecraft
            // NASA CHECK
            // Units: switch
            if (!TelemWebRequest.vehicle_power_Flag)
            {
                
                if (vehicle_power)
                {
                    // Warning: No power is being recieved from the spacecraft
                    //errorScript.HandleError(0,  "Warning: no power is being recieved from the spacecraft");
                    TelemWebRequest.vehicle_power_Flag = true;
                }
            }
        
        // h2o_off
            // h2o system
            // Units: switch
            if (!TelemWebRequest.h2o_off_Flag)
            {
                
                if (h2o_off)
                {
                    // Warning: h2o system is offline
                    //errorScript.HandleError(0,  "Warning: h2o system is offline");
                    TelemWebRequest.h2o_off_Flag = true;
                }
            }

        // o2_off
            // o2 system
            // Units: switch
            if (!TelemWebRequest.o2_off_Flag)
            {
                
                if (o2_off)
                {
                    // Warning: o2 system is offline
                    //errorScript.HandleError(0,  "Warning: o2 system is offline");
                    TelemWebRequest.o2_off_Flag = true;
                }
            }
    }

    public void PrintVals()
    {
        // prints a few values for testing
        //Debug.Log("t_battery         : " + t_battery);
        //Debug.Log("Heart BPM    : " + heart_bpm);
        //Debug.Log("Sop          : " + sop_on);
        //Debug.Log("Battery      : " + cap_battery);
       // Debug.Log("V Fan        : " + v_fan);
       // Debug.Log("Fan Error    : " + fan_error);
       // Debug.Log("Vehicle Power: " + vehicle_power);
    }
    

    public static TelemObject CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TelemObject>(jsonString);
    }

    public TelemObject(ErrorHandler _errorScript)
    {
        // set these to null for checking if the script looses connection to the server
        suit_populated = false;
        switch_populated = false;
        errorScript = _errorScript;
    }

    public TelemObject()
    {
        // set these to null for checking if the script looses connection to the server
        suit_populated = false;
        switch_populated = false;
    }
}




