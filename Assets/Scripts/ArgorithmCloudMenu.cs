using UnityEngine;
using UnityEngine.UI;
using TMPro;
// import library of ARgorithm
using ARgorithm.Client;
using ARgorithm.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class ArgorithmCloudMenu : MonoBehaviour
{
    public GameObject ARgorithmCloudMenu;
    public GameObject MainMenu;
    public GameObject ArgorithmUiObject;
    public GameObject ParameterMenu;
    public Transform PanelListHolderGameObject;
    private float panelListHeight;
    private bool flag = false;
    // this function is called only when it is enabled or set to active
    void OnEnable()
    {
        list();
    }

    private void Update()
    {
        if (panelListHeight > 0 && !flag)
        {
            RectTransform panelRT = PanelListHolderGameObject.transform.GetComponent<RectTransform>();
            //panelRT.sizeDelta = new Vector2(0, this.panelListHeight);
            Debug.Log(this.panelListHeight);
            panelRT.offsetMin = new Vector2(0, -this.panelListHeight);
            flag = true;
        }
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
        this.panelListHeight = 0;
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
                
                PlayerPrefs.SetString("argorithmID", item.argorithmID);
                JObject jb =  item.parameters;
                ParameterMenu.GetComponent<ParametersMenu>().parametersInfo = jb;
                ARgorithmCloudMenu.SetActive(false);
                ParameterMenu.SetActive(true);
            });

            RectTransform rt = Item.transform.GetComponent<RectTransform>();
            this.panelListHeight += rt.rect.height+50;
            
        }

        /*
        
        for(int i=0;i<40;i++)
        {
            var Item = Instantiate(ArgorithmUiObject);
            Item.transform.SetParent(PanelListHolderGameObject);
            Item.transform.localScale = new Vector3(1, 1, 1);
            RectTransform rt = Item.transform.GetComponent<RectTransform>();
            this.panelListHeight += rt.rect.height + 50;
        }
        */
    }
    public void BackButton()
    {
        PlayerPrefs.DeleteKey("CloudMenuEnabled");
        ARgorithmCloudMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

}
