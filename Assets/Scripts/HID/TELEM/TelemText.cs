using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TelemText : MonoBehaviour
{
    public StyleSheet style;
    public TelemWebRequest webRequest;

    public TextMeshProUGUI[] names;
    public TextMeshProUGUI[] data;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        foreach (TextMeshProUGUI obj in data)
        {
            //int temp = obj.name.Length;
            //Debug.Log("LEN: " + temp);


            string removeHeader = obj.name.Substring(5, obj.name.Length - 5);
            //removeHeader = removeHeader;
            //Debug.Log("REMOVE: " + removeHeader);

            obj.SetText(webRequest.GetDataFromString(removeHeader));
           
        }

        foreach (TextMeshProUGUI obj in names)
        {
            // TODO: change name color based on error
        }
    }
}
