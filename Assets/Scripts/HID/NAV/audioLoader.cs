using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System;

public class audioLoader : MonoBehaviour
{ 
    public GameObject upArrowCollider;
    public GameObject actualUpArrowRender;
    public GameObject downArrowCollider;
    public GameObject actualDownArrowRender;
    public TextMeshProUGUI pageCount;
    public TextMeshProUGUI Header;
    public TextMeshProUGUI upperLeft;
    public List<GameObject> box_list;
    private List<recording> clips; 
    private delegate void ButtonDelegate();
    private int currentPage;
    private AudioSource aud;

    private struct recording
    {
        public AudioClip clip;
        public string Name; 
        public DateTime Time;
        public float length; 
    }

    // Start is called before the first frame update
    void Start()
    {
        aud = GetComponent<AudioSource>();

        clips = new List<recording>();

        currentPage = 0; 

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        ButtonDelegate tmpDelegate = new ButtonDelegate(upArrowHit);
        ht.registerCollider(upArrowCollider.GetComponentInChildren<MeshCollider>().name, tmpDelegate);
        tmpDelegate = new ButtonDelegate(downArrowHit);
        ht.registerCollider(downArrowCollider.GetComponentInChildren<MeshCollider>().name, tmpDelegate);

        int i = 0;
        foreach(GameObject box in box_list)
        {
            if(i == 0)
                tmpDelegate = new ButtonDelegate(Box1Hit);
            else if(i == 1)
                tmpDelegate = new ButtonDelegate(Box2Hit);
            else if(i == 2)
                tmpDelegate = new ButtonDelegate(Box3Hit);
            else if(i == 3)
                tmpDelegate = new ButtonDelegate(Box4Hit);
            else if(i == 4)
                tmpDelegate = new ButtonDelegate(Box5Hit);

            ht.registerCollider(box.GetComponentInChildren<Collider>().name, tmpDelegate);
            i++;

            box.GetComponentInChildren<TextMeshProUGUI>().SetText("");
            pageCount.SetText("");
            
            upperLeft.SetText("");
        }

        UpdateList();
    }

    private void playClip(int Box)
    {
        int idx = currentPage * 5 + Box;
        if(clips.Count > idx)
        {
            aud.clip = clips[idx].clip;
            aud.Play();
        }
        else
        {
            Debug.Log("ERROR NO CLIP TO LOAD");
        }
    }

    public void UpdateList()
    {
        int maxPg = (int)Mathf.Ceil(clips.Count / 5.0f);
        if(maxPg == 0)
        {
            maxPg = 1;
        }
        pageCount.SetText(currentPage + 1 + "/" + maxPg);

        //Load in the new audio Files and render screen appropriately 
        //Update the down arrow visible if needed
        if((clips.Count - (currentPage*5)) < 6) 
        {
            actualDownArrowRender.SetActive(false);
        }
        else
        {
            actualDownArrowRender.SetActive(true);
        }
        //Update the up arrow if needed 
        if(currentPage != 0)
        {
            actualUpArrowRender.SetActive(true);
        }
        else
        {
            Header.SetText("Audio Clips");
            actualUpArrowRender.SetActive(false);
        }
        

        //Load In Appropriate Text Here
        int j = 0;

        //int maxVal = clips.Count - (currentPage * 5);

        for(int i =(currentPage*5); i < (currentPage*5 + 5); i++)
        {
            GameObject tempBox = box_list[j];
            if(i < clips.Count)
            {
                 AudioClip tempClip = clips[i].clip;

                
                string minutes = Mathf.Floor(clips[i].length/ 60).ToString("00");
                string seconds = Mathf.Floor(clips[i].length % 60).ToString("00");

                tempBox.GetComponentInChildren<TextMeshProUGUI>().SetText(clips[i].Name + ": LENGTH - " + minutes +":" + seconds + " AT TIME - " + clips[i].Time.ToString());
            }
            else
            {
                tempBox.GetComponentInChildren<TextMeshProUGUI>().SetText("");
            }
            j++;
        }
    }

    public void Box1Hit()
    {
        playClip(0);
    }
    public void Box2Hit()
    {
        playClip(1);
    }
    public void Box3Hit()
    {
        playClip(2);
    }
    public void Box4Hit()
    {
        playClip(3);
    }
    public void Box5Hit()
    {
        playClip(4);
    }
    public void upArrowHit()
    {
        if(currentPage > 0)
        {
            currentPage--;
        }
        else
        {
            Debug.Log("Pages Min");
        }
        UpdateList();
    }
    public void downArrowHit()
    {
        int pageMax = (int)Mathf.Ceil(clips.Count / 5.0f); // 2
        if (currentPage < pageMax-1)
        {
            currentPage++;
        }
        else
        {
            Debug.Log("Pages Maxed");
        }
        UpdateList();
    }

    public void LoadNewAudio(AudioClip clip, DateTime time, string Name)
    {
        recording tempRec = new recording();

        tempRec.clip = clip;
        tempRec.length = clip.length;
        tempRec.Name = Name;
        tempRec.Time = time; 


        clips.Add(tempRec);
        UpdateList();
    }
}
