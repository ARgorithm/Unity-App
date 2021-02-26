using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using System;
using TMPro;
using ARgorithm.Engine;
using Newtonsoft.Json;
using ARgorithm.Models;
public class ARTapToPlace : MonoBehaviour
{
    //private ARSessionOrigin arOrigin;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;
    private bool placed = false;

    public GameObject placementIndicator;
    public GameObject cube;
    public GameObject CommentBox;

    private Stage stage;


    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void FixedUpdate()
    {
        if (!placed)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();


            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
                ChangeComments("Tap to place objects on the table");
                placed = true;
            }
        }
    }

    private void PlaceObject()
    {

        // Change object here with the StateList/EventList
        // Instantiate everything here.
        // Instantiate(cube, PlacementPose.position, PlacementPose.rotation);

        string rawData = PlayerPrefs.GetString("StateSet");
        ExecutionResponse response = JsonConvert.DeserializeObject<ExecutionResponse>(rawData);
        Debug.Log(response.data);
        StageData sd = response.convertStageData();
        // GameObject indicator = (GameObject)Instantiate(Resources.Load("PlacementIndicator") as GameObject, new Vector3(0, 0, 0), Quaternion.identity);
        // indicator.SetActive(true);
        stage = new Stage(sd, placementIndicator);
    }

    public void Next()
    {
        stage.Next();
    }

    private void ChangeComments(string text)
    {
        // called in updates based on EventList contents with "comments" key
        CommentBox.GetComponent<TextMeshProUGUI>().SetText(text); 
    }
    
    //Dont Change the code below this comment 
    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            PlacementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}