using UnityEngine;
using TMPro;

public class ModelLoader : MonoBehaviour
{
    // buttons
    public GameObject clearModelsButton, misc1Button, misc2Button, upButton, downButton, upArrow;
    public GameObject[] modelButtons;

    // text
    public TextMeshProUGUI[] modelTexts;
    public TextMeshProUGUI heading_text;
    public TextMeshProUGUI index_text;  // left
    public TextMeshProUGUI misc_text;   // right - unused

    // array/list of models
    private int NUMOFMODELS = 13;
    private int NUMOFPAGES;
    private int page_index = 0;

    private delegate void functionDelegate();


    // Start is called before the first frame update
    void Start()
    {
        NUMOFPAGES = (int)System.Math.Ceiling((double)NUMOFMODELS / 5);
        DebugManager.Instance.LogUnityConsole("PAGES", NUMOFPAGES.ToString());

        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();

        // clear models button
        functionDelegate tmpDelegate0 = new functionDelegate(ClearModelsButton);
        ht.registerCollider(clearModelsButton.GetComponent<Collider>().name, tmpDelegate0);
        functionDebug.Instance.registerFunction("clearModels", tmpDelegate0);

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

    private void ClearModelsButton()
    {

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

    }

    private void LoadModel1()
    {

    }

    private void LoadModel2()
    {

    }

    private void LoadModel3()
    {

    }

    private void LoadModel4()
    {

    }
}
