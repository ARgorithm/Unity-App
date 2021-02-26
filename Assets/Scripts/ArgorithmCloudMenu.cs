using UnityEngine;
using UnityEngine.UI;
using TMPro;
// import library of ARgorithm
using ARgorithm.Client;
using ARgorithm.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class ArgorithmCloudMenu : MonoBehaviour
{
    public GameObject ARgorithmCloudMenu;
    public GameObject ARMenu;
    public GameObject ARUIHeadingGameObject;
    public GameObject ArgorithmUiObject;
    public Transform PanelListHolderGameObject;
    // this function is called only when it is enabled or set to active
    void OnEnable()
    {
        list();
    }

    //Function to list algorithms present for showing animations of them
    public void list()
    {
        foreach (Transform child in PanelListHolderGameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        StartCoroutine(
            APIClient.Instance.list(
                (r) => callback(r)
            )
        );
    }

    void callback(ARgorithmCollection lar)
    {
        var NoOfAlgos = 0;
        //Instantiates the UI object(a prefab) dynamically to list the various algos
        foreach (ARgorithmModel item in lar.items)
        {
            var Item = Instantiate(ArgorithmUiObject);
            Item.transform.SetParent(PanelListHolderGameObject);
            Item.transform.localScale = new Vector3(1, 1, 1);
            var child = Item.transform.GetChild(0).gameObject;
            child.GetComponent<TextMeshProUGUI>().SetText(item.argorithmID.ToUpper());

            child = Item.transform.GetChild(1).gameObject;
            child.GetComponent<TextMeshProUGUI>().SetText(item.description.ToUpper());

            Button ExecuteButton = Item.transform.GetChild(4).GetComponent<Button>();
            ExecuteButton.onClick.AddListener(() =>
            {
                run(item.argorithmID);
            });

            NoOfAlgos += 1;
        }

        // If more than 5 algos are not present, fill up rest of the space with blank boxes
        if (NoOfAlgos < 5)
        {
            for (int i = 0; i < 5 - NoOfAlgos; i++)
            {
                var Item = Instantiate(ArgorithmUiObject);
                Item.transform.SetParent(PanelListHolderGameObject);
                Item.transform.localScale = new Vector3(1, 1, 1);

                foreach (Transform child in Item.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
    }

    public void run(string argorithmID)
    {
        // This sends an execution request to server and gets the states
        
        StartCoroutine(
            APIClient.Instance.run(
                new ExecutionRequest
                {
                    argorithmID = argorithmID,
                    parameters = new JObject()
                },
                (r) => callback(r,argorithmID)
             )
        );
    }

    void callback(ExecutionResponse response,string argorithmID)
    {
        /* 
        After the states are recieved, this function is invoked

        This function should send the states to the ARgorithm Parser which in turn send to ARTapToPlace
        */
        Debug.Log(response.status);
        string data = JsonConvert.SerializeObject(response);
        PlayerPrefs.SetString("StateSet", data);
        ARgorithmCloudMenu.SetActive(false);
        ARMenu.SetActive(true);
        changeHeading(argorithmID.ToUpper());
        //Debug.Log(PlayerPrefs.GetString("StateSet"));
    }
    public void changeHeading(string heading)
    {
        ARUIHeadingGameObject.GetComponent<TextMeshProUGUI>().SetText(heading);
    }
}
