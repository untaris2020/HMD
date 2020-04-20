using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;
using TMPro;

public class NavManager : MonoBehaviour
{
    // TODO 
    // 1) create backup system
    // 2) fix persistant behivior

    public GameObject rearviewToggleButton, gloveToggleButton, rthButton, showallButton;
    public Material buttonMat, buttonHoverMat, headerMat, headerHoverMat;
    public Collider navPage0_col, navPage1_col;
    private CamerasManager camerasManager;

    private bool glove_active; 

    public delegate void MyDelegate();
    private delegate void functionDelegate();

    // Nav System
    //public MLPersistentBehavior persistentBehavior;
    public GameObject waypoint_prefab;
    public GameObject rth_waypoint_prefab;
    List<GameObject> waypoint_meshes = new List<GameObject>();
    List<GameObject> rth_waypoints = new List<GameObject>();

    //GameObject _content = null;
    //List<MLPersistentBehavior> _pointBehaviors = new List<MLPersistentBehavior>();


    List<UserPosition> userPositions = new List<UserPosition>();

    List<Waypoint> waypoints = new List<Waypoint>();

    public CameraHandler GLOVE_CAM;
    public CameraHandler REAR_CAM;
    bool rearviewCamStatus;
    bool gloveCamStatus;
    public GameObject _camera, _arrow, _world_center;
    public TextMeshProUGUI glove_text;
    public TextMeshProUGUI rearview_text;
    

    private IEnumerator coroutine;
    private float TICKTIME = 3f;   //was 5.0f
    private float BACKUPTIMESECONDS = 1000.0f;   // amount of time between backups was 300 (5mins)
    private int NUMOFOBJECTS = 0; 
    private int userPosCounter;
    private bool update_waypoints_rth = false;
    private bool rth_status = false;
    private bool showall_status = false;
    public TextMeshProUGUI rth_text;
    public TextMeshProUGUI showall_text;
    GameObject homeWaypoint;
    public GameObject homeBase;
    AStar astarScript;

    public bool getRTHStat() { return rth_status; }
    public bool getSAStat() { return showall_status; }
    public bool getHeadCam() { return rearviewCamStatus; }
    public bool getGloveCam() { return gloveCamStatus; }

    //TEST
    public GameObject test1;
    public GameObject test2;

    // Start is called before the first frame update
    void Start()
    {

        gloveCamStatus = false;
        rearviewCamStatus = false;

        camerasManager = GetComponent<CamerasManager>();
        NUMOFOBJECTS = (int)(BACKUPTIMESECONDS / TICKTIME);
        // Allocate space
        InitUserPositions();

        userPosCounter = 0;

        astarScript = GetComponent<AStar>();
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        functionDelegate startChest = new functionDelegate(ToggleRearviewCam);
        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        functionDebug.Instance.registerFunction("toggleRearviewCam", startChest);
        ht.registerCollider(rearviewToggleButton.GetComponent<Collider>().name, navPage1_col.name, startChest, input);

        input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        functionDelegate startGlove = new functionDelegate(ToggleGloveCam);
        functionDebug.Instance.registerFunction("toggleGloveCam", startGlove);
        ht.registerCollider(gloveToggleButton.GetComponent<Collider>().name, navPage1_col.name, startGlove, input);

        input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        functionDelegate rth = new functionDelegate(PressRTH);
        functionDebug.Instance.registerFunction("toggleRTH", rth);
        ht.registerCollider(rthButton.GetComponent<Collider>().name, navPage0_col.name, rth, input);

        input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        functionDelegate showall = new functionDelegate(PressShowAll);
        functionDebug.Instance.registerFunction("toggleShowAll", showall);
        ht.registerCollider(showallButton.GetComponent<Collider>().name, navPage0_col.name, showall, input);

        coroutine = GetUserPOSLoop(TICKTIME);
        StartCoroutine(coroutine);

        // set worldcenter
        if (DebugManager.Instance.GetSimulatorMode()) {
             _world_center.transform.position = homeBase.transform.position;
        } else {
             _world_center.transform.position = _camera.transform.position;
        }
       
        DebugManager.Instance.LogUnityConsole("NavManager", "Setting World Center: " + _world_center.transform.position);

        //persistentBehavior.UpdateBinding();
        UpdateUI();
    }

    private void OnDestroy() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("DST: " + Vector3.Distance(test1.transform.position, test2.transform.position));
        if (rth_status)
        {
            foreach (GameObject obj in waypoint_meshes)
            {
                var col = obj.GetComponent<Renderer>().material.color;
                col.a = 1.0f / ((float)(_camera.transform.position - obj.transform.position).magnitude / 10.0f);
                obj.GetComponent<Renderer>().material.color = col;
            }

            // update arrow position
            //_arrow.transform.RotateAround(Quaternion.LookRotation(_arrow.transform.position - _world_center.transform.position));
            _arrow.transform.LookAt(_world_center.transform);
        }
    }

    

    private IEnumerator GetUserPOSLoop(float waitTime)
    {
        
        while (true)
        {

            UserPosition tmpPos = new UserPosition(DateTime.Now, _camera.transform.position);

            
            //_cube.transform.rotation = _camera.transform.rotation;

            //DebugManager.Instance.LogUnityConsole("NavManager", "New Coordniate: " + tmpPos.position);
            userPositions[userPosCounter] = tmpPos;

            // Make a waypoint game object
            
            GameObject temp = Instantiate(waypoint_prefab, tmpPos.position, Quaternion.identity, _world_center.transform);
            temp.SetActive(true);
            temp.tag = "Waypoint";
            waypoint_meshes.Add(temp);

            UpdateWaypointsVisibility();

            if (userPosCounter >= (NUMOFOBJECTS)-1)
            {
                // save to backup
                // for now just deleate coords
                
                
                InitUserPositions();
                userPosCounter = 0;

            } else { userPosCounter++; }
            //Debug.Log(userPosCounter);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void InitUserPositions()
    {
        DebugManager.Instance.LogBoth("Clearing User Coordniates...");
        userPositions.Clear();

        foreach (GameObject obj in waypoint_meshes)
        {
            Destroy(obj);
        }
        waypoint_meshes.Clear();

        for (int i=0; i<NUMOFOBJECTS; i++)
        {
            userPositions.Add(new UserPosition());
        }
    }

    public void PressRTH()
    {
        rth_status = !rth_status;
        showall_status = false;
        _arrow.SetActive(rth_status);

        UpdateUI();
        UpdateWaypointsVisibility();

        //foreach (GameObject obj in waypoint_meshes)
        //{
        //    obj.SetActive(rth_status);
        //}

        if (rth_status) {

            // new code to drive ASTAR
            homeWaypoint = waypoint_meshes[0];  // first waypoint is home - note if the waypoitns get cleared (every X seconds set above) this alg will not work correctly

            List<GameObject> path = new List<GameObject>();
            path = astarScript.generateRTHPath(homeWaypoint);

            Debug.Log("Final path len: " + path.Count);


            foreach(GameObject obj in path) {
                GameObject tmp = Instantiate(rth_waypoint_prefab, obj.transform.position, Quaternion.identity, _world_center.transform);
                tmp.SetActive(true);
                rth_waypoints.Add(tmp);
            }

        } else {
            foreach(GameObject obj in rth_waypoints) {
                Destroy(obj);
            }
            rth_waypoints.Clear();
        }

    }

    public void PressShowAll()
    {
        // toggle function
        showall_status = !showall_status;
        rth_status = false;

        UpdateUI();
        UpdateWaypointsVisibility();
    }

    void UpdateWaypointsVisibility() {
        foreach (GameObject obj in waypoint_meshes)
        {

            obj.transform.localScale = new Vector3(0, 0, 0);
            if (showall_status)
            {
                obj.transform.localScale = new Vector3(1, 1, 1);
                var col = obj.GetComponent<Renderer>().material.color;
                col.a = 1.0f;
                obj.GetComponent<Renderer>().material.color = col;
            }
        }


    }

    private void UpdateUI()
    {
        _arrow.SetActive(rth_status);

        if (rth_status)
        {
            rth_text.SetText("ON");
            rth_text.color = new Color32(0, 255, 0, 255);   // green
        } else
        {
            rth_text.SetText("OFF");
            rth_text.color = new Color32(255, 0, 0, 255);   // red
        }

        if (showall_status)
        {
            showall_text.SetText("ON");
            showall_text.color = new Color32(0, 255, 0, 255);   // green
        } else
        {
            showall_text.SetText("OFF");
            showall_text.color = new Color32(255, 0, 0, 255);   // red
        }

        if (rearviewCamStatus)
        {
            rearview_text.SetText("ON");
            rearview_text.color = new Color32(0, 255, 0, 255);   // green
        } else
        {
            rearview_text.SetText("OFF");
            rearview_text.color = new Color32(255, 0, 0, 255);   // red
        }

        if (gloveCamStatus)
        {
            glove_text.SetText("ON");
            glove_text.color = new Color32(0, 255, 0, 255);   // green
        } else
        {
            glove_text.SetText("OFF");
            glove_text.color = new Color32(255, 0, 0, 255);   // red
        }
        
    }

    void ToggleRearviewCam() {
        gloveCamStatus = false;
        rearviewCamStatus = !rearviewCamStatus;

        UpdateCamStatus();
    }

    void ToggleGloveCam() {
        rearviewCamStatus = false;
        gloveCamStatus = !gloveCamStatus;

        UpdateCamStatus();
    }

    void UpdateCamStatus() {
        if (rearviewCamStatus) {
            PressRearviewON();
        } else {
            PressRearviewOFF();
        }

        if (gloveCamStatus) {
            PressGloveON();
        } else {
            PressGloveOFF();
        }

        UpdateUI();
    }


    public void PressRearviewON()
    {
        // spawn camera
        camerasManager.spawnCam();
        
        if(REAR_CAM.getConnected())
        {
            Debug.Log("START STREAM");
            REAR_CAM.startStream();
        }
        else
        {
            DebugManager.Instance.LogSceneConsole("ERROR: CAMERA DISCONNECTED");
        }
    }

    public void PressRearviewOFF()
    {
        
        //Despawn camera
        camerasManager.destroyCam();
        if(REAR_CAM.getConnected())
        {
            REAR_CAM.stopStream();
        }
        else
        {
            DebugManager.Instance.LogSceneConsole("ERROR: CAMERA DISCONNECTED");
        }
    }

    public void PressGloveON()
    {

        Debug.Log("glove on");
        camerasManager.spawnCam();
        if(GLOVE_CAM.getConnected())
        {
            GLOVE_CAM.startStream();
        }
        else
        {
            DebugManager.Instance.LogSceneConsole("ERROR: CAMERA DISCONNECTED");
        }
    }

    public void PressGloveOFF() {

        camerasManager.destroyCam();
        if (GLOVE_CAM.getConnected()) {
            GLOVE_CAM.stopStream();
        } else {
            DebugManager.Instance.LogSceneConsole("ERROR: CAMERA DISCONNECTED");
        }
    }

}

public class UserPosition
{
    public Vector3 position;
    public DateTime timestamp;

    public UserPosition(DateTime _timestamp, Vector3 _position)
    {
        timestamp = _timestamp;
        position = _position;
    }

    public UserPosition()
    {
        position = new Vector3(0f, 0f, 0f);
        timestamp = new DateTime();
    }
}

public class Waypoint
{
    public Vector3 position;
    public DateTime timestamp;

    public Waypoint(DateTime _timestamp, Vector3 _position)
    {
        timestamp = _timestamp;
        position = _position;
    }

    public Waypoint()
    {
        position = new Vector3(0f, 0f, 0f);
        timestamp = new DateTime();
    }
}