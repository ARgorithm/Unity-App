using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARgorithm.Client;
using ARgorithm.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using ARgorithm.Exception;

public class ParametersMenu : MonoBehaviour
{
    public GameObject ARgorithmCloudMenu;
    public GameObject MainMenu;
    public GameObject ParameterMenu;
    //Parameters Menu -> Middle Panel -> Variables
    public GameObject VariablesGameObject;
    public Button SubmitParametersButton;
    public JObject parametersInfo;

    private Dictionary<string, Dictionary<string, JToken>> objectParameter ;

    void OnEnable()
    {
        foreach (Transform child in VariablesGameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        /*
         * On Getting Values from ARgorithmCloudMenu for parametersInfo jObject
         * It takes some time to do so. So a timer is set withn the Coroutine for
         * the jobject to load all the values
        */
        Debug.Log(parametersInfo);
        SettingUpParametersInfo();
    }

    private void SettingUpParametersInfo()
    {
        this.objectParameter = new Dictionary<string, Dictionary<string, JToken>>();
        foreach (var property in parametersInfo)
        {
            Dictionary<string, JToken> parametersDict = new Dictionary<string, JToken>();
            foreach(JProperty entry in property.Value)
                parametersDict[entry.Name] = entry.Value;
            objectParameter[property.Key] = parametersDict;
        }

        foreach (string variable in objectParameter.Keys)
        {
            string type = objectParameter[variable]["type"].ToString();
            string description = objectParameter[variable]["description"].ToString();

            switch (type)
            {
                case "INT":
                    SetupIntFloatParameters<int>(variable, description);
                    break;
                case "FLOAT":
                    SetupIntFloatParameters<float>(variable, description);
                    break;
                case "STRING":
                    SetupStringParameters(variable, description);
                    break;
                case "ARRAY":
                    string itemType = objectParameter[variable]["item-type"].ToString();
                    if (itemType == "INT")
                        SetupArrayParameters<int>(variable, description);
                    else if (itemType == "FLOAT")
                        SetupArrayParameters<float>(variable, description);
                    else if (itemType == "STRING")
                        SetupArrayParameters<string>(variable, description);
                    else
                        throw new ARgorithmException("Invalid item type expected");
                    break;
                case "MATRIX":
                    break;
            }
        }
    }
    private void SetupIntFloatParameters<T>(string variable,string description)
    {
        /*
        * Instantiates Int Parameter Prefab
        */
        GameObject intParameter = Instantiate(Resources.Load("IntParameterPrefab") as GameObject);
        intParameter.transform.parent = VariablesGameObject.transform;
        intParameter.transform.localScale = new Vector3(1, 1, 1);
        /*
        * Child holds the text component within Int Parameter Prefab and sets variable name
        */
        var child = intParameter.transform.GetChild(0).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(variable);
        /*
        * intInputField holds the intinputfield component within int Parameter Prefab
        * and onEdit completion stores its value
        */
        T value;
        InputField valueInputField = intParameter.transform.GetChild(1).gameObject.GetComponent<InputField>();
        valueInputField.onEndEdit.AddListener(delegate
        {
            try
            {
                value = Parse<T>(valueInputField.text);
                child = intParameter.transform.GetChild(3).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
            }
            catch (ARgorithmException e)
            {
                child = intParameter.transform.GetChild(3).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
            }
            catch
            {
                child = intParameter.transform.GetChild(3).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Error in Input");
            }
            
        });
        /*
        * Child holds the description text component and sets description
        */
        child = intParameter.transform.GetChild(2).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(description);
    }
    private void SetupStringParameters(string variable, string description)
    {
        /*
        * Instantiates Int Parameter Prefab
        */
        GameObject stringParameter = Instantiate(Resources.Load("StringParameterPrefab") as GameObject);
        stringParameter.transform.parent = VariablesGameObject.transform;
        stringParameter.transform.localScale = new Vector3(1, 1, 1);
        /*
        * Child holds the text component within Int Parameter Prefab and sets variable name
        */
        var child = stringParameter.transform.GetChild(0).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(variable);
        /*
        * intInputField holds the intinputfield component within int Parameter Prefab
        * and onEdit completion stores its value
        */
        string value = "";
        InputField valueInputField = stringParameter.transform.GetChild(1).gameObject.GetComponent<InputField>();
        valueInputField.onEndEdit.AddListener(delegate
        {
            try
            {
                value = valueInputField.text;
                child = stringParameter.transform.GetChild(3).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
                Debug.Log(value.ToString());

            }
            catch (ARgorithmException e)
            {
                child = stringParameter.transform.GetChild(3).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
            }
            catch
            {
                child = stringParameter.transform.GetChild(3).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Error in Input");
            }

        });
        /*
        * Child holds the description text component and sets description
        */
        child = stringParameter.transform.GetChild(2).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(description);
    }

    private void SetupArrayParameters<T>(string variable,string description)
    {
        /*
        * Instantiates Array Parameter Prefab
        */
        GameObject arrayParameter = Instantiate(Resources.Load("ArrayParameterPrefab") as GameObject);
        arrayParameter.transform.parent = VariablesGameObject.transform;
        arrayParameter.transform.localScale = new Vector3(1, 1, 1);
        /*
        * Child holds the text component within Array Parameter Prefab and sets variable name
        */
        var child = arrayParameter.transform.GetChild(0).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(variable);
        /*
        * child holds the element type component within Array Parameter Prefab and sets the Element type
        */
        child = arrayParameter.transform.GetChild(2).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(objectParameter[variable]["item-type"].ToString().ToLower()+" type");
        /*
        * sizeInputField holds the array size inputfield component within Array Parameter Prefab
        * and onEdit completion store in size variable
        */
        InputField sizeInputField = arrayParameter.transform.GetChild(1).gameObject.GetComponent<InputField>();
        int arraySize = 0;
        sizeInputField.onEndEdit.AddListener(delegate
        {
            try
            {
                arraySize = int.Parse(sizeInputField.text);
                if (arraySize <= 0)
                {
                    throw new ARgorithmException("Size cannot be lesser than or equal to Zero");
                }
                child = arrayParameter.transform.GetChild(4).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
            }
            catch(ARgorithmException e)
            {
                child = arrayParameter.transform.GetChild(4).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
            }
            catch 
            {
                child = arrayParameter.transform.GetChild(4).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Invalid input in size field");
            }
        });
      
        /*
        * arrayElementsInputField holds the array elementsinputfield component within Array Parameter Prefab
        * and onEdit completion store in arrayElemets variable
        */
        T[] arrayElements = new T[arraySize];

        InputField arrayElementsInputField = arrayParameter.transform.GetChild(3).gameObject.GetComponent<InputField>();
        arrayElementsInputField.onEndEdit.AddListener(delegate
        {
            try
            {
                T[] arrayInputs = System.Array.ConvertAll(arrayElementsInputField.text.Split(' '), Parse<T>);
                if(arrayInputs.Length != arraySize)
                {
                    throw new ARgorithmException("Array size doesn't match with input size");
                }
                arrayElements = arrayInputs;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
            }
            catch(ARgorithmException e)
            {
                child = arrayParameter.transform.GetChild(5).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
            }
            catch
            {
                child = arrayParameter.transform.GetChild(5).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Input Error in Array Elements");
            }
        });
        /*
        * Child holds the description text component and sets description
        */
        child = arrayParameter.transform.GetChild(4).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(description);
    }

    void OnDisable()
    {
        Debug.Log("Parameters Menu diasbled");
    }
   
    private T Parse<T>(string elem)
    {
        if (typeof(T) == typeof(int))
        {
            return (T)(object)int.Parse(elem);
        }
        if (typeof(T) == typeof(float))
        {
            return (T)(object)float.Parse(elem);
        }
        if (typeof(T) == typeof(string))
        {
            return (T)(object)(elem);
        }
        return (T)(object)null;
    }


    public void RunParameters()
    {
        // This sends an execution request to server and gets the states
        string argorithmID = PlayerPrefs.GetString("argorithmID");
        
        /*
         StartCoroutine(
            APIClient.Instance.run(
                new ExecutionRequest
                {
                    argorithmID = argorithmID,
                    parameters = new JObject()
                },
                (r) => callback(r, argorithmID)
             )
        );
        */
    }
    public void RunDefault()
    {
        // This sends an execution request to server and gets the states

        string argorithmID = PlayerPrefs.GetString("argorithmID");

        StartCoroutine(
            APIClient.Instance.run(
                new ExecutionRequest
                {
                    argorithmID = argorithmID,
                    parameters = new JObject()
                },
                (r) => callback(r, argorithmID)
             )
        );
    }

    void callback(ExecutionResponse response, string argorithmID)
    {
        /* 
        After the states are recieved, this function is invoked

        This function should send the states to the ARgorithm Parser which in turn send to ARTapToPlace
        */
        Debug.Log(response.status);
        string data = JsonConvert.SerializeObject(response);
        PlayerPrefs.SetString("StateSet", data);
        //PlayerPrefs.SetString("argorithmID", argorithmID);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
        //Debug.Log(PlayerPrefs.GetString("StateSet"));
    }

    public void CrossButton()
    {
        PlayerPrefs.DeleteKey("parametersInfo");
        PlayerPrefs.DeleteKey("argorithmID");
        ParameterMenu.SetActive(false);
        ARgorithmCloudMenu.SetActive(true);
    }
}
