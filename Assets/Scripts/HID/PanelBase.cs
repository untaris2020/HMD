using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PanelBase : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    public bool Verbose;

    public StyleSheet style;

    public GameObject[] panels;
    public GameObject[] pages;
    List<GameObject> gestureMarkers = new List<GameObject>();

    protected int pageIndex;
    //public string panelName; 
    public delegate void Button0Delegate();
    public delegate void Button1Delegate();
    public delegate void Button2Delegate();
    public delegate void Button3Delegate();

    public Collider Page0Col; 
    public Collider Page1Col;
    public Collider Page2Col;
    public Collider Page3Col;

    private bool page0RenderState;
    private bool page1RenderState;
    private bool page2RenderState;
    private bool page3RenderState;

    protected virtual void Start() 
    {

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();
        // Resiter Colliders
        page0RenderState = false;
        page1RenderState = false;
        page2RenderState = false;
        page3RenderState = false;

        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 0, 0, 0, 1);

        Button0Delegate tmpDelegate0 = new Button0Delegate(Button0Press);
        ht.registerCollider(panels[0].GetComponent<Collider>().name, Page3Col.name, tmpDelegate0, input);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0",tmpDelegate0);

        tmpDelegate0 = new Button0Delegate(onViewTogglePG0);
        ht.registerToggleCollider(Page0Col.name, tmpDelegate0);

        Button1Delegate tmpDelegate1 = new Button1Delegate(Button1Press);
        ht.registerCollider(panels[1].GetComponent<Collider>().name, Page0Col.name, tmpDelegate1, input);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab1",tmpDelegate1);

        tmpDelegate0 = new Button0Delegate(onViewTogglePG1);
        ht.registerToggleCollider(Page1Col.name, tmpDelegate0);

        Button2Delegate tmpDelegate2 = new Button2Delegate(Button2Press);
        ht.registerCollider(panels[2].GetComponent<Collider>().name, Page1Col.name, tmpDelegate2, input);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab2",tmpDelegate2);

        tmpDelegate0 = new Button0Delegate(onViewTogglePG2);
        ht.registerToggleCollider(Page2Col.name, tmpDelegate0);

        Button3Delegate tmpDelegate3 = new Button3Delegate(Button3Press);
        ht.registerCollider(panels[3].GetComponent<Collider>().name, Page2Col.name, tmpDelegate3, input);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab3",tmpDelegate3);

        tmpDelegate0 = new Button0Delegate(onViewTogglePG3);
        ht.registerToggleCollider(Page3Col.name, tmpDelegate0);
        

        pageIndex = -1;

        Button0Press();
    }

    // Getters
    public int GetIndex() { return pageIndex; }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Button0Press()
    {
        LoadPage(0);
    }

    public void Button1Press()
    {
        LoadPage(1);
    }

    public void Button2Press()
    {
        LoadPage(2);
    }

    public void Button3Press()
    {
        LoadPage(3);
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

    public void onViewTogglePG1()
    {
        page1RenderState = !page1RenderState;

        if (page1RenderState) {
            InstantiateGestureMarkers();
        } else {
            FadeOutGestureMarkers();
        }
    }

    Transform[] obj;
    public void onViewTogglePG2()
    {
        page2RenderState = !page2RenderState;

        if (page2RenderState) {
            InstantiateGestureMarkers();
        } else {
            FadeOutGestureMarkers();
        }
    }

    public void onViewTogglePG3()
    {
        page3RenderState = !page3RenderState;

        if (page3RenderState) {
            InstantiateGestureMarkers();
        } else {
            FadeOutGestureMarkers();
        }
    }

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
                temp.transform.localPosition = new Vector3(-0.4575001f, -0.064f, -0.05166666f);
                //temp.transform.position 
                gestureMarkers.Add(temp);
            } else if (gesture_number != -1) {
                // regurlar number marker

                GameObject temp = Instantiate(DebugManager.Instance.gestureMarkers[gesture_number - 1], ob.transform);
                // set offset
                temp.transform.localScale = new Vector3(0.0015f, 0.002f, 0.002f);
                temp.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
                temp.transform.localPosition = new Vector3(-0.00252f, 0.00203f, 0.099f);
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

    public void LoadPage(int page)
    {
        if (page == pageIndex) {
            // page is the same, dont reload
            return;
        }

        DestroyGestureMarkers();
        

        // clean up null elements occasionally
        //gestureMarkers.RemoveAll(item => item == null);

        pageIndex = page;
        
        if(Verbose)
        {
            DebugManager.Instance.LogUnityConsole(this.GetType().Name, "Page " + page + " Loaded.");
        }

        // Set all buttons material to inactive
        foreach (GameObject obj in panels)
        {
            obj.GetComponent<MeshRenderer>().material = style.ButtonInactiveMat;
        }

        foreach (GameObject obj in pages)
        {
            //obj.GetComponent<Renderer>().enabled = false;
            obj.SetActive(false);   
        }

        // check page is valid
        if (page < 0 || page > 3)
        {
            DebugManager.Instance.LogUnityConsole(this.GetType().Name, ": Wrong page value - " + page);
        } else
        {
            // set selected page active material and the page active
            panels[page].GetComponent<MeshRenderer>().material = style.ButtonActiveMat;
            //pages[page].GetComponent<Renderer>().enabled = true;
            pages[page].SetActive(true);
        }
    }
}
