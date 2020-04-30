using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics;

public class MissionPanelManager : MonoBehaviour
{
    [SerializeField] private string MissionServerURL;
    private string InputJSON;
    public MissionContainer MissionContainerInstance;
    public TextAsset textFile;
    public StyleSheet style;

    bool isScriptLoaded = false;

    List<GameObject> gestureMarkers = new List<GameObject>();

    public delegate void Button0Delegate();

    public GameObject Page0Col;
    private bool page0RenderState;

    //Arrow game object
    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject upArrowR;
    public GameObject downArrowR;

    public GameObject button1C;
    public GameObject button2C;
    public GameObject button3C;
    public GameObject button4C;
    public GameObject button5C;
    public GameObject[] panels;
    //public GameObject panel1;
    //public GameObject panel2;
    //public GameObject panel3;
    //public GameObject panel4;

    //Text fields that need to be updated
    public TextMeshProUGUI button1;
    public TextMeshProUGUI button2;
    public TextMeshProUGUI button3;
    public TextMeshProUGUI button4;
    public TextMeshProUGUI button5;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Index;
    public TextMeshProUGUI Page;


    private IEnumerator coroutine;

    //Set count of all tasks
    //int SMissionCount = 0;
    //public List<int> MissionCount;
    //public List<List<int>> TaskCount;
    //public List<List<List<int>>> STask;



    //Flags for mission selection
    bool SuperMissionFlag = true;
    bool MissionFlag = false;
    bool TaskFlag = false;
    bool SubTaskFlag = false;
    bool displayPicture = false; //used for when we want to display pictures when subtask is selected

    //PanelFlags -- Tells me which panel is selected and what should be displayed


    //Current Selections
    public int SuperMissionNumber;
    public int MissionNumber;
    public int TaskNumber;
    public int SubTaskNumber;

    //Offets for displaying missions if a user wants to view other missions
    public int SMOffset = 0;
    public int MOffset = 0;
    public int TOffset = 0;
    public int STOffset = 0;

    //public List<int> SuperMissions;
    forceSensorManager.fingerInput input;
    public int currentHighlighted;
    public int currentPage;
    Transform[] obj;

    int counter = 1;

    public bool force;

    // Use this for initialization
    void Start()
    {
        //coroutine = GetUserPOSLoop(3);
        //StartCoroutine(coroutine);
        force = false;
        page0RenderState = false;
        currentHighlighted = 0;
        currentPage = -1;
        createColliders();
        //Set default selections to 0 on start
        SuperMissionNumber = 0;
        MissionNumber = 0;
        TaskNumber = 0;
        SubTaskNumber = 0;


        //LoadPage(0);
        panel1Press();
        isScriptLoaded = true;

        upArrow.SetActive(false);



    }

    void Awake()
    {
        string InputJSON = textFile.text;
        //DebugManager.Instance.LogUnityConsole("JSON INPUT: " + InputJSON);

        MissionContainerInstance = MissionContainer.CreateFromJSON(InputJSON);
    }

    void Update()
    {
        if (SuperMissionFlag)
        {
            displaySuperMissions();
        }
        else if (MissionFlag)
        {
            displayMissions();
        }
        else if (TaskFlag)
        {
            displayTasks();
        }
        else if (SubTaskFlag)
        {
            displaySubTasks();
        }

        if(currentHighlighted == 0)
        {
            button1C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.Highlighted;
            button2C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button3C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button4C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button5C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
        }
        else if(currentHighlighted == 1)
        {
            button1C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button2C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.Highlighted;
            button3C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button4C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button5C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
        }
        else if (currentHighlighted == 2)
        {
            button1C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button2C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button3C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.Highlighted;
            button4C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button5C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
        }
        else if (currentHighlighted == 3)
        {
            button1C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button2C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button3C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button4C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.Highlighted;
            button5C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
        }
        else if (currentHighlighted == 4)
        {
            button1C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button2C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button3C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button4C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            button5C.GetComponent<MeshRenderer>().material = StyleSheet.Instance.Highlighted;
        }
        //MissionContainerInstance.printData();

    }

