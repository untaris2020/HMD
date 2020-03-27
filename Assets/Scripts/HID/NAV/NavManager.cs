using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class NavManager : MonoBehaviour
{
    // TODO 
    // 1) create backup system
    // 2) fix persistant behivior

    public GameObject rearviewONButton, rearviewOFFButton, gloveONButton, gloveOFFButton;
    public Material buttonMat, buttonHoverMat, headerMat, headerHoverMat;
    public CamerasManager camerasManager;

    public delegate void MyDelegate();
    private delegate void functionDelegate();

    // Nav System
    public MLPersistentBehavior persistentBehavior;
    public GameObject waypoint_mesh;
    List<GameObject> waypoint_meshes = new List<GameObject>();

    GameObject _content = null;
    List<MLPersistentBehavior> _pointBehaviors = new List<MLPersistentBehavior>();


    List<UserPosition> userPositions = new List<UserPosition>();

    List<Waypoint> waypoints = new List<Waypoint>();


    public GameObject _cube, _camera;
    

    private IEnumerator coroutine;
    private float TICKTIME = 1.0f;   //was 10.0f
    private float BACKUPTIMESECONDS = 20.0f;   // amount of time between backups was 300 (5mins)
    private int NUMOFOBJECTS = 0; 
    private int userPosCounter;
    private bool update_waypoints_rth = false;

    //private IEnumerator coroutine;
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        NUMOFOBJECTS = (int)(BACKUPTIMESECONDS / TICKTIME);
        // Allocate space
        InitUserPositions();

        userPosCounter = 0;

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        functionDelegate startRear = new functionDelegate(PressRearviewON);
        functionDelegate stopRear = new functionDelegate(PressRearviewOFF);
        functionDebug.Instance.registerFunction("startRearCam", startRear);
        functionDebug.Instance.registerFunction("stopRearCam", stopRear);

        functionDelegate startGlove = new functionDelegate(PressGloveON);
        functionDelegate stopGlove = new functionDelegate(PressGloveOFF);
        functionDebug.Instance.registerFunction("startGloveCam", startGlove);
        functionDebug.Instance.registerFunction("stopGloveCam", stopGlove);
        
        coroutine = GetUserPOSLoop(TICKTIME);
        StartCoroutine(coroutine);

        // This might be broken, make unique MyDelegates
        MyDelegate RearviewON = new MyDelegate(PressRearviewON);
        ht.registerCollider(rearviewONButton.GetComponent<Collider>().name,RearviewON);

        MyDelegate RearviewOFF = new MyDelegate(PressRearviewOFF);
        ht.registerCollider(rearviewOFFButton.GetComponent<Collider>().name,RearviewOFF);

        // set worldcenter
        _cube.transform.position = _camera.transform.position + _camera.transform.forward * 2.0f;
        DebugManager.Instance.LogUnityConsole("NavManager", "Setting World Center: " + _cube.transform.position);
        
        //persistentBehavior.UpdateBinding();
        ReturnToHome(true);
    }

    private void OnDestroy() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (update_waypoints_rth)
        {
            foreach (GameObject obj in waypoint_meshes)
            {
                var col = obj.GetComponent<Renderer>().material.color;
                col.a = 1.0f / ((float)(_camera.transform.position - obj.transform.position).magnitude / 10.0f);
                obj.GetComponent<Renderer>().material.color = col;
            }
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
            GameObject temp = Instantiate(waypoint_mesh, tmpPos.position, Quaternion.identity);
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

    private void ShowAllUserPositions(bool state)
    {
        if (state)
        {
            foreach (GameObject obj in waypoint_meshes)
            {
                obj.SetActive(state);
            }
        }
    }

    private void ReturnToHome(bool state)
    {
        ShowAllUserPositions(state);
        update_waypoints_rth = state;
    }


    public void PressRearviewON()
    {
        rearviewONButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewOFFButton.GetComponent<Renderer>().material = buttonMat;

        // spawn camera
        camerasManager.spawnCam();
        TCPServer.Instance.getHeadCam().startStream();
    }

    public void PressRearviewOFF()
    {
        rearviewOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewONButton.GetComponent<Renderer>().material = buttonMat;
        
        //Despawn camera
        camerasManager.destroyCam();
        TCPServer.Instance.getHeadCam().stopStream();
    }

    public void PressGloveON()
    {
        gloveONButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveOFFButton.GetComponent<Renderer>().material = buttonMat;

        camerasManager.spawnCam();
        TCPServer.Instance.getGloveCam().startStream();
    }

    public void PressGloveOFF()
    {
        gloveOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveONButton.GetComponent<Renderer>().material = buttonMat;

        camerasManager.destroyCam();
        TCPServer.Instance.getGloveCam().stopStream();
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