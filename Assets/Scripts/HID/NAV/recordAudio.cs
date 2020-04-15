using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recordAudio : MonoBehaviour
{
    public int MAX_RECORD_TIME = 300; 
    private audioLoader al; 
    private AudioSource audioSource = null;
    List<float> tempRecording = new List<float>();
    List<float[]> recordedClips = new List<float[]>();
    private string AudioPathDir; 
    int idx;
    private bool firstClip;
    private int minFreq;  
    private int maxFreq;

    private bool micConnected;

    public bool isMicConnected() { return micConnected; }

    // Start is called before the first frame update
    void Start()
    {
        idx = 0;
        al = GetComponent<audioLoader>();
        audioSource = GetComponent<AudioSource>();

        firstClip = true; 
        //Check if there is at least one microphone connected  
        if (Microphone.devices.Length <= 0)
        {
            DebugManager.Instance.LogBoth("NO MICROPHONE DETECTED");
            micConnected = false;
        }
        else
        {
            micConnected = true;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            if (minFreq == 0 && maxFreq == 0)
            {
                maxFreq = 44100;  //Default case use 44100 Hz rate
            }
        }
    }

    public void startRecording()
    {
        if(micConnected)
        { 
            audioSource.clip = Microphone.Start(null, true, MAX_RECORD_TIME, maxFreq); 
        }
            
    }
    
    public void stopRecording()
    {
        if(micConnected)
        {
            int lastTime = Microphone.GetPosition(null);
            Microphone.End(null);
            float[] samples = new float[audioSource.clip.samples]; 
            audioSource.clip.GetData(samples, 0);
            float[] ClipSamples = new float[lastTime];
            Array.Copy(samples, ClipSamples, ClipSamples.Length - 1);
            audioSource.clip = AudioClip.Create("playRecordClip", ClipSamples.Length, 1, 44100, false);
            audioSource.clip.SetData(ClipSamples, 0);
            if(firstClip)
            {
                AudioPathDir = DateTime.Now.ToString("MMM_dd_HH_mm");
                firstClip = false; 
            }
            

            SavWav.Save(AudioPathDir + "/" + "recording" + idx + ".wav", audioSource.clip);
            al.LoadNewAudio(audioSource.clip, DateTime.Now, ("Recording " + idx));
            idx += 1;
        }
    }

     private string FormatTime(float time)
     {
        int minutes = (int)time/60;
        int seconds = (int)time-60*minutes;
        int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds );
     }
}
