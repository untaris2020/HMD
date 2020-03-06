using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class NavManager : MonoBehaviour
{
    public GameObject rearviewONButton, rearviewOFFButton, gloveONButton, gloveOFFButton;
    public Material buttonMat, buttonHoverMat, headerMat, headerHoverMat;
    public CamerasManager camerasManager;

    public delegate void MyDelegate();

    // Nav System
    public MLPersistentBehavior persistentBehavior;
    GameObject _content = null;
    List<MLPersistentBehavior> _pointBehaviors = new List<MLPersistentBehavior>();

    public GameObject _cube, _camera;
    private MLInputController _controller;

    private IEnumerator coroutine;
    private float LOOPTIME = 10.0f;

    //private IEnumerator coroutine;
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        
        coroutine = GetUserPOSLoop(LOOPTIME);
        StartCoroutine(coroutine);

        // This might be broken, make unique MyDelegates
        MyDelegate RearviewON = new MyDelegate(PressRearviewON);
        ht.registerCollider(rearviewONButton.GetComponent<Collider>().name,RearviewON);

        MyDelegate RearviewOFF = new MyDelegate(PressRearviewOFF);
        ht.registerCollider(rearviewOFFButton.GetComponent<Collider>().name,RearviewOFF);

        MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Left);

        //Set Worldcenter

        _cube.transform.position = _camera.transform.position + _camera.transform.forward * 2.0f;
        DebugManager.Instance.LogUnityConsole("NavManager", "Setting World Center: " + _cube.transform.position);
        persistentBehavior.UpdateBinding();
    }

    private void OnDestroy() {
        MLInput.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        //_cube.transform.position = SOMETHING...
        // PCF Testing
        //if (_controller != null && _controller.TriggerValue > 0.2f) {
            
        //    persistentBehavior.UpdateBinding();
            
        //    //persistentBehavior.
        //}
    }

    private IEnumerator GetUserPOSLoop(float waitTime)
    {

        while (true)
        {
            _cube.transform.position = _camera.transform.position + _camera.transform.forward * 2.0f;
            //_cube.transform.rotation = _camera.transform.rotation;

            DebugManager.Instance.LogUnityConsole("NavManager", "New Coordniate: " + _cube.transform.position);
            yield return new WaitForSeconds(waitTime);
        }
    }


    public void PressRearviewON()
    {
        rearviewONButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewOFFButton.GetComponent<Renderer>().material = buttonMat;

        // spawn camera
        camerasManager.SpawnRearviewCam();
    }

    public void PressRearviewOFF()
    {
        rearviewOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        rearviewONButton.GetComponent<Renderer>().material = buttonMat;
        
        //Despawn camera
        camerasManager.DestroyRearviewCam();
    }

    public void PressGloveON()
    {
        gloveONButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveOFFButton.GetComponent<Renderer>().material = buttonMat;
    }

    public void PressGloveOFF()
    {
        gloveOFFButton.GetComponent<Renderer>().material = buttonHoverMat;
        gloveONButton.GetComponent<Renderer>().material = buttonMat;
    }

    //ML Code

    /// <summary>
        /// Instantiates a new object with MLPersistentBehavior. The MLPersistentBehavior is
        /// responsible for restoring and saving itself.
        /// </summary>
        String timeStamp = DateTime.Now.ToString();
    

    void CreateContent(Vector3 position, Quaternion rotation)
        {
            GameObject gameObj = Instantiate(_content, position, rotation);
            MLPersistentBehavior persistentBehavior = gameObj.GetComponent<MLPersistentBehavior>();
            persistentBehavior.UniqueId = Guid.NewGuid().ToString();
            _pointBehaviors.Add(persistentBehavior);
            //AddContentListeners(persistentBehavior);
        }

        /// <summary>
        /// Removes the points and destroys its binding to prevent future restoration
        /// </summary>
        /// <param name="gameObj">Game Object to be removed</param>
        void RemoveContent(GameObject gameObj)
        {
            MLPersistentBehavior persistentBehavior = gameObj.GetComponent<MLPersistentBehavior>();
            //RemoveContentListeners(persistentBehavior);
            _pointBehaviors.Remove(persistentBehavior);
            persistentBehavior.DestroyBinding();
            //Instantiate(_destroyedContentEffect, persistentBehavior.transform.position, Quaternion.identity);

            Destroy(persistentBehavior.gameObject);
        }


}
