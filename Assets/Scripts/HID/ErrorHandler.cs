using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// TODO
// 1) Handle errors if error window already active
// 2) Fade animation

public class ErrorHandler : MonoBehaviour {

    // Use this for initialization
    public GameObject errorPanel;
    public TextMeshProUGUI myText; 

    public AudioClip error;
        AudioSource source;

    public float VOL = 1.7f;

    //ErrorWindow windowInstance;
    public bool windowActive;

    public List<string> ErrorList; 

    void Start () {
        windowActive = false;
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if((ErrorList.Count > 0) && (windowActive == false))
        {
            SpawnErrorWindow();

        }
    }

    public int HandleError(int Action, string ErrorMsg)
    {

        ErrorList.Add(ErrorMsg);
        return 0;
    }

    public void SpawnErrorWindow()
    {
        windowActive = true;
        //float trans = 0f;
        myText.text = ErrorList[0];
        errorPanel.SetActive(true);
        source.PlayOneShot(error,VOL);

    }

    public void CloseErrorWindow()
    {
        ErrorList.RemoveAt(0);
        errorPanel.SetActive(false);
        windowActive = false;
       // Debug.Log("Item at 0" + ErrorList[0]);
 

        // fade out
        //float trans = 1.0f;
       // if (trans > 0.0)
       // {
           
       //     trans -= 0.03f;
       // }
        // destroy object
       // Destroy(windowInstance.gameObject);

    }
}
    





        // spawn window with countdown bar
       // Debug.Log("INFO: Error Window Spawned");
       // windowInstance = Instantiate(ErrorPrefab, transform.position, Quaternion.identity) as ErrorWindow;

        // set paramaters
       // windowInstance.SetText(ErrorMsg);
       // windowInstance.transform.position = new Vector3(0.0f, 0.08f, 0.5f);
       // windowInstance.transform.localScale = new Vector3(0.25f, 0.25f, 0.0999f);

        // fade in
        // TODO 2
        //if (trans < 1.0)
        //{
            // windowInstance.GetComponent<Renderer>().material.color.a = trans;
        //    trans += 0.001f;
        //}



    // old countdown code
    /*
    IEnumerator ErrorWindowTimeout()
    {
        float TimeLeft = 100.0f;
        float trans = 0f;
        while (TimeLeft > 0)
        {
            // fade in
            // TODO 2
            if (trans < 1.0)
            {
               // windowInstance.GetComponent<Renderer>().material.color.a = trans;
                trans += 0.001f;
            }
            
            yield return new WaitForSeconds(0.05f);
            TimeLeft--;
        }
        
        CloseErrorWindow();
    }
    */