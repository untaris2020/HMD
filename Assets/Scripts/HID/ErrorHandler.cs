using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



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

    private void Awake()
    {
        ErrorList = new List<string>();
        PriorityList = new List<int>();
        Instance = this;

    }
    void Start () {
        windowActive = false;
        //source = GetComponent<AudioSource>();

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();
        // Resiter Colliders


        Button0Delegate tmpDelegate0 = new Button0Delegate(CloseErrorWindow);
        ht.registerCollider(errorPanel.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate0);

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
            Debug.Log(ErrorList.Count);
            Debug.Log(ErrorMsg);

        }
        else if(Priority < PriorityList[0])
        {
            isHigherPriority = true;
            ErrorList.Add(ErrorMsg);
            PriorityList.Add(Priority);

        }
        else if(Priority > PriorityList[0])
        {
            
                int PrioLocation = PriorityList.IndexOf(Priority);
                PriorityList.Insert(PrioLocation - 1, Priority);
                ErrorList.Insert(PrioLocation - 1, ErrorMsg);

        }
        else
        {
            ErrorList.Add(ErrorMsg);
            PriorityList.Add(Priority);

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
        ErrorList.RemoveAt(0);
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