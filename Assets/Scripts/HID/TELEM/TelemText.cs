using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TelemText : MonoBehaviour
{
    public StyleSheet style;
    public TelemWebRequest webRequest;

    public TextMeshProUGUI[] suit_names;
    public TextMeshProUGUI[] suit_data;
    public TextMeshProUGUI[] switch_names;
    public TextMeshProUGUI[] switch_data;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {

    }

    // Update is called once per frame
    public void UpdateText()
    {

        // SUIT
        foreach (TextMeshProUGUI obj in suit_data)
        {
            //int temp = obj.name.Length;
            //Debug.Log("LEN: " + temp);


            string removeHeader = obj.name.Substring(5, obj.name.Length - 5);
            //removeHeader = removeHeader;
            //Debug.Log("REMOVE: " + removeHeader);

            // set text color

            string[] returnStr = webRequest.GetDataFromString(removeHeader);
            obj.SetText(returnStr[0]);

            if (returnStr[1] == "red" || returnStr[0] == "false")
            {
                obj.color = new Color32(255, 0, 0, 255);   // red
            } else if (returnStr[1] == "green" || returnStr[0] == "true")
            {
                obj.color = new Color32(0, 255, 0, 255);   // green
            }
           
        }

        // SWITCH
        foreach (TextMeshProUGUI obj in switch_data)
        {
            //int temp = obj.name.Length;
            //Debug.Log("LEN: " + temp);


            //string removeHeader = obj.name.Substring(5, obj.name.Length - 5);
            //removeHeader = removeHeader;
            //Debug.Log("SEARCH: " + obj.name);
            string[] returnStr = webRequest.GetDataFromString(obj.name);
            obj.SetText(returnStr[0]);

            if (returnStr[1] == "red" || returnStr[0] == "False")
            {
                obj.color = new Color32(255, 0, 0, 255);   // red
            } else if (returnStr[1] == "green" || returnStr[0] == "True")
            {
                obj.color = new Color32(0, 255, 0, 255);   // green
            }
        }
    }
}
