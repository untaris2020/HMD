using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;



// TODO
// 1) Handling errors of different priorities

public class ErrorHandler : MonoBehaviour {

    //Handles mesh collider button functionalities
    public delegate void Button0Delegate();

    //Public ErrorHandler all objects can reference
    public static ErrorHandler Instance;

    // Use this for initialization
    public GameObject errorPanel;
    public TextMeshProUGUI myText; 

    //public AudioClip error;
      //  AudioSource source;

    public float VOL = 1.7f;

    //ErrorWindow windowInstance;
    private bool windowActive;

    //True if priority of incoming error message is > current
    private bool isHigherPriority;

    private List<string> ErrorList;
    private List<int> PriorityList;

    List<GameObject> gestureMarkers = new List<GameObject>();
    public Collider Page0Col; 
    private bool page0RenderState;

    private void Awake()
    {
        ErrorList = new List<string>();
        PriorityList = new List<int>();
        Instance = this;

    }
    void Start () {
        windowActive = false;
        errorPanel.SetActive(false);
        myText.SetText("");
        //source = GetComponent<AudioSource>();

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();
        // Resiter Colliders

        // Resiter Colliders
        //page0RenderState = false;
        //page1RenderState = false;
        //page2RenderState = false;
        //page3RenderState = false;

        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);

        

        //HandleError(0, "test3");
        Button0Delegate tmpDelegate0 = new Button0Delegate(CloseErrorWindow);
        ht.registerCollider(errorPanel.GetComponent<Collider>().name, Page0Col.name,  tmpDelegate0, input);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate0);

        tmpDelegate0 = new Button0Delegate(onViewTogglePG0);
        ht.registerToggleCollider(Page0Col.name, tmpDelegate0);

        //Debug.Log("Do I run v2");
    }
	
	// Update is called once per frame
	void Update () {
        if(((ErrorList.Count > 0) && (windowActive == false)) || isHigherPriority)
        {

            SpawnErrorWindow();

        }
    }

    public int HandleError(int Priority, string ErrorMsg)
    {
        if (ErrorList.Count == 0)
        {

            //Debug.Log("Do I run lol");
            ErrorList.Add(ErrorMsg);
            PriorityList.Add(Priority);
            //Debug.Log(ErrorList.Count);
            //Debug.Log(ErrorMsg);

        }
        else if(Priority < PriorityList[0])
        {
            isHigherPriority = true;
            ErrorList.Insert(0, ErrorMsg);
            PriorityList.Insert(0, Priority);

        }
        else if(Priority > PriorityList[0])
        {
                
            int PrioLocation = PriorityList.IndexOf(Priority);
            if (PrioLocation == -1)
            {
                 PriorityList.Add(Priority);
                 ErrorList.Add(ErrorMsg);

            }
            else
            {
                PriorityList.Insert(PrioLocation, Priority);
                ErrorList.Insert(PrioLocation, ErrorMsg);

            }
                    

        }
        else
        {
            int PrioLocation = PriorityList.IndexOf(Priority);
            ErrorList.Insert(PrioLocation, ErrorMsg);
            PriorityList.Insert(PrioLocation, Priority);

        }
        return 0;
    }

    public void SpawnErrorWindow()
    {
        windowActive = true;
        //float trans = 0f;
        myText.SetText(ErrorList[0]);
        errorPanel.SetActive(true);
        //source.PlayOneShot(error,VOL);

    }

    public void CloseErrorWindow()
    {
        DestroyGestureMarkers();
        ErrorList.RemoveAt(0);
        PriorityList.RemoveAt(0);
        myText.SetText("");
        errorPanel.SetActive(false);
        windowActive = false;
        isHigherPriority = false;
        //Debug.Log("Item at 0" + ErrorList[0]);
 

        // fade out
        //float trans = 1.0f;
       // if (trans > 0.0)
       // {
           
       //     trans -= 0.03f;
       // }
        // destroy object
       // Destroy(windowInstance.gameObject);

    }

    public void onViewTogglePG0()
    {
        page0RenderState = !page0RenderState;

        if (page0RenderState) {
            InstantiateGestureMarkers();
        } else {
            FadeOutGestureMarkers();
        }

    }

    Transform[] obj;
    void InstantiateGestureMarkers() {
        if (!InputSystemStatus.Instance.GetShowGestureIndicators()) {
            return;
        }

        DestroyGestureMarkers();
        obj = GetComponentsInChildren<Transform> (true);

        

        // get all buttons if any
        foreach (var ob in obj.Where(ob => (ob != transform))) {
            int gesture_number = -1;

            if (ob.CompareTag("gesture_1")) {
                gesture_number = 1;
            } else if (ob.CompareTag("gesture_2")) {
                gesture_number = 2;
            } else if (ob.CompareTag("gesture_3")) {
                gesture_number = 3;
            } else if (ob.CompareTag("gesture_4")) {
                gesture_number = 4;
            } else if (ob.CompareTag("gesture_sidescroll")) {
                gesture_number = 5;
            }

            if (gesture_number == 5) {
                //side scroll marker
                Debug.Log("sidescroll");
                GameObject temp = Instantiate(DebugManager.Instance.sideScrollMarker, ob.transform);
                // set offset
                temp.transform.localScale = new Vector3(0.08333334f, 0.08333334f, 0.08333334f);
                temp.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                //temp.transform.localPosition = new Vector3(0.0101f, 0.00151f, -0.102f); // this is wrong
                //temp.transform.position 
                gestureMarkers.Add(temp);
            } else if (gesture_number != -1) {
                // regurlar number marker

                GameObject temp = Instantiate(DebugManager.Instance.gestureMarkers[gesture_number - 1], ob.transform);
                // set offset
                temp.transform.localScale = new Vector3(0.0015f, 0.002f, 0.002f);
                temp.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
                temp.transform.localPosition = new Vector3(0.0101f, 0.00151f, -0.102f);
                //temp.transform.position 
                gestureMarkers.Add(temp);
            }
        }
    }

    void FadeOutGestureMarkers() {

        //Debug.Log(gestureMarkers.Count);
        //Debug.Log("Fading out...");
        foreach (GameObject obj in gestureMarkers) {
            if (obj != null) {
                FadeInOut temp = obj.GetComponent<FadeInOut>();
                temp.FadeOut();
            }
        }
    }

    void DestroyGestureMarkers() {
        foreach (GameObject obj in gestureMarkers) {
            // can you destroy a null object? idk might need to null check
            Destroy(obj);
        }
        gestureMarkers.Clear();
    }
}
    





        // spawn window with countdown bar
       // Debug.Log("INFO: Error Window Spawned");
       // windowInstance = Instantiate(ErrorPrefab, transform.position, Quaternion.identity) as ErrorWindow;

        // set paramaters
       // windowInstance.SetText(ErrorMsg);
       // windowInstance.transform.position = new Vector3(0.0f, 0.08f, 0.5f);
       // windowInstance.transform.localScale = new Vector3(0.25f, 0.25f, 0.0999f);

        // fade in
        // TODO 2
        //if (trans < 1.0)
        //{
            // windowInstance.GetComponent<Renderer>().material.color.a = trans;
        //    trans += 0.001f;
        //}



    // old countdown code
    /*
    IEnumerator ErrorWindowTimeout()
    {
        float TimeLeft = 100.0f;
        float trans = 0f;
        while (TimeLeft > 0)
        {
            // fade in
            // TODO 2
            if (trans < 1.0)
            {
               // windowInstance.GetComponent<Renderer>().material.color.a = trans;
                trans += 0.001f;
            }
            
            yield return new WaitForSeconds(0.05f);
            TimeLeft--;
        }
        
        CloseErrorWindow();
    }
    */