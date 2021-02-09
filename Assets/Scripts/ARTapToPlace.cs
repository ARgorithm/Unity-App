using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using System;
using TMPro;

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


    void onEnable()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
      
    }

    void Update()
    {
        if (!placed)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();


            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
                ChangeComments("Tap to place objects on the table");
                placementIndicator.SetActive(false);
                placed = true;
            }
        }
    }

    private void PlaceObject()
    {
        Instantiate(cube, PlacementPose.position, PlacementPose.rotation);
    }

    private void ChangeComments(string text)
    {
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