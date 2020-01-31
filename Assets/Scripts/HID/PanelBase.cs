﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    public StyleSheet style;

    public GameObject[] panels;
    public GameObject[] pages;

    public int pageIndex;

    public delegate void Button0Delegate();
    public delegate void Button1Delegate();
    public delegate void Button2Delegate();
    public delegate void Button3Delegate();

    void Start()
    {
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        //MyDelegate RearviewON = new MyDelegate(PressRearviewON);
        //ht.registerCollider(rearviewONButton.GetComponent<Collider>().name,RearviewON);

        // Resiter Colliders
        DebugManager.Instance.LogUnityConsole("T0", ht.name);
        DebugManager.Instance.LogUnityConsole("T1" , panels[0].GetComponent<Collider>().name);

        Button0Delegate tmpDelegate0 = new Button0Delegate(Button0Press);
        ht.registerCollider(panels[0].GetComponent<Collider>().name, tmpDelegate0);

        Button1Delegate tmpDelegate1 = new Button1Delegate(Button1Press);
        ht.registerCollider(panels[1].GetComponent<Collider>().name, tmpDelegate1);

        Button2Delegate tmpDelegate2 = new Button2Delegate(Button2Press);
        ht.registerCollider(panels[2].GetComponent<Collider>().name, tmpDelegate2);

        Button3Delegate tmpDelegate3 = new Button3Delegate(Button3Press);
        ht.registerCollider(panels[3].GetComponent<Collider>().name, tmpDelegate3);

        pageIndex = 0;

        Button0Press();
    }

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

    public void LoadPage(int page)
    {
        pageIndex = page;

        DebugManager.Instance.LogUnityConsole(this.GetType().Name, "Page " + page + " Loaded.");

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