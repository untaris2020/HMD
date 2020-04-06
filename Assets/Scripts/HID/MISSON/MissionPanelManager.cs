using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class MissionPanelManager : PanelBase
{
    [SerializeField] private string MissionServerURL;
    private string InputJSON;
    public MissionContainer MissionContainerInstance;
    public TextAsset textFile;
    public bool Local;
    private const float tickTime = 1f;

    //Arrow game object
    public GameObject upArrow;
    public GameObject downArrow;


    //Text fields that need to be updated
    public TextMeshProUGUI button1;
    public TextMeshProUGUI button2;
    public TextMeshProUGUI button3;
    public TextMeshProUGUI button4;
    public TextMeshProUGUI button5;
    public TextMeshProUGUI Title;


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

        

        //createColliders();
        //Set default selections to 0 on start
        SuperMissionNumber = 0;
        MissionNumber = 0;
        TaskNumber = 0;
        SubTaskNumber = 0;

        string InputJSON = textFile.text;

        MissionContainerInstance = MissionContainer.CreateFromJSON(InputJSON);


        //SMissionCount = MissionContainerInstance.SuperMissions.Count;

        /*
        for (int i =0; i < SMissionCount; i++)
        {
            MissionCount.Add(MissionContainerInstance.SuperMissions[i].Missions.Count);
            
        }

        for (int i = 0; i < SMissionCount; i++)
        {
            List<int> tempList = new List<int>();
            for (int j=0; j < MissionCount[i]; j++)
            {

                tempList.Add(MissionContainerInstance.SuperMissions[i].Missions[j].Tasks.Count);

            }
            TaskCount.Add(tempList);
        }

        for (int i = 0; i < SMissionCount; i++)
        {
            List<int> tempList = new List<int>();
            for (int j = 0; j < MissionCount[i]; j++)
            {

                tempList.Add(MissionContainerInstance.SuperMissions[i].Missions[j].Tasks.Count);

            }
            TaskCount.Add(tempList);
        }
        */

        //upArrow.SetActive(false);
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
            Debug.Log("Subtasksrun?");
            displaySubTasks();
        }
        //MissionContainerInstance.printData();
        
    }
    public void displaySuperMissions()
    {
        int SMissionCount = MissionContainerInstance.SuperMissions.Count;
        Title.SetText("Super Missions");
        Debug.Log("SMission Count: " + SMissionCount);
        if(SMissionCount - SMOffset > 0)
            button1.SetText(MissionContainerInstance.SuperMissions[0 + SMOffset].SuperMissionText);
        if (SMissionCount - SMOffset > 1)
            button2.SetText(MissionContainerInstance.SuperMissions[1 + SMOffset].SuperMissionText);
        if (SMissionCount - SMOffset > 2)
            button3.SetText(MissionContainerInstance.SuperMissions[2 + SMOffset].SuperMissionText);
        if (SMissionCount - SMOffset > 3)
            button4.SetText(MissionContainerInstance.SuperMissions[3 + SMOffset].SuperMissionText);
        if (SMissionCount - SMOffset > 4)
            button5.SetText(MissionContainerInstance.SuperMissions[4 + SMOffset].SuperMissionText);
    }
    public void displayMissions()
    {
        int MissionCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions.Count;
        Title.SetText("Missions");
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
    }
    public void displayTasks()
    {
        int taskCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks.Count;
        Title.SetText("Tasks");
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
    }
    public void displaySubTasks()
    {
        int StaskCount = MissionContainerInstance.SuperMissions[SuperMissionNumber].Missions[MissionNumber].Tasks[TaskNumber].SubTasks.Count;
        Title.SetText("Sub-Tasks");
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
    }

    public void createColliders()
    {

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();


        Button0Delegate tmpDelegate0 = new Button0Delegate(nextPage);
        ht.registerCollider(downArrow.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate0);

        Button0Delegate tmpDelegate1 = new Button0Delegate(backPage);
        ht.registerCollider(upArrow.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction(this.GetType().Name + "_tab0", tmpDelegate0);

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