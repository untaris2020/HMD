using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MissionWebRequest : MonoBehaviour
{

    [SerializeField] private string MissionServerURL;
    private string InputJSON;
    public MissionContainer MissionContainerInstance;
    public TextAsset textFile;
    public bool Local;
    private const float tickTime = 1f;


    // Use this for initialization
    void Start()
    {
        MissionServerURL = "http://davidw.engineer/ARIS/onsiteMissionJSON.txt";
        Debug.Log("Mission URL: " + MissionServerURL);
        if (Local)
        {
            string InputJSON = textFile.text;

            MissionContainerInstance = MissionContainer.CreateFromJSON(InputJSON);

            Debug.Log("INFO: Mission Loaded.");

        }
        else
        {
            StartCoroutine(GetRequest(MissionServerURL));
        }

    }
    IEnumerator GetRequest(string uri)
    {


        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                // Parse HTML webpage

                InputJSON = webRequest.downloadHandler.text;

                MissionContainerInstance = MissionContainer.CreateFromJSON(InputJSON);

                Debug.Log("INFO: Mission Loaded.");

            }
        } // Debug.Log(InputJSON);

        // loop
        yield return new WaitForSeconds(tickTime);
        StartCoroutine(GetRequest(MissionServerURL));

    }

    //  Debug.Log(MissionContainerInstance.SpaceWalkID);
    //Debug.Log(MissionContainerInstance.SuperMissions[2].SuperMissionText);
    // Debug.Log(MissionContainerInstance.SuperMissions[2].SuperMissionNumeral);
    // Debug.Log(MissionContainerInstance.SuperMissions[2].Missions[1].Tasks[5].SubTasks[6].SubTaskText);
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
    public int SubTaskID;
    public string SubTaskText;
    public string SubTaskProgram;
}

[System.Serializable]
public class SubTask
{
    public int SubTaskID;
    public string SubTaskText;
    public string SubTaskProgram;
}