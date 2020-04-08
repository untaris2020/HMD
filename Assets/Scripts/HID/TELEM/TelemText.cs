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

            obj.SetText(webRequest.GetDataFromString(removeHeader));

            // set text color
           
        }

        // SWITCH
        foreach (TextMeshProUGUI obj in switch_data)
        {
            //int temp = obj.name.Length;
            //Debug.Log("LEN: " + temp);


            //string removeHeader = obj.name.Substring(5, obj.name.Length - 5);
            //removeHeader = removeHeader;
            Debug.Log("SEARCH: " + obj.name);

            obj.SetText(webRequest.GetDataFromString(obj.name));

            if (obj.text == "false")
            {
                // red

            } else
            {
                // green

            }
           
        }
    }
}
