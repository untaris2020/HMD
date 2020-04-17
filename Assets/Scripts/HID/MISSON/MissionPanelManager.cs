using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class MissionPanelManager : MonoBehaviour 
{
    [SerializeField] private string MissionServerURL;
    private string InputJSON;
    public MissionContainer MissionContainerInstance;
    public TextAsset textFile;
    public StyleSheet style;

    public delegate void Button0Delegate();

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

    

    // Use this for initialization
    void Start()
    {

        

        createColliders();
        //Set default selections to 0 on start
        SuperMissionNumber = 0;
        MissionNumber = 0;
        TaskNumber = 0;
        SubTaskNumber = 0;



        panel1Press();

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
        //MissionContainerInstance.printData();
        
    }

    public void displaySuperMissions()
    {
        int SMissionCount = MissionContainerInstance.SuperMissions.Count;
        
        Debug.Log("SMission Count: " + SMissionCount);

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

        if(SMissionCount - SMOffset == 1)
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
        if(SMissionCount > 5)
            pages = SMissionCount % 5 + 1;
   
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
            pages = MissionCount % 5 + 1;

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

        Index.SetText(SuperMissionNumber+1 + "." + (MissionNumber+1) + ".");

        int pages = 1;
        if (taskCount > 5)
            pages = taskCount % 5 + 1;

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
           

        
        Debug.Log("SubTask Count: " + StaskCount);
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
            pages = StaskCount % 5 + 1;

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
        

    }

    public void LoadPage(int page)
    {
        // function overload
        DebugManager.Instance.LogUnityConsole("Correct Load page");

        DebugManager.Instance.LogUnityConsole(this.GetType().Name, "Page " + page + " Loaded.");
        
        //if(Verbose)
        //{
            
        //}

        // Set all buttons material to inactive
        foreach (GameObject obj in panels)
        {
            obj.GetComponent<MeshRenderer>().material = style.ButtonInactiveMat;
        }

        // set current page as the active material
        panels[page].GetComponent<MeshRenderer>().material = style.ButtonActiveMat;


        

        // add any custom logic here if needed

        //
    }

    public void button1Press()
    {
        

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
        else //subtasks are currently being displayed
        {
            if (displayPicture)
            {
                displayPicture = !displayPicture;
            }
            else
            {
                SubTaskNumber = STOffset;
                displayPicture = true;
            }
            

        }
        

    }

    public void button2Press()
    {
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
        else //subtasks are currently being displayed
        {
            if (displayPicture)
            {
                displayPicture = !displayPicture;
            }
            else
            {
                SubTaskNumber = STOffset + 1;
                displayPicture = true;
            }


        }


    }

    public void button3Press()
    {
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
        else //subtasks are currently being displayed
        {
            if (displayPicture)
            {
                displayPicture = !displayPicture;
            }
            else
            {
                SubTaskNumber = STOffset + 2;
                displayPicture = true;
            }


        }


    }

    public void button4Press()
    {
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
        else //subtasks are currently being displayed
        {
            if (displayPicture)
            {
                displayPicture = !displayPicture;
            }
            else
            {
                SubTaskNumber = STOffset + 3;
                displayPicture = true;
            }


        }


    }

    public void button5Press()
    {
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
        else //subtasks are currently being displayed
        {
            if (displayPicture)
            {
                displayPicture = !displayPicture;
            }
            else
            {
                SubTaskNumber = STOffset + 4;
                displayPicture = true;
            }


        }


    }

    public void panel1Press()
    {
        LoadPage(0);
        SuperMissionFlag = true;
        MissionFlag = false;
        TaskFlag = false;
        SubTaskFlag = false;
    }
    public void panel2Press()
    {
        LoadPage(1);
        SuperMissionFlag = false;
        MissionFlag = true;
        TaskFlag = false;
        SubTaskFlag = false;
    }
    public void panel3Press()
    {
        LoadPage(2);
        SuperMissionFlag = false;
        MissionFlag = false;
        TaskFlag = true;
        SubTaskFlag = false;
    }
    public void panel4Press()
    {
        LoadPage(3);
        SuperMissionFlag = false;
        MissionFlag = false;
        TaskFlag = false;
        SubTaskFlag = true;
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

        Debug.Log("Supermission Text 0: " + SuperMissions[0].SuperMissionText);
        Debug.Log("Supermission Text 1: " + SuperMissions[1].SuperMissionText);

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