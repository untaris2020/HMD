﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class SimulatorEventSystem : MonoBehaviour
{
    public GameObject hid;
    public GameObject soilParticleSystem;
    AudioSource source;


    [Space]
    public GameObject mmsev_broken;
    public GameObject mmsev;
    public GameObject mmsev_tire;
    public AudioClip mmsev_audio;

    [Space]
    public GameObject[] soilSamples;
    public AudioClip soil_audio;

    List<PlaceOfIntrest> placesOfInterest = new List<PlaceOfIntrest>();
    //List<GameObject> placesOfIntrestMarkers = new List<GameObject>();
    public GameObject marker;

    //float VOL = 1.7f;
    float interactionDistance = 15;
    bool has_fixed_mmsev = false;

    void Start()
    {
        source = hid.GetComponent<AudioSource>();

        placesOfInterest.Add(new PlaceOfIntrest(mmsev_broken, marker));

        foreach (var obj in soilSamples) {
            placesOfInterest.Add(new PlaceOfIntrest(obj, marker));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckUserEvent(GameObject user) {
        
        if ((user.transform.position - mmsev_broken.transform.position).sqrMagnitude < interactionDistance*interactionDistance) {
            if (!has_fixed_mmsev) {
                FixMMSEV();
                has_fixed_mmsev = true;
            }
            
        }

        foreach(GameObject sample in soilSamples) {
            if (sample != null) {
                    if ((user.transform.position - sample.transform.position).sqrMagnitude < interactionDistance*interactionDistance) {
                    CollectSample(sample);
                }
            }
        }
    }

    void CollectSample(GameObject sample) {
        UserVisited(sample);
        source.PlayOneShot(soil_audio, 1.0f);
        GameObject probaParticleClone = Instantiate(soilParticleSystem, sample.transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f))) as GameObject;
        Destroy(probaParticleClone, 4);
        Destroy(sample);
    }

    void FixMMSEV() {
        UserVisited(mmsev_broken);

        source.PlayOneShot(mmsev_audio, 1.0f);
        //mmsev_tire.transform.position = Vector3.MoveTowards();

        StartCoroutine(MoveToPosition(mmsev_tire.transform, new Vector3(298.68f, 0.8f, 231.78f), Time.deltaTime * 180f));
        StartCoroutine(RotateToPosition(mmsev_tire.transform, new Vector3(0f, 0f, 0f), Time.deltaTime * 180f));

        //mmsev_tire.transform.rotation = Quaternion.RotateTowards(mmsev_tire.transform.rotation, new Quaternion(0f, 0f, 0f, 0f), Time.deltaTime*180f); 
    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while(t < 1)
        {
                t += Time.deltaTime / timeToMove;
                transform.position = Vector3.Lerp(currentPos, position, t);
                yield return null;
        }
    }

    public IEnumerator RotateToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.rotation;
        var t = 0f;
        while(t < 1)
        {
                t += Time.deltaTime / timeToMove;
                transform.rotation = Quaternion.Lerp(currentPos, Quaternion.Euler(position), t);
                yield return null;
        }
    }

    void UserVisited(GameObject obj) {
        Debug.Log("Uservisited: " + obj.name);
        foreach  (PlaceOfIntrest place in placesOfInterest) {
            if (place.parentInstanceID == obj.GetInstanceID()) {
                place.hasUserInteracted = true;
                place.marker.SetActive(false);
            }
        }
    }

    public class PlaceOfIntrest {
        public int parentInstanceID;
        public GameObject marker;

        public bool hasUserInteracted = false;

        public PlaceOfIntrest(GameObject _parent, GameObject _marker) {
            parentInstanceID = _parent.GetInstanceID();

            if (_marker != null) {
                
                marker = Instantiate(_marker, _parent.transform.position + new Vector3(0, 55f, 0f), Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                marker.SetActive(true);
            }
        }
    }

}



