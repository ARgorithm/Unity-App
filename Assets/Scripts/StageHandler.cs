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
        StageData sd = response.convertStageData();
        stage = new Stage(sd);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
