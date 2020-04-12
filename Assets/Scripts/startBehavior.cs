using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class startBehavior : MonoBehaviour
{
    public GameObject hid;
    public GameObject button;
    public GameObject text;
    public IMUHandler IMU_CHEST;
    private bool hidState; 
    private delegate void ButtonDelegate();
    public bool useStartRoutine; 
    // Start is called before the first frame update
    void Start()
    {
        if(useStartRoutine)
        {
            hidState = false;
            toggleHID(hidState);
            
            button.SetActive(true);
            text.SetActive(true);
            HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();
            ButtonDelegate tmpDelegate = new ButtonDelegate(onClick);
            ht.registerCollider(this.GetComponentInChildren<Collider>().name, tmpDelegate);
            
            this.GetComponentInChildren<TextMeshProUGUI>().SetText("CLICK TO START SCENE");
        }
        else
        {
            hidState = true;
            toggleHID(hidState);
            button.SetActive(false);
            text.SetActive(false);
        }
        
    }

    public void onClick()
    {
        if(IMU_CHEST.getConnected())
        {
            hidState = true;
            toggleHID(hidState);
            IMU_CHEST.startStream();
            button.SetActive(false);
            text.SetActive(false);
        }
        else
        {
            this.GetComponentInChildren<TextMeshProUGUI>().SetText("Please Connect Chest IMU");
        }
        

    }

    public void toggleHID(bool mode)
    {
        if(mode == true)
        {
            hid.transform.localScale = new Vector3(1, 1, 1);
        }
        if(mode == false)
        {
            hid.transform.localScale = new Vector3(0, 0, 0);
        }
    }
    
    public void DisableHID()
    {
        hidState = false;
        toggleHID(hidState);
        button.SetActive(true);
        text.SetActive(true);
    }
}
