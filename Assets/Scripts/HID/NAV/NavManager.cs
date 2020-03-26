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
    private float TICKTIME = 0.50f;   //was 10.0f
    private float BACKUPTIMESECONDS = 5.0f;   // amount of time between backups was 300 (5mins)
    private int NUMOFOBJECTS = 0; 
    private int userPosCounter;

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
    }

    private void OnDestroy() {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GetUserPOSLoop(float waitTime)
    {
        
        while (true)
        {
            UserPosition tmpPos = new UserPosition(DateTime.Now, _camera.transform.position);
            //_cube.transform.rotation = _camera.transform.rotation;

            //DebugManager.Instance.LogUnityConsole("NavManager", "New Coordniate: " + tmpPos.position);
            userPositions[userPosCounter] = tmpPos;

            if (userPosCounter >= (NUMOFOBJECTS)-1)
            {
                // save to backup
                // for now just deleate coords
                ShowAllUserPositions(true);
                
                
                
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
        for (int i=0; i<NUMOFOBJECTS; i++)
        {
            userPositions.Add(new UserPosition());
        }
    }

    private void ShowAllUserPositions(bool state)
    {
        foreach (GameObject obj in waypoint_meshes)
        {
            Destroy(obj);
        }
        
        waypoint_meshes.Clear();


        if (state)
        {
            foreach (UserPosition pos in userPositions)
            {
                GameObject temp = Instantiate(waypoint_mesh, pos.position, Quaternion.identity);
                waypoint_meshes.Add(temp);
            }
        }   
    }

    private void ReturnToHome(bool state)
    {
        foreach (GameObject obj in waypoint_meshes)
        {
            Destroy(obj);
        }
        
        waypoint_meshes.Clear();

        if (state)
        {
            foreach (UserPosition pos in userPositions)
            {
                GameObject temp = Instantiate(waypoint_mesh, pos.position, Quaternion.identity);

                // change marker opacity based on most recent marker
                var col = temp.GetComponent<Renderer>().material.color;
                //NUMOFOBJECTS
                col.a = 1.0f / (float)(DateTime.Now - pos.timestamp).TotalSeconds;

                waypoint_meshes.Add(temp);
            }
        }  
    }


    public void PressRearviewON()
    {
        rearviewONButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewOFFButton.GetComponent<Renderer>().material = buttonMat;

        // spawn camera
        camerasManager.spawnCam();
    }

    public void PressRearviewOFF()
    {
        rearviewOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewONButton.GetComponent<Renderer>().material = buttonMat;
        
        //Despawn camera
        camerasManager.destroyCam();
    }

    public void PressGloveON()
    {
        gloveONButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveOFFButton.GetComponent<Renderer>().material = buttonMat;

        camerasManager.spawnCam();
    }

    public void PressGloveOFF()
    {
        gloveOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveONButton.GetComponent<Renderer>().material = buttonMat;

        camerasManager.destroyCam();
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