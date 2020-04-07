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

    public GameObject rearviewONButton, rearviewOFFButton, gloveONButton, gloveOFFButton, rthButton, showallButton;
    public Material buttonMat, buttonHoverMat, headerMat, headerHoverMat;
    public CamerasManager camerasManager;

    public delegate void MyDelegate();
    private delegate void functionDelegate();

    // Nav System
    //public MLPersistentBehavior persistentBehavior;
    public GameObject waypoint_mesh;
    List<GameObject> waypoint_meshes = new List<GameObject>();

    //GameObject _content = null;
    //List<MLPersistentBehavior> _pointBehaviors = new List<MLPersistentBehavior>();


    List<UserPosition> userPositions = new List<UserPosition>();

    List<Waypoint> waypoints = new List<Waypoint>();

    private CameraHandler GLOVE_CAM;
    private CameraHandler REAR_CAM;

    public GameObject _cube, _camera, _arrow, _world_center;
    

    private IEnumerator coroutine;
    private float TICKTIME = 1.0f;   //was 10.0f
    private float BACKUPTIMESECONDS = 20.0f;   // amount of time between backups was 300 (5mins)
    private int NUMOFOBJECTS = 0; 
    private int userPosCounter;
    private bool update_waypoints_rth = false;
    private bool rth_status = false;
    private bool showall_status = false;
    public TextMeshProUGUI rth_text;
    public TextMeshProUGUI showall_text;


    // Start is called before the first frame update
    void Start()
    {
        NUMOFOBJECTS = (int)(BACKUPTIMESECONDS / TICKTIME);
        // Allocate space
        InitUserPositions();

        userPosCounter = 0;

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        REAR_CAM = GameObject.Find("HEAD_CAM").GetComponent<CameraHandler>();
        GLOVE_CAM = GameObject.Find("HEAD_CAM").GetComponent<CameraHandler>();

        functionDelegate startRear = new functionDelegate(PressRearviewON);
        functionDelegate stopRear = new functionDelegate(PressRearviewOFF);
        functionDebug.Instance.registerFunction("startRearCam", startRear);
        functionDebug.Instance.registerFunction("stopRearCam", stopRear);

        functionDelegate startGlove = new functionDelegate(PressGloveON);
        functionDelegate stopGlove = new functionDelegate(PressGloveOFF);
        functionDebug.Instance.registerFunction("startGloveCam", startGlove);
        functionDebug.Instance.registerFunction("stopGloveCam", stopGlove);

        functionDelegate rth = new functionDelegate(PressRTH);
        functionDelegate showall = new functionDelegate(PressShowAll);
        functionDebug.Instance.registerFunction("toggleRTH", rth);
        functionDebug.Instance.registerFunction("toggleShowAll", showall);
        
        coroutine = GetUserPOSLoop(TICKTIME);
        StartCoroutine(coroutine);

        // This might be broken, make unique MyDelegates
        MyDelegate RearviewON = new MyDelegate(PressRearviewON);
        ht.registerCollider(rearviewONButton.GetComponent<Collider>().name, RearviewON);

        MyDelegate RearviewOFF = new MyDelegate(PressRearviewOFF);
        ht.registerCollider(rearviewOFFButton.GetComponent<Collider>().name, RearviewOFF);

        MyDelegate RTH = new MyDelegate(PressRTH);
        ht.registerCollider(rthButton.GetComponent<Collider>().name, RTH);

        MyDelegate ShowAll = new MyDelegate(PressShowAll);
        ht.registerCollider(showallButton.GetComponent<Collider>().name, ShowAll);

        // set worldcenter
        _world_center.transform.position = _camera.transform.position + _camera.transform.forward * 2.0f;
        DebugManager.Instance.LogUnityConsole("NavManager", "Setting World Center: " + _cube.transform.position);

        //persistentBehavior.UpdateBinding();
        UpdateStatusText();
        
    }

    private void OnDestroy() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
            GameObject temp = Instantiate(waypoint_mesh, tmpPos.position, Quaternion.identity, _world_center.transform);
            temp.SetActive(rth_status || showall_status);
            waypoint_meshes.Add(temp);

            if (userPosCounter >= (NUMOFOBJECTS)-1)
            {
                // save to backup
                // for now just deleate coords
                
                
                InitUserPositions();
                userPosCounter = 0;

            } else { userPosCounter++; }
            
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
        UpdateStatusText();

        foreach (GameObject obj in waypoint_meshes)
        {
            obj.SetActive(rth_status);
        }

    }

    public void PressShowAll()
    {
        // toggle function
        showall_status = !showall_status;
        rth_status = false;
        UpdateStatusText();

        foreach (GameObject obj in waypoint_meshes)
        {
            obj.SetActive(showall_status);
            if (showall_status)
            {
                var col = obj.GetComponent<Renderer>().material.color;
                col.a = 1.0f;
                obj.GetComponent<Renderer>().material.color = col;
            }
        }
    }

    private void UpdateStatusText()
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
    }


    public void PressRearviewON()
    {
        rearviewONButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewOFFButton.GetComponent<Renderer>().material = buttonMat;

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
        rearviewOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewONButton.GetComponent<Renderer>().material = buttonMat;
        
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
        gloveONButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveOFFButton.GetComponent<Renderer>().material = buttonMat;

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

    public void PressGloveOFF()
    {
        gloveOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveONButton.GetComponent<Renderer>().material = buttonMat;

        camerasManager.destroyCam();
        if(GLOVE_CAM.getConnected())
        {
            GLOVE_CAM.stopStream();
        }
        else
        {
            DebugManager.Instance.LogSceneConsole("ERROR: CAMERA DISCONNECTED");
        }
    }

    //ML Code

    /// <summary>
        /// Instantiates a new object with MLPersistentBehavior. The MLPersistentBehavior is
        /// responsible for restoring and saving itself.
        /// </summary>
    //    String timeStamp = DateTime.Now.ToString();
    

    //void CreateContent(Vector3 position, Quaternion rotation)
    //    {
    //        GameObject gameObj = Instantiate(_content, position, rotation);
    //        MLPersistentBehavior persistentBehavior = gameObj.GetComponent<MLPersistentBehavior>();
    //        persistentBehavior.UniqueId = Guid.NewGuid().ToString();
    //        _pointBehaviors.Add(persistentBehavior);
    //        //AddContentListeners(persistentBehavior);
    //    }

    //    /// <summary>
    //    /// Removes the points and destroys its binding to prevent future restoration
    //    /// </summary>
    //    /// <param name="gameObj">Game Object to be removed</param>
    //    void RemoveContent(GameObject gameObj)
    //    {
    //        MLPersistentBehavior persistentBehavior = gameObj.GetComponent<MLPersistentBehavior>();
    //        //RemoveContentListeners(persistentBehavior);
    //        _pointBehaviors.Remove(persistentBehavior);
    //        persistentBehavior.DestroyBinding();
    //        //Instantiate(_destroyedContentEffect, persistentBehavior.transform.position, Quaternion.identity);

    //        Destroy(persistentBehavior.gameObject);
    //    }


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