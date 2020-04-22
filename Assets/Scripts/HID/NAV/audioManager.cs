using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class audioManager : MonoBehaviour
{
    public GameObject recordButton;
    public TextMeshProUGUI recordButtonText;
    public TextMeshProUGUI recordTime;
    public GameObject playBackButton;
    public TextMeshProUGUI playBackButtonText;
    public TextMeshProUGUI playBackTime;
    public TextMeshProUGUI playBackTitle; 
    public GameObject recordingIndicator; 
    public Collider navPG2Collider; 

    private bool recording;
    private float updateTime;
    private float waitTime = 1.0f;
    private float startTime;
    private float prevTime;
    private delegate void functionDelegate();
    private recordAudio ar;
    private bool playBackState;
    private bool begAud = true; 
    private bool getPlayBackState() { return playBackState; }
    private void setPlayBackState(bool state) { playBackState = state; }

    private int playBackMin;
    private int playBackSec; 

    public bool getRecording() { return recording; }
    // Start is called before the first frame update
    void Start()
    {
        HeadTracking ht = GameObject.Find("SceneManager").GetComponent<HeadTracking>();
        ar = this.GetComponent<recordAudio>();
        functionDelegate rec = new functionDelegate(recordingHit);

        forceSensorManager.fingerInput input = new forceSensorManager.fingerInput(0, 0, 1, 0, 0);
        ht.registerCollider(recordButton.GetComponentInChildren<Collider>().name, navPG2Collider.name, rec, input);

        functionDelegate pb = new functionDelegate(playbackHit);

        input = new forceSensorManager.fingerInput(0, 1, 0, 0, 0);
        ht.registerCollider(playBackButton.GetComponentInChildren<Collider>().name, navPG2Collider.name, pb, input);

        Time.timeScale = 1.0f;
        updateTime = 0f;
        prevTime = 0f;
        //recordTime.SetActive(false);
        recordingIndicator.GetComponent<MeshRenderer>().material = StyleSheet.Instance.Icons; 
        recordButtonText.SetText("START RECORDING");
        recordTime.SetText("");
        recording = false;

        playBackTime.SetText("");
        playBackButtonText.SetText("PLAY AUDIO LOG");
    }

   
    // Update is called once per frame
    void Update()
    {
        if(ar.isMicConnected())
        {
            if(recording)
            {
                updateTime = Time.time - prevTime;

                if(updateTime > waitTime)
                {
                    prevTime = Time.time;
                    float timeElapsed = Time.time - startTime; 
                    string minutes = Mathf.Floor(timeElapsed/ 60).ToString("00");
                    string seconds = Mathf.Floor(timeElapsed % 60).ToString("00");

                    recordTime.SetText( minutes + ":" + seconds);
                    updateTime = 0f; 
                }
            }
            
        }
        audioLoader al = GetComponent<audioLoader>();
        if(al.getPlayback() != -1)
        {
            if (!al.getCurrSource().isPlaying && playBackState) //if we are not playing and we should be we reached the end so reset 
            {
                //reset time output here
                begAud = true;
                playBackButtonText.SetText("PLAY AUDIO LOG");
                string minutes = Mathf.Floor(al.getCurrSource().time/ 60).ToString("00");
                string seconds = Mathf.Floor(al.getCurrSource().time % 60).ToString("00");
                playBackTime.SetText(minutes + ":" + seconds);
            }
            else if(playBackState && al.getCurrSource().isPlaying)
            {
                //adjust time output here 
                string minutes = Mathf.Floor(al.getCurrSource().time/ 60).ToString("00");
                string seconds = Mathf.Floor(al.getCurrSource().time % 60).ToString("00");
                playBackTime.SetText(minutes + ":" + seconds);
            }
        }
        
    }

    public void recordingHit()
    {
        if(ar.isMicConnected())
        {
            if(recording)
            {
                recordTime.SetText("");
                recordingIndicator.GetComponent<MeshRenderer>().material = StyleSheet.Instance.Icons; 
                recordButtonText.SetText("START RECORDING");
                recording = false;
                ar.stopRecording();
            }
            else
            {
                recordingIndicator.GetComponent<MeshRenderer>().material = StyleSheet.Instance.Red; 
                recordButtonText.SetText("STOP RECORDING");
                recordTime.SetText("00:00");
                recording = true;
                startTime = Time.time;
                prevTime = Time.time;
                ar.startRecording();
            }
        }
        else
        {
            Debug.Log("NO MIC");
        }
    }  
    public void playbackHit()
    {
        audioLoader al = GetComponent<audioLoader>();
        if(begAud && al.getPlayback() != -1)
        {
            al.restartAudio();
            playBackButtonText.SetText("PAUSE PLAYBACK");
            playBackState = true;
            begAud = false;
        }
        else if(playBackState) //if we are currently running audio 
        {
            al.pauseAudio();
            playBackButtonText.SetText("PLAY AUDIO LOG");
            playBackState = false; 
            
        }
        else if(!playBackState && al.getPlayback() != -1)
        {
            al.unPause();
            playBackButtonText.SetText("PAUSE PLAYBACK");
            playBackState = true; 
            
        }
    }
    public void loadTitle()
    {
        audioLoader al = GetComponent<audioLoader>();
        if (al.getPlayback() != -1)
        {
            audioLoader.recording rec = al.GetRecording(al.getPlayback());
            string minutes = Mathf.Floor(rec.length / 60).ToString("00");
            string seconds = Mathf.Floor(rec.length % 60).ToString("00");
            playBackTitle.SetText(rec.Name + ": LENGTH - " + minutes + ":" + seconds + " AT TIME - " + rec.Time.ToString());
        }
    }
}
