using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using System;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Linq;

using ARgorithm.Structure;
using ARgorithm.Models;

public class ARStage : MonoBehaviour
{
    //private ARSessionOrigin arOrigin;
    private Pose PlacementPose;
    private ARRaycastManager ARRayCastManager;
    private bool placementPoseIsValid = false;
    private bool placed = true;

    public GameObject placementIndicator;
    public GameObject cube;
    public GameObject CommentBox;
    public GameObject ARgorithmHeading;

    private StageData stageData;
    private int index;
    private Dictionary<string, GameObject> idToPlaceholderMap;
    

    void Start()
    {
        // Sets Header of UI
        string argorithmID = PlayerPrefs.GetString("argorithmID");
        ARgorithmHeading.GetComponent<TextMeshProUGUI>().SetText(argorithmID);
        
        // Initialises the metadata that will be used to control the animations
        index = -1;
        placementIndicator.SetActive(false);
        ARRayCastManager = FindObjectOfType<ARRaycastManager>();
        idToPlaceholderMap = new Dictionary<string, GameObject>();
        string rawData = PlayerPrefs.GetString("StateSet");
        ExecutionResponse response = JsonConvert.DeserializeObject<ExecutionResponse>(rawData);
        stageData = response.convertStageData();

        // If first state is a `declare` state then we activate placement indicator
        State state = stageData.states[0];
        string funcType = state.state_type.Split('_').ToList()[1];
        if (funcType == "declare")
            placed = false;
            ChangeComments("Press Play to place new object. Please place objects properly spaced");
    }

    void FixedUpdate()
    {
        if (!placed) {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("CloudMenuEnabled");
    }

    private void ChangeComments(string text)
    {
        // called in updates based on EventList contents with "comments" key
        CommentBox.GetComponent<TextMeshProUGUI>().SetText(text); 
    }

    //Dont Change the code below this comment 
    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && !placed)
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
        ARRayCastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            PlacementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
    public void Next()
    {
        index++;
        if (index >= stageData.size)
        {
            index = stageData.size - 1;
            return;
        }
        State args = stageData.states[index];
        if (args.state_type != "comment")
        {
            /* 
            Should remove this and alter a code a bit because this is logically wrong
            But It works :)
            */
        }
        string id = (string)args.state_def["id"];
        string funcType = args.state_type.Split('_').ToList()[1];
        if (stageData.objectMap[id].rendered && funcType == "declare")
        {
            idToPlaceholderMap[id].SetActive(true);
            return;
        }
        if (!stageData.objectMap[id].rendered && funcType == "declare" && !placed)
        {
            /*ask user to set position of object if not set already*/
            placementIndicator.SetActive(false);
            placed = true;
            GameObject placeHolder = new GameObject("id:" + id);
            placeHolder.transform.position = placementIndicator.transform.position;
            placeHolder.transform.rotation = placementIndicator.transform.rotation;
            idToPlaceholderMap[id] = placeHolder;
        }

        ChangeComments(args.comments);
        stageData.eventList[index](args, idToPlaceholderMap[id]);
        if (index + 1 < stageData.states.Count)
        {
            State nextState = stageData.states[index + 1];
            string nextFuncType = nextState.state_type.Split('_').ToList()[1];
            if (nextFuncType == "declare")
            {
                placed = false;
                ChangeComments("Press Play to place new object. Please place objects properly spaced");
            }

        }
    }
    public void Undo()
    {
        // Called the structure Undo method to reset values without animation
        if (index <= -1)
            return;
        State args = stageData.states[index];
        if (args.state_type == "comments")
        {
            index--;
            return;
        }

        JObject stateDef = args.state_def;
        string id = (string)stateDef["id"];
        ChangeComments(" ");
        string funcType = args.state_type.Split('_').ToList()[1];
        if (funcType == "declare")
        {
            idToPlaceholderMap[id].SetActive(false);
        }

        BaseStructure currStructure = stageData.objectMap[id];
        currStructure.Undo(args);
        index--;
    }
    public void BackButton()
    {
        // Exit Scene
        PlayerPrefs.SetInt("CloudMenuEnabled", 1);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex - 1);
    }

    public void Reset()
    {
        // Reset Scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}