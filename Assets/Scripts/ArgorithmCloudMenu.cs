using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// import library of ARgorithm
using ARgorithmAPI;
using ARgorithmAPI.Models;


public class ArgorithmCloudMenu : MonoBehaviour
{
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
        foreach (ARgorithm item in lar.items)
        {
            Debug.Log(item.argorithmID);
            var Item = Instantiate(ArgorithmUiObject);
            Item.transform.SetParent(PanelListHolderGameObject);
            Item.transform.localScale = new Vector3(1, 1, 1);
            var child = Item.transform.GetChild(0).gameObject;
            child.GetComponent<TextMeshProUGUI>().SetText(item.argorithmID.ToUpper());

            child = Item.transform.GetChild(1).gameObject;
            child.GetComponent<TextMeshProUGUI>().SetText(item.description.ToUpper());

            NoOfAlgos += 1;
        }

        // If more than 5 algos are not present, fill up rest of the space with blank boxes
        if (NoOfAlgos < 5)
        {
            for(int i = 0; i < 5 - NoOfAlgos; i++)
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

}
