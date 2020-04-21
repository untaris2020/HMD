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
    private int currentHighlighted;
    public Collider navPG3_col; 

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
        currentHighlighted = 0;
        aud = GetComponent<AudioSource>();

        clips = new List<recording>();

        currentPage = 0; 

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        ButtonDelegate tmpDelegate = new ButtonDelegate(upArrowHit);
        ht.registerCollider(upArrowCollider.GetComponent<Collider>().name, tmpDelegate);
        tmpDelegate = new ButtonDelegate(downArrowHit);
        ht.registerCollider(downArrowCollider.GetComponent<Collider>().name, tmpDelegate);

        //Register Force functions
        tmpDelegate  = new ButtonDelegate(pressCurrentHighlighted);
        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        ht.registerForceCollider(navPG3_col.name + "1", navPG3_col.name, tmpDelegate, input);

        tmpDelegate = new ButtonDelegate(upCurrentHighlight);
        input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        ht.registerForceCollider(navPG3_col.name + "2", navPG3_col.name, tmpDelegate, input);

        tmpDelegate = new ButtonDelegate(downCurrentHighlight);
        input = new forceSensorManager.fingerInput(0, 0, 0, 1, 0);
        ht.registerForceCollider(navPG3_col.name + "3", navPG3_col.name, tmpDelegate, input);

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
            if(j == currentHighlighted%5)
            {
                tempBox.GetComponentInChildren<MeshRenderer>().material = StyleSheet.Instance.Highlighted;
            }
            else
            {
                tempBox.GetComponentInChildren<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;
            }

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
        currentHighlighted = 0;
        UpdateList(); 
        playClip(0);
    }
    public void Box2Hit()
    {
        currentHighlighted = 1; 
        UpdateList(); 
        playClip(1);
    }
    public void Box3Hit()
    {
        currentHighlighted = 2;
        UpdateList(); 
        playClip(2);
    }
    public void Box4Hit()
    {
        currentHighlighted = 3;
        UpdateList(); 
        playClip(3);
    }
    public void Box5Hit()
    {
        currentHighlighted = 4; 
        UpdateList(); 
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

    private void pressCurrentHighlighted()
    {
       playClip(currentHighlighted%5);
    }

    private void upCurrentHighlight()
    {
        if(currentHighlighted > 0)
        {
            currentHighlighted--; 
            if(currentHighlighted % 5 == 4)
            {
                upArrowHit();
                
            }
            else
            {
                UpdateList(); 
            }
        }
    }
    private void downCurrentHighlight()
    {
        if(currentHighlighted < (clips.Count - 1))
        {
            currentHighlighted++; 
            if(currentHighlighted % 5 == 0)
            {
                downArrowHit();
            }
            else
            {
                UpdateList(); 
            }
        }
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
