using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recordAudio : MonoBehaviour
{
    public int MAX_RECORD_TIME = 300; 

    private AudioSource audioSource = null;
   
    List<float> tempRecording = new List<float>();

    List<float[]> recordedClips = new List<float[]>();

    int idx; 

    private int minFreq;  
    private int maxFreq;

    private bool micConnected;

    public bool isMicConnected() { return micConnected; }

    // Start is called before the first frame update
    void Start()
    {
        idx = 0;

        audioSource = GetComponent<AudioSource>();

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
            SavWav.Save("recording" + idx, audioSource.clip);
            idx += 1;
        }
    }

}