    private IEnumerator GetUserPOSLoop(float waitTime)
    {
        while (true)
        {
            if (counter == 1)
            {

                input = new forceSensorManager.fingerInput(0, 0, 0, 0, 1);
                //DebugManager.Instance.LogUnityConsole("Counter 1");

            }
            if (counter == 2)
            {
                input = new forceSensorManager.fingerInput(0, 0, 0, 1, 0);
                //DebugManager.Instance.LogUnityConsole("Counter 2");
            }
            if (counter == 3)
            {
                input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
                //DebugManager.Instance.LogUnityConsole("Counter 3");
            }

            yield return new WaitForSeconds(waitTime);
            counter += 1;
        }


    }

    public void displaySuperMissions()
    {
        int SMissionCount = MissionContainerInstance.SuperMissions.Count;



        if (SMissionCount - SMOffset <= 5)
        {
            downArrowR.SetActive(false);
            downArrow.SetActive(false);

        }
        else
        {
            downArrowR.SetActive(true);
            downArrow.SetActive(true);
        }

        //Need to only disable the arrow instead of the entire bar
        if (SMOffset == 0)
        {
            Title.SetText("Super Missions");
            upArrowR.SetActive(false);
            upArrow.SetActive(false);
        }
        else
        {
            Title.SetText("");
            upArrowR.SetActive(true);
            upArrow.SetActive(true);
        }




        if (SMissionCount - SMOffset > 0)
            button1.SetText(MissionContainerInstance.SuperMissions[0 + SMOffset].SuperMissionText);
        if (SMissionCount - SMOffset > 1)
            button2.SetText(MissionContainerInstance.SuperMissions[1 + SMOffset].SuperMissionText);
        if (SMissionCount - SMOffset > 2)
            button3.SetText(MissionContainerInstance.SuperMissions[2 + SMOffset].SuperMissionText);
        if (SMissionCount - SMOffset > 3)
            button4.SetText(MissionContainerInstance.SuperMissions[3 + SMOffset].SuperMissionText);
        if (SMissionCount - SMOffset > 4)
            button5.SetText(MissionContainerInstance.SuperMissions[4 + SMOffset].SuperMissionText);

        if (SMissionCount - SMOffset == 1)
        {
            button2.SetText("");
            button3.SetText("");
            button4.SetText("");
            button5.SetText("");
        }
        else if (SMissionCount - SMOffset == 2)
        {
            button3.SetText("");
            button4.SetText("");
            button5.SetText("");
        }
        else if (SMissionCount - SMOffset == 3)
        {
            button4.SetText("");
            button5.SetText("");
        }
        else if (SMissionCount - SMOffset == 4)
        {
            button5.SetText("");
        }

        Index.SetText("Index");
        int pages = 1;
        if (SMissionCount > 5)
            pages = SMissionCount / 5 + 1;

        if (SMOffset == 0)
            Page.SetText("Page 1/" + pages);
        else if (SMOffset == 5)
            Page.SetText("Page 2/" + pages);
        else if (SMOffset == 10)
            Page.SetText("Page 3/" + pages);
        else if (SMOffset == 15)
            Page.SetText("Page 4/" + pages);

    }

    public void displayMissions()
    {
        int MissionCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions.Count;

        if (MissionCount - MOffset <= 5)
        {
            downArrowR.SetActive(false);
            downArrow.SetActive(false);

        }
        else
        {
            downArrowR.SetActive(true);
            downArrow.SetActive(true);
        }

        if (MOffset == 0)
        {
            Title.SetText("Missions");
            upArrowR.SetActive(false);
            upArrow.SetActive(false);
        }
        else
        {
            Title.SetText("");
            upArrowR.SetActive(true);
            upArrow.SetActive(true);
        }

        if (MissionCount - MOffset > 0)
            button1.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[0 + MOffset].MissionText);
        if (MissionCount - MOffset > 1)
            button2.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[1 + MOffset].MissionText);
        if (MissionCount - MOffset > 2)
            button3.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[2 + MOffset].MissionText);
        if (MissionCount - MOffset > 3)
            button4.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[3 + MOffset].MissionText);
        if (MissionCount - MOffset > 4)
            button5.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[4 + MOffset].MissionText);

        if (MissionCount - MOffset == 1)
        {
            button2.SetText("");
            button3.SetText("");
            button4.SetText("");
            button5.SetText("");
        }
        else if (MissionCount - MOffset == 2)
        {
            button3.SetText("");
            button4.SetText("");
            button5.SetText("");
        }
        else if (MissionCount - MOffset == 3)
        {
            button4.SetText("");
            button5.SetText("");
        }
        else if (MissionCount - MOffset == 4)
        {
            button5.SetText("");
        }

        Index.SetText(SuperMissionNumber + 1 + ".");

        int pages = 1;
        if (MissionCount > 5)
            pages = MissionCount / 5 + 1;

        if (MOffset == 0)
            Page.SetText("Page 1/" + pages);
        else if (MOffset == 5)
            Page.SetText("Page 2/" + pages);
        else if (MOffset == 10)
            Page.SetText("Page 3/" + pages);
        else if (MOffset == 15)
            Page.SetText("Page 4/" + pages);

    }

    public void displayTasks()
    {
        force = false;
        int taskCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks.Count;

        if (taskCount - TOffset <= 5)
        {
            downArrowR.SetActive(false);
            downArrow.SetActive(false);

        }
        else
        {
            downArrowR.SetActive(true);
            downArrow.SetActive(true);
        }

        if (TOffset == 0)
        {
            Title.SetText("Tasks");
            upArrowR.SetActive(false);
            upArrow.SetActive(false);
        }
        else
        {
            Title.SetText("");
            upArrowR.SetActive(true);
            upArrow.SetActive(true);
        }


        if (taskCount - TOffset > 0)
            button1.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[0 + TOffset].TaskText);
        if (taskCount - TOffset > 1)
            button2.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[1 + TOffset].TaskText);
        if (taskCount - TOffset > 2)
            button3.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[2 + TOffset].TaskText);
        if (taskCount - TOffset > 3)
            button4.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[3 + TOffset].TaskText);
        if (taskCount - TOffset > 4)
            button5.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[4 + TOffset].TaskText);

        if (taskCount - TOffset == 1)
        {
            button2.SetText("");
            button3.SetText("");
            button4.SetText("");
            button5.SetText("");
        }
        else if (taskCount - TOffset == 2)
        {
            button3.SetText("");
            button4.SetText("");
            button5.SetText("");
        }
        else if (taskCount - TOffset == 3)
        {
            button4.SetText("");
            button5.SetText("");
        }
        else if (taskCount - TOffset == 4)
        {
            button5.SetText("");
        }

        Index.SetText(SuperMissionNumber + 1 + "." + (MissionNumber + 1) + ".");

        int pages = 1;
        if (taskCount > 5)
            pages = taskCount / 5 + 1;

        if (TOffset == 0)
            Page.SetText("Page 1/" + pages);
        else if (TOffset == 5)
            Page.SetText("Page 2/" + pages);
        else if (TOffset == 10)
            Page.SetText("Page 3/" + pages);
        else if (TOffset == 15)
            Page.SetText("Page 4/" + pages);

    }

    public void displaySubTasks()
    {
        int StaskCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[TaskNumber].SubTasks.Count;

        if (StaskCount - STOffset <= 5)
        {
            downArrowR.SetActive(false);
            downArrow.SetActive(false);

        }
        else
        {
            downArrowR.SetActive(true);
            downArrow.SetActive(true);
        }


        if (STOffset == 0)
        {
            Title.SetText("Sub-Tasks");
            upArrowR.SetActive(false);
            upArrow.SetActive(false);
        }
        else
        {
            Title.SetText("");
            upArrowR.SetActive(true);
            upArrow.SetActive(true);
        }



        if (StaskCount - STOffset > 0)
            button1.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[TaskNumber].SubTasks[0 + STOffset].SubTaskText);
        if (StaskCount - STOffset > 1)
            button2.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[TaskNumber].SubTasks[1 + STOffset].SubTaskText);
        if (StaskCount - STOffset > 2)
            button3.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[TaskNumber].SubTasks[2 + STOffset].SubTaskText);
        if (StaskCount - STOffset > 3)
            button4.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[TaskNumber].SubTasks[3 + STOffset].SubTaskText);
        if (StaskCount - STOffset > 4)
            button5.SetText(MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[TaskNumber].SubTasks[4 + STOffset].SubTaskText);

        if (StaskCount - STOffset == 1)
        {
            button2.SetText("");
            button3.SetText("");
            button4.SetText("");
            button5.SetText("");
        }
        else if (StaskCount - STOffset == 2)
        {
            button3.SetText("");
            button4.SetText("");
            button5.SetText("");
        }
        else if (StaskCount - STOffset == 3)
        {
            button4.SetText("");
            button5.SetText("");
        }
        else if (StaskCount - STOffset == 4)
        {
            button5.SetText("");
        }

        Index.SetText(SuperMissionNumber + 1 + "." + (MissionNumber + 1) + "." + (TaskNumber + 1) + ".");

        int pages = 1;
        if (StaskCount > 5)
            pages = StaskCount / 5 + 1;

        if (STOffset == 0)
            Page.SetText("Page 1/" + pages);
        else if (STOffset == 5)
            Page.SetText("Page 2/" + pages);
        else if (STOffset == 10)
            Page.SetText("Page 3/" + pages);
        else if (STOffset == 15)
            Page.SetText("Page 4/" + pages);

    }

    public void createColliders()
    {
        
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        //forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 0, 0, 0, 1);
        Button0Delegate tmpDelegate0 = new Button0Delegate(nextPage);
        ht.registerCollider(downArrow.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate0);

        Button0Delegate tmpDelegate1 = new Button0Delegate(backPage);
        ht.registerCollider(upArrow.GetComponent<Collider>().name, tmpDelegate1);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate1);

        Button0Delegate tmpDelegate2 = new Button0Delegate(button1Press);
        ht.registerCollider(button1C.GetComponent<Collider>().name, tmpDelegate2);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate2);

        Button0Delegate tmpDelegate3 = new Button0Delegate(button2Press);
        ht.registerCollider(button2C.GetComponent<Collider>().name, tmpDelegate3);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate3);

        Button0Delegate tmpDelegate4 = new Button0Delegate(button3Press);
        ht.registerCollider(button3C.GetComponent<Collider>().name, tmpDelegate4);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate4);

        Button0Delegate tmpDelegate5 = new Button0Delegate(button4Press);
        ht.registerCollider(button4C.GetComponent<Collider>().name, tmpDelegate5);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate5);

        Button0Delegate tmpDelegate6 = new Button0Delegate(button5Press);
        ht.registerCollider(button5C.GetComponent<Collider>().name, tmpDelegate6);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate6);

        Button0Delegate tmpDelegate7 = new Button0Delegate(panel1Press);
        ht.registerCollider(panels[0].GetComponent<Collider>().name, tmpDelegate7);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate7);

        Button0Delegate tmpDelegate8 = new Button0Delegate(panel2Press);
        ht.registerCollider(panels[1].GetComponent<Collider>().name, tmpDelegate8);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate8);

        Button0Delegate tmpDelegate9 = new Button0Delegate(panel3Press);
        ht.registerCollider(panels[2].GetComponent<Collider>().name, tmpDelegate9);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate9);

        Button0Delegate tmpDelegate10 = new Button0Delegate(panel4Press);
        ht.registerCollider(panels[3].GetComponent<Collider>().name, tmpDelegate10);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate10);

        Button0Delegate tmpDelegate11 = new Button0Delegate(onViewTogglePG0);
        ht.registerToggleCollider(Page0Col.name, tmpDelegate11);

        Button0Delegate tmpDelegate12 = new Button0Delegate(pressCurrentHighlighted);
        input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        ht.registerForceCollider(Page0Col.name + "1", Page0Col.name, tmpDelegate12, input);

        Button0Delegate tmpDelegate13 = new Button0Delegate(upCurrentHighlight);
        input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        ht.registerForceCollider(Page0Col.name + "2", Page0Col.name, tmpDelegate13, input);

        Button0Delegate tmpDelegate14 = new Button0Delegate(downCurrentHighlight);
        input = new forceSensorManager.fingerInput(0, 0, 0, 1, 0);
        ht.registerForceCollider(Page0Col.name + "3", Page0Col.name, tmpDelegate14, input);

        Button0Delegate tmpDelegate15 = new Button0Delegate(forcePanelPress);
        input = new forceSensorManager.fingerInput(0, 0, 0, 0, 1);
        ht.registerForceCollider(Page0Col.name + "4", Page0Col.name, tmpDelegate15, input);


    }
    public void forcePanelPress()
    {
        /*
        if (currentPage == 0)
        {
            currentHighlighted = 0;
            LoadPage(1);
            SuperMissionFlag = false;
            MissionFlag = true;
            TaskFlag = false;
            SubTaskFlag = false;
            //currentPage++;

        }
        else if(currentPage == 1)
        {
            //DebugManager.Instance.LogUnityConsole("Panel2Press");
            currentHighlighted = 0;
            LoadPage(2);
            SuperMissionFlag = false;
            MissionFlag = false;
            TaskFlag = true;
            SubTaskFlag = false;
            //currentPage++;
        }
        else if(currentPage == 2)
        {
            currentHighlighted = 0;
            LoadPage(3);
            SuperMissionFlag = false;
            MissionFlag = false;
            TaskFlag = false;
            SubTaskFlag = true;
            //currentPage++;
        }
        else if(currentPage == 3)
        {
            currentHighlighted = 0;
            LoadPage(0);
            SuperMissionFlag = true;
            MissionFlag = false;
            TaskFlag = false;
            SubTaskFlag = false;
            //currentPage = 0;
        }
        */
        //DebugManager.Instance.LogUnityConsole("Current page: " + currentPage);

        force = true;
            
        pressCurrentHighlighted();




    }

    public void nextPage()
    {

        if (SuperMissionFlag)
            SMOffset += 5;
        else if (MissionFlag)
            MOffset += 5;
        else if (TaskFlag)
            TOffset += 5;
        else
            STOffset += 5;
        currentHighlighted = 0;
    }
    public void backPage()
    {
        if (SuperMissionFlag)
            SMOffset -= 5;
        else if (MissionFlag)
            MOffset -= 5;
        else if (TaskFlag)
            TOffset -= 5;
        else
            STOffset -= 5;
        currentHighlighted = 4;

    }

    public void LoadPage(int page)
    {

        if (isScriptLoaded)
        {
            DestroyGestureMarkers();
            InstantiateGestureMarkers();

        }
        if(page == currentPage)
        {
            return;
        }
        currentPage = page;
        currentHighlighted = 0;
        SMOffset = 0;
        MOffset = 0;
        TOffset = 0;
        STOffset = 0;
        // function overload
        //DebugManager.Instance.LogUnityConsole("Correct Load page");

        //DebugManager.Instance.LogUnityConsole(this.GetType().Name, "Page " + page + " Loaded.");

        //if(Verbose)
        //{

        //}

        // Set all buttons material to inactive
        foreach (GameObject obj in panels)
        {
            obj.GetComponent<MeshRenderer>().material = style.NonHighlighted;
        }

        // set current page as the active material
        panels[page].GetComponent<MeshRenderer>().material = style.Highlighted;




        // add any custom logic here if needed

        //
    }

    public void button1Press()
    {
        

        if (!checkText(0))
        {
            return;
            currentHighlighted = 0;

        }
        DebugManager.Instance.LogUnityConsole("I ran");
        if (SuperMissionFlag)
        {
            SuperMissionNumber = SMOffset;
            MissionFlag = true;
            SuperMissionFlag = false;
            LoadPage(1);
        }
        else if (MissionFlag)
        {

            MissionNumber = MOffset;
            TaskFlag = true;
            MissionFlag = false;
            LoadPage(2);
        }
        else if (TaskFlag)
        {
            TaskNumber = TOffset;
            SubTaskFlag = true;
            TaskFlag = false;
            LoadPage(3);
        }
        else if(force) //subtasks are currently being displayed
        {
            SubTaskNumber = STOffset;
            SubTaskFlag = false;
            SuperMissionFlag = true;
            LoadPage(0);
            force = false;


        }


    }

    public void button2Press()
    {
        currentHighlighted = 1;


        if(!checkText(1))
        {
            return;
            currentHighlighted = 1;

        }
        if (SuperMissionFlag)
        {
            SuperMissionNumber = SMOffset + 1;
            MissionFlag = true;
            SuperMissionFlag = false;
            LoadPage(1);
        }
        else if (MissionFlag)
        {
            MissionNumber = MOffset + 1;
            TaskFlag = true;
            MissionFlag = false;
            LoadPage(2);
        }
        else if (TaskFlag)
        {
            TaskNumber = TOffset + 1;
            SubTaskFlag = true;
            TaskFlag = false;
            LoadPage(3);
        }
        else if (force) //subtasks are currently being displayed
        {
            SubTaskNumber = STOffset;
            SubTaskFlag = false;
            SuperMissionFlag = true;
            LoadPage(0);
            force = false;

        }


    }

    public void button3Press()
    {
        if (!checkText(2))
        {
            return;
            currentHighlighted = 2;

        }
        if (!checkText(2))
            return;
        if (SuperMissionFlag)
        {
            SuperMissionNumber = SMOffset + 2;
            MissionFlag = true;
            SuperMissionFlag = false;
            LoadPage(1);
        }
        else if (MissionFlag)
        {
            MissionNumber = MOffset + 2;
            TaskFlag = true;
            MissionFlag = false;
            LoadPage(2);
        }
        else if (TaskFlag)
        {
            TaskNumber = TOffset + 2;
            SubTaskFlag = true;
            TaskFlag = false;
            LoadPage(3);
        }
        else if (force) //subtasks are currently being displayed
        {
            SubTaskNumber = STOffset;
            SubTaskFlag = false;
            SuperMissionFlag = true;
            LoadPage(0);
            force = false;

        }


    }

    public void button4Press()
    {
        if (!checkText(3))
        {
            return;
            currentHighlighted = 3;

        }
        if (SuperMissionFlag)
        {
            SuperMissionNumber = SMOffset + 3;
            MissionFlag = true;
            SuperMissionFlag = false;
            LoadPage(1);
        }
        else if (MissionFlag)
        {
            MissionNumber = MOffset + 3;
            TaskFlag = true;
            MissionFlag = false;
            LoadPage(2);
        }
        else if (TaskFlag)
        {
            TaskNumber = TOffset + 3;
            SubTaskFlag = true;
            TaskFlag = false;
            LoadPage(3);
        }
        else if (force) //subtasks are currently being displayed
        {
            SubTaskNumber = STOffset;
            SubTaskFlag = false;
            SuperMissionFlag = true;
            LoadPage(0);
            force = false;

        }


    }

    public void button5Press()
    {
        if (!checkText(4))
        {
            return;
            currentHighlighted = 4;

        }
        if (SuperMissionFlag)
        {
            SuperMissionNumber = SMOffset + 4;
            MissionFlag = true;
            SuperMissionFlag = false;
            LoadPage(1);
        }
        else if (MissionFlag)
        {
            MissionNumber = MOffset + 4;
            TaskFlag = true;
            MissionFlag = false;
            LoadPage(2);
        }
        else if (TaskFlag)
        {
            TaskNumber = TOffset + 4;
            SubTaskFlag = true;
            TaskFlag = false;
            LoadPage(3);
        }
        else if (force) //subtasks are currently being displayed
        {
            SubTaskNumber = STOffset;
            SubTaskFlag = false;
            SuperMissionFlag = true;
            LoadPage(0);
            force = false;

        }


    }

    public void panel1Press()
    {
        //DebugManager.Instance.LogUnityConsole("Panel1Press");
        LoadPage(0);
        SuperMissionFlag = true;
        MissionFlag = false;
        TaskFlag = false;
        SubTaskFlag = false;
        currentPage = 0;
        
    }
    public void panel2Press()
    {
        //DebugManager.Instance.LogUnityConsole("Panel2Press");
        LoadPage(1);
        SuperMissionFlag = false;
        MissionFlag = true;
        TaskFlag = false;
        SubTaskFlag = false;
        currentPage = 1;
    }
    public void panel3Press()
    {
        LoadPage(2);
        SuperMissionFlag = false;
        MissionFlag = false;
        TaskFlag = true;
        SubTaskFlag = false;
        currentPage = 2;
    }
    public void panel4Press()
    {
        LoadPage(3);
        SuperMissionFlag = false;
        MissionFlag = false;
        TaskFlag = false;
        SubTaskFlag = true;
        currentPage = 3;
    }

    public bool checkText(int num)
    {

        if (num == 0)
        {
            
             if (button1.text == "")
                 return false;
           
        }
        else if (num == 1)
        {
            if (button2.text == "")
                return false;
        }
        else if (num == 2)
        {
            if (button3.text == "")
                return false;
        }
        else if (num == 3)
        {
            if (button4.text == "")
                return false;
        }
        else if (num == 4)
        {
            if (button5.text == "")
                return false;
        }

        return true;



    }

    public void onViewTogglePG0()
    {
        page0RenderState = !page0RenderState;

        if (page0RenderState)
        {
            InstantiateGestureMarkers();
        }
        else
        {
            FadeOutGestureMarkers();
        }

    }

    void InstantiateGestureMarkers()
    {
        if (!InputSystemStatus.Instance.GetShowGestureIndicators())
        {
            return;
        }

        DestroyGestureMarkers();
        obj = GetComponentsInChildren<Transform>(true);

        // get all buttons if any
        foreach (var ob in obj.Where(ob => (ob != transform)))
        {
            int gesture_number = -1;

            if (ob.CompareTag("gesture_1"))
            {
                gesture_number = 1;
            }
            else if (ob.CompareTag("gesture_2"))
            {
                gesture_number = 2;
            }
            else if (ob.CompareTag("gesture_3"))
            {
                gesture_number = 3;
            }
            else if (ob.CompareTag("gesture_4"))
            {
                gesture_number = 4;
            }
            else if (ob.CompareTag("gesture_sidescroll"))
            {
                gesture_number = 5;
            }

            if (gesture_number == 5)
            {
                //side scroll marker
                //Debug.Log("sidescroll");
                GameObject temp = Instantiate(DebugManager.Instance.sideScrollMarker, ob.transform);
                // set offset
                temp.transform.localScale = new Vector3(0.08333334f, 0.08333334f, 0.08333334f);
                temp.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                temp.transform.localPosition = new Vector3(-0.4575001f, -0.064f, -0.05166666f);
                //temp.transform.position 
                gestureMarkers.Add(temp);
            }
            else if (gesture_number != -1)
            {
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

    void FadeOutGestureMarkers()
    {

        //Debug.Log(gestureMarkers.Count);
        //Debug.Log("Fading out...");
        foreach (GameObject obj in gestureMarkers)
        {
            if (obj != null)
            {
                FadeInOut temp = obj.GetComponent<FadeInOut>();
                temp.FadeOut();
            }
        }
    }

    void DestroyGestureMarkers()
    {
        foreach (GameObject obj in gestureMarkers)
        {
            // can you destroy a null object? idk might need to null check
            Destroy(obj);
        }
        gestureMarkers.Clear();
    }

    private void pressCurrentHighlighted()
    {
        switch (currentHighlighted)
        {
            case 0:
                button1Press();
                break;
            case 1:
                button2Press();
                break;
            case 2:
                button3Press();
                break;
            case 3:
                button4Press();
                break;
            case 4:
                button5Press();
                break;
        }
    }

    private void upCurrentHighlight()
    {
        //DebugManager.Instance.LogUnityConsole("Current highlighted: " + currentHighlighted);
        if (currentHighlighted > 0 || (SuperMissionFlag && (SMOffset!=0)) || (MissionFlag && (MOffset != 0)) || (TaskFlag && (TOffset != 0)) || (SubTaskFlag && (STOffset != 0)))
        {
            if(currentHighlighted == 0)
                currentHighlighted = 4;
            else
             currentHighlighted--;

            if (currentHighlighted % 5 == 4)
            {
                backPage();

            }

        }
    }
    private void downCurrentHighlight()
    {
        if (SuperMissionFlag)
        {
            int SMissionCount = MissionContainerInstance.SuperMissions.Count;
            if (currentHighlighted < SMissionCount-SMOffset - 1)
            {
                currentHighlighted++;
                if (currentHighlighted % 5 == 0)
                {
                    currentHighlighted = 0;
                    nextPage();

                }

            }

        }
        else if (MissionFlag)
        {
            int MissionCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions.Count;
            if (currentHighlighted < (MissionCount-MOffset - 1))
            {
                currentHighlighted++;
                if (currentHighlighted % 5 == 0)
                {
                    currentHighlighted = 0;
                    nextPage();

                }

            }

        }
        else if (TaskFlag)
        {
            int taskCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks.Count;
            if (currentHighlighted < (taskCount-TOffset - 1))
            {
                currentHighlighted++;
                if (currentHighlighted % 5 == 0)
                {
                    currentHighlighted = 0;
                    nextPage();

                }

            }

        }
        else if (SubTaskFlag)
        {
            int StaskCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[TaskNumber].SubTasks.Count;
            if (currentHighlighted < (StaskCount-STOffset - 1))
            {
                currentHighlighted++;
                if (currentHighlighted % 5 == 0)
                {
                    currentHighlighted = 0;
                    nextPage();

                }

            }

        }

    }

}



[System.Serializable]
public class MissionContainer
{
    public int SpaceWalkID;
    public List<SuperMission> SuperMissions;

    public static MissionContainer CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MissionContainer>(jsonString);
    }

    public void printData()
    {

        //Debug.Log("Supermission Text 0: " + SuperMissions[0].SuperMissionText);
        //Debug.Log("Supermission Text 1: " + SuperMissions[1].SuperMissionText);

    }
    public int getSMCount()
    {
        return SuperMissions.Count;
    }
}

[System.Serializable]
public class SuperMission
{
    public int SuperMissionID;
    public string SuperMissionText;
    public string SuperMissionNumeral;
    public List<Mission> Missions;
}

[System.Serializable]
public class Mission
{
    public int MissionID;
    public string MissionText;
    public string MissionNumeral;
    public List<Task> Tasks;
}

[System.Serializable]
public class Task
{
    public int TaskID;
    public string TaskText;
    public string TaskNumeral;
    public string TaskProgram;
    public List<SubTask> SubTasks;
    //public int SubTaskID;
    //public string SubTaskText;
    //public string SubTaskProgram;
}

[System.Serializable]
public class SubTask
{
    public int SubTaskID;
    public string SubTaskText;
    public string SubTaskProgram;
}