using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ModelLoader : MonoBehaviour
{
    // buttons
    public TelemPanelManager telem_panel_manager;
    public GameObject clearModelsButton, misc1Button, misc2Button, upButton, downButton, upArrow, _camera;
    public GameObject[] modelButtons;

    // text
    public TextMeshProUGUI[] modelTexts;
    public TextMeshProUGUI heading_text;
    public TextMeshProUGUI index_text;  // left
    public TextMeshProUGUI misc_text;   // right - unused
    public TextMeshProUGUI instructions_text;

    // array/list of models
    private int NUMOFMODELS;
    private int NUMOFPAGES;
    private int page_index = 0;
    private int load_model;
    private bool ready_to_load = false;
    public GameObject[] models;
    public GameObject glove_model;
    private List<GameObject> loaded_models;


    private delegate void functionDelegate();


    // Start is called before the first frame update
    void Start()
    {
        loaded_models = new List<GameObject>();
        NUMOFMODELS = models.Length;
        DebugManager.Instance.LogBoth(this.GetType().Name, NUMOFMODELS + " models loaded.");
        NUMOFPAGES = (int)System.Math.Ceiling((double)NUMOFMODELS / 5);
        DebugManager.Instance.LogUnityConsole("PAGES", NUMOFPAGES.ToString());

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        // clear models button
        functionDelegate tmpDelegate0 = new functionDelegate(ClearModelsButton);
        ht.registerCollider(clearModelsButton.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("clearModels", tmpDelegate0);

        // set position button
        tmpDelegate0 = new functionDelegate(Set3DModelPosition);
        ht.registerCollider(misc1Button.GetComponent<Collider>().name, tmpDelegate0);
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

        UpdateUI();
        misc_text.SetText("");
        glove_model.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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

        glove_model.SetActive(true);
        glove_model.transform.position = spawn_pos;

        // waiting for user to click the button...
        ready_to_load = true;
    }

    private void ClearModelsButton()
    {
        // not sure if this will work
        foreach (GameObject obj in loaded_models)
        {
            Destroy(obj);
        }

        loaded_models.Clear();
    }

    private void Set3DModelPosition()
    {
        if (!ready_to_load)
        {
            return;
        }

        // Load and scale selected model
        if (load_model == -1)
        {
            DebugManager.Instance.LogBoth(this.GetType().Name, "ERROR: load_model number not set");
        }
        int model_index = load_model + (page_index * 5);

        loaded_models.Add( (GameObject)Instantiate(models[model_index], new Vector3(0, 0, -15), Quaternion.identity));
        DebugManager.Instance.LogBoth("INFO", "Loading 3D Model " + models[model_index].name);

        load_model = -1;
        glove_model.SetActive(false);
        instructions_text.SetText("");
        ready_to_load = false;
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
}
