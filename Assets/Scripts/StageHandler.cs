using UnityEngine;
using ARgorithm.Engine;
using Newtonsoft.Json;
using ARgorithm.Models;
public class StageHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private Stage stage;
    void Start()
    {
        string rawData = PlayerPrefs.GetString("StateSet");
        ExecutionResponse response = JsonConvert.DeserializeObject<ExecutionResponse>(rawData);
        Debug.Log(response.data);
        StageData sd = response.convertStageData();
        GameObject indicator = (GameObject)Instantiate(Resources.Load("PlacementIndicator") as GameObject, new Vector3(0,0,0), Quaternion.identity);
        indicator.SetActive(true);
        stage = new Stage(sd, indicator);
    }

    public void Next()
    {
        stage.Next();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
