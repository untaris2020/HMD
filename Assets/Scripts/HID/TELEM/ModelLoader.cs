using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.XR.MagicLeap;
using System.Linq;

public class ModelLoader : MonoBehaviour
{
    // buttons
    public MOV_MODE mode;
    public TelemPanelManager telem_panel_manager;
    public GameObject clearModelsBut, misc1Button, misc2Button, upButton, downButton, upArrow, _camera;
    public GameObject[] modelButtons;
    public IMUHandler IMU_GLOVE;
    public IMUHandler IMU_CHEST;
    public ToggleHandler TOGGLE_CHEST;
    public ToggleHandler TOGGLE_GLOVE; 
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
    public List<GameObject> models;
    public GameObject glove_model;
    public GameObject ModelList; 
    private List<GameObject> loaded_models;
    private bool newIMUpacket;
    private bool firstPacket;
    private bool ready_to_load = false;
    private Quaternion rot;
    private Quaternion defaultRot; 
    private Vector3 pos;

    private int currentHighlighted; 
    public enum MOV_MODE
    {
        IMU = 0,
        BLEND = 1,
        controller = 2, 
    }

    
    private delegate void functionDelegate();

    [SerializeField]
    private MLControllerConnectionHandlerBehavior _controllerConnectionHandler;

    private MLInput.Controller _control;


    // Start is called before the first frame update
    void Start()
    {
        loaded_models = new List<GameObject>();
        
        currentHighlighted = 0; 

        newIMUpacket = false;
        firstPacket = false;
        //Future expansior below line should be removed to allow for multiple models
        
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        // clear models button
        functionDelegate tmpDelegate0 = new functionDelegate(ClearModelsButton);
        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        ht.registerCollider(clearModelsBut.GetComponent<Collider>().name, telem_panel3Col.name, tmpDelegate0, input);
        functionDebug.Instance.registerFunction("clearModels", tmpDelegate0);

        // set position button
        tmpDelegate0 = new functionDelegate(Set3DModelPosition);
        input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
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

        for (int i=0; i<models.Count; i++) {
            if (models[i] == null) {
                //models.RemoveAt(i);
                GameObject temp = new GameObject("RemoveElement");
                models[i] = temp;
            }
        }

        models.RemoveAll(item => item.name == "RemoveElement");

        NUMOFMODELS = models.Count;
        DebugManager.Instance.LogBoth(this.GetType().Name, NUMOFMODELS + " models loaded.");
        NUMOFPAGES = (int)System.Math.Ceiling((double)NUMOFMODELS / 5);
        DebugManager.Instance.LogUnityConsole("PAGES", NUMOFPAGES.ToString());

        UpdateUI();
        misc_text.SetText("");
        glove_model.GetComponent<Renderer>().enabled = false;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (loaded_models != null)
        {
            if (mode == MOV_MODE.IMU)
            {
                if (newIMUpacket && loaded_models.Count > 0)
                {
                    if (firstPacket)
                    {
                        defaultRot = rot;
                    }
                    newIMUpacket = false;
                    float speed = Time.deltaTime * 12.0f;
                    ModelList.transform.position = Vector3.Slerp(ModelList.transform.position, pos, speed);
                    rot = rot * Quaternion.Inverse(defaultRot);
                    ModelList.transform.rotation = Quaternion.Slerp(ModelList.transform.rotation, rot, speed);
                }
            }
            else if (mode == MOV_MODE.BLEND)
            {
                   


                if (newIMUpacket && loaded_models.Count > 0)
                {
                    if (firstPacket)
                    {
                        defaultRot = rot;
                    }
                    newIMUpacket = false;
                    float speed = Time.deltaTime * 12.0f;
                    ModelList.transform.position = Vector3.Slerp(ModelList.transform.position, _control.Position, speed);
                    rot = rot * Quaternion.Inverse(defaultRot);
                    ModelList.transform.rotation = Quaternion.Slerp(ModelList.transform.rotation, rot, speed);
                }
            }
            else if (mode == MOV_MODE.controller)
            {
                if (loaded_models.Count > 0)
                {
                    float speed = Time.deltaTime * 12.0f;
                    ModelList.transform.position = Vector3.Slerp(ModelList.transform.position, _control.Position, speed);
                    ModelList.transform.rotation = Quaternion.Slerp(ModelList.transform.rotation, _control.Orientation, speed);
                }
            }
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
            if (tmp_index >=0 && tmp_index < models.Count)
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
        if (models.Count <= page_index * 5 + pos) {
            return;
        }

        ClearModelsButton(); 

        if(!(IMU_GLOVE.getConnected() && TOGGLE_CHEST.getConnected() && TOGGLE_GLOVE.getConnected() && IMU_CHEST.getConnected()))
        {
            DebugManager.Instance.LogBoth("UNABLE TO LOAD MODEL: CHECK CONNECTION STATUS");
            return; 
        }

        if(mode != MOV_MODE.IMU)
        {
            if (!_controllerConnectionHandler.IsControllerValid()) //this is error condition
            {
                ErrorHandler.Instance.HandleError(0, "NO CONTROLLER DETECTED");
                return;
            }
            else if(_controllerConnectionHandler.IsControllerValid())
            {
                _control = _controllerConnectionHandler.ConnectedController;
            }
        }

        load_model = pos;

        // load Telem_pg_3
        telem_panel_manager.LoadPage(3);
        instructions_text.SetText("Align your hand with the glove and click the SET POSITION button");
        

        // Load Hand Model and have user align glove
        Vector3 spawn_pos = _camera.transform.position;
        spawn_pos.z += 5;
        spawn_pos.y -= 1;
        spawn_pos.x -= 2; 
        glove_model.GetComponent<Renderer>().enabled = true;
        glove_model.transform.position = spawn_pos;
        glove_model.transform.rotation = Quaternion.Euler(180, 90, 0);

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
        glove_model.GetComponent<Renderer>().enabled = false;

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

        if(!IMU_GLOVE.getConnected() && mode != MOV_MODE.controller)
        {
            DebugManager.Instance.LogBoth("MODEL LOADER: ERROR GLOVE IMU NOT CONNECTED");
            ErrorHandler.Instance.HandleError(1, "MODEL LOADER: ERROR GLOVE IMU NOT CONNECTED");
            return;
        }
        else
        {
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

            if(IMU_GLOVE.getConnected() && TOGGLE_CHEST.getConnected() && TOGGLE_GLOVE.getConnected() && IMU_CHEST.getConnected())
            {
                loaded_models.Add( (GameObject)Instantiate(models[model_index], ModelList.transform));
                DebugManager.Instance.LogBoth("INFO", "Loading 3D Model " + models[model_index].name);
                instructions_text.SetText("Loading 3D Model " + models[model_index].name);

                load_model = -1;
                glove_model.GetComponent<Renderer>().enabled = false;
                instructions_text.SetText("Click the \"CLEAR ALL");
                ready_to_load = false;

                //Start IMU stream 
                IMU_GLOVE.startStream(); 
            }
            else
            {
                glove_model.GetComponent<Renderer>().enabled = false;
                
            }
            
        }
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
        switch(currentHighlighted % 5)
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
        if(currentHighlighted < (models.Count - 1))
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
