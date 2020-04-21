﻿using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ModelLoader : MonoBehaviour
{
    // buttons
    public TelemPanelManager telem_panel_manager;
    public GameObject clearModelsBut, misc1Button, misc2Button, upButton, downButton, upArrow, _camera;
    public GameObject[] modelButtons;
    public IMUHandler IMU_GLOVE; 

    // text
    public TextMeshProUGUI[] modelTexts;
    public TextMeshProUGUI heading_text;
    public TextMeshProUGUI index_text;  // left
    public TextMeshProUGUI misc_text;   // right - unused
    public TextMeshProUGUI instructions_text;

    public GameObject telem_panel2Col;
    public GameObject telem_panel3Col; 
    // array/list of models
    private int NUMOFMODELS;
    private int NUMOFPAGES;
    private int page_index = 0;
    private int load_model;
    private bool ready_to_load = false;
    public GameObject[] models;
    public GameObject glove_model;
    private List<GameObject> loaded_models;
    private bool newIMUpacket;
    private bool firstPacket; 
    private Quaternion rot;
    private Quaternion defaultRot; 
    private Vector3 pos;

    private int currentHighlighted; 


    private delegate void functionDelegate();


    // Start is called before the first frame update
    void Start()
    {
        currentHighlighted = 0; 

        newIMUpacket = false;
        firstPacket = false; 
        loaded_models = new List<GameObject>();
        NUMOFMODELS = models.Length;
        DebugManager.Instance.LogBoth(this.GetType().Name, NUMOFMODELS + " models loaded.");
        NUMOFPAGES = (int)System.Math.Ceiling((double)NUMOFMODELS / 5);
        DebugManager.Instance.LogUnityConsole("PAGES", NUMOFPAGES.ToString());

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        // clear models button
        functionDelegate tmpDelegate0 = new functionDelegate(ClearModelsButton);
        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        ht.registerCollider(clearModelsBut.GetComponent<Collider>().name, telem_panel3Col.name, tmpDelegate0, input);
        functionDebug.Instance.registerFunction("clearModels", tmpDelegate0);

        // set position button
        tmpDelegate0 = new functionDelegate(Set3DModelPosition);
        input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        ht.registerCollider(misc1Button.GetComponent<Collider>().name, telem_panel3Col.name, tmpDelegate0, input);
        functionDebug.Instance.registerFunction("set3DModelPosition", tmpDelegate0);
        
        // up arrow
        tmpDelegate0 = new functionDelegate(UpArrowButton);
        ht.registerCollider(upButton.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("upArrowButtonModels", tmpDelegate0);

        // down arrow
        tmpDelegate0 = new functionDelegate(DownArrowButton);
        ht.registerCollider(downButton.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("downArrowButtonModels", tmpDelegate0);

        // model buttons
        tmpDelegate0 = new functionDelegate(LoadModel0);
        ht.registerCollider(modelButtons[0].GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("loadModel0", tmpDelegate0);

        tmpDelegate0 = new functionDelegate(LoadModel1);
        ht.registerCollider(modelButtons[1].GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("loadModel1", tmpDelegate0);

        tmpDelegate0 = new functionDelegate(LoadModel2);
        ht.registerCollider(modelButtons[2].GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("loadModel2", tmpDelegate0);

        tmpDelegate0 = new functionDelegate(LoadModel3);
        ht.registerCollider(modelButtons[3].GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("loadModel4", tmpDelegate0);

        tmpDelegate0 = new functionDelegate(LoadModel4);
        ht.registerCollider(modelButtons[4].GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("loadModel4", tmpDelegate0);

        //Register Force functions
        tmpDelegate0 = new functionDelegate(pressCurrentHighlighted);
        input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        ht.registerForceCollider(telem_panel2Col.name + "1", telem_panel2Col.name, tmpDelegate0, input);

        tmpDelegate0 = new functionDelegate(upCurrentHighlight);
        input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        ht.registerForceCollider(telem_panel2Col.name + "2", telem_panel2Col.name, tmpDelegate0, input);

        tmpDelegate0 = new functionDelegate(downCurrentHighlight);
        input = new forceSensorManager.fingerInput(0, 0, 0, 1, 0);
        ht.registerForceCollider(telem_panel2Col.name + "3", telem_panel2Col.name, tmpDelegate0, input);

        for (int i=0; i<models.Length; i++) {
            if (models[i] == null) {
                GameObject temp = new GameObject("NULL");
                models[i] = temp;
            }
        }

        UpdateUI();
        misc_text.SetText("");
        glove_model.GetComponent<Renderer>().enabled = false;

        
    }

    // Update is called once per frame
    void Update()
    {
        if(newIMUpacket && loaded_models.Count > 0)
        {
            if(firstPacket)
            {
                defaultRot = rot; 
            }
            newIMUpacket = false; 
            float speed = Time.deltaTime * 12.0f;
            glove_model.transform.position = Vector3.Slerp(glove_model.transform.position, pos, speed);
            rot = rot *  Quaternion.Inverse(defaultRot);
            glove_model.transform.rotation = Quaternion.Slerp(glove_model.transform.rotation, rot, speed); 
        }
    }

    void UpdateUI()
    { 
        index_text.SetText((page_index + 1).ToString() + " / " + NUMOFPAGES.ToString());
        if (page_index == 0)
        {
            heading_text.SetText("3D MODEL SELECT");
            upArrow.SetActive(false);
        } else
        {
            heading_text.SetText("");
            upArrow.SetActive(true);
        }

        for (int i=0; i<5; i++)
        {
            if(i == (currentHighlighted %5))
            {
                modelButtons[i].GetComponent<MeshRenderer>().material = StyleSheet.Instance.Highlighted;
            }
            else
            {
                modelButtons[i].GetComponent<MeshRenderer>().material = StyleSheet.Instance.NonHighlighted;   
            }

            int tmp_index = i + (page_index * 5);
            // TODO need to check if in bounds of array
            if (tmp_index >=0 && tmp_index < models.Length)
            {
                modelTexts[i].SetText(models[tmp_index].name);
            } else
            {
                modelTexts[i].SetText("");
            }
        }
    }

    private void NextPage()
    {
        if (page_index <= (NUMOFPAGES-2))
        {

            page_index++;
            UpdateUI();
        } else
        {
            DebugManager.Instance.LogUnityConsole("ERROR max pages");
        }
    }

    private void PrevPage()
    {
        if (page_index > 0)
        {

            page_index--;
            UpdateUI();
        }
    }

    private void LoadModel(int pos)
    {
        load_model = pos;

        // load Telem_pg_3
        telem_panel_manager.LoadPage(3);
        instructions_text.SetText("Align your hand with the glove and click the SET POSITION button");
        

        // Load Hand Model and have user align glove
        Vector3 spawn_pos = _camera.transform.position;
        spawn_pos.z += 5;
        spawn_pos.y -= 5;

        glove_model.GetComponent<Renderer>().enabled = true;
        glove_model.transform.position = spawn_pos;

        // waiting for user to click the button...
        ready_to_load = true;
    }

    public void ClearModelsButton()
    {
        // not sure if this will work
        foreach (GameObject obj in loaded_models)
        {
            Destroy(obj);
        }

        loaded_models.Clear();
    }

    public void updateModelwithIMU(float w, float x, float y, float z, float xAccel, float yAccel, float zAccel)
    { 
        pos = new Vector3(xAccel, yAccel, zAccel);
        rot = new Quaternion(-x,-z,-y,w);
        newIMUpacket = true;
    }

    private void Set3DModelPosition()
    {
        if (!ready_to_load)
        {
            return;
        }

        if(!IMU_GLOVE.getConnected())
        {
            DebugManager.Instance.LogBoth("MODEL LOADER: ERROR CHEST IMU NOT CONNECTED");
        }

        // Load and scale selected model
        if (load_model == -1)
        {
            DebugManager.Instance.LogBoth(this.GetType().Name, "ERROR: load_model number not set");
        }
        int model_index = load_model + (page_index * 5);

        // TODO
        if (DebugManager.Instance.GetSimulatorMode()) {

        } else {

        }

        loaded_models.Add( (GameObject)Instantiate(models[model_index], new Vector3(0, 0, 0), Quaternion.identity, glove_model.transform));
        DebugManager.Instance.LogBoth("INFO", "Loading 3D Model " + models[model_index].name);

        load_model = -1;
        glove_model.GetComponent<Renderer>().enabled = false;
        instructions_text.SetText("");
        ready_to_load = false;

        //Start IMU stream 
        IMU_GLOVE.startStream(); 
    }

    private void UpArrowButton()
    {
        PrevPage();
    }

    private void DownArrowButton()
    {
        NextPage();
    }

    private void LoadModel0()
    {
        LoadModel(0);
    }

    private void LoadModel1()
    {
        LoadModel(1);
    }

    private void LoadModel2()
    {
        LoadModel(2);
    }

    private void LoadModel3()
    {
        LoadModel(3);
    }

    private void LoadModel4()
    {
        LoadModel(4);
    }

    private void pressCurrentHighlighted()
    {
        switch(currentHighlighted)
        {
            case 0:
                LoadModel0(); 
                break;
            case 1:
                LoadModel1(); 
                break;
            case 2:
                LoadModel2(); 
                break;
            case 3:
                LoadModel3(); 
                break;
            case 4:
                LoadModel4(); 
                break;
        }
    }

    private void upCurrentHighlight()
    {
        if(currentHighlighted > 0)
        {
            currentHighlighted--; 
            if(currentHighlighted % 5 == 4)
            {
                PrevPage();
                
            }
            UpdateUI(); 
        }
    }
    private void downCurrentHighlight()
    {
        if(currentHighlighted < (models.Length -1))
        {
            currentHighlighted++; 
            if(currentHighlighted % 5 == 0)
            {
                NextPage();
                
            }
            UpdateUI();
        }
    }

}
