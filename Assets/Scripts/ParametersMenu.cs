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
    public GameObject ParameterGameObject;
    public Button SubmitParametersButton;
    public JObject parametersInfo;
    private float panelListHeight;
    private bool panelFlag = false;
    private Dictionary<string, Dictionary<string, JToken>> objectParameter;
    private Dictionary<string, JToken> customParameters;
    void OnEnable()
    {
        customParameters = new Dictionary<string, JToken>();
        
        foreach (Transform child in ParameterGameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        /*
         * On Getting Values from ARgorithmCloudMenu for parametersInfo jObject
         * It takes some time to do so. So a timer is set withn the Coroutine for
         * the jobject to load all the values
        */
        SettingUpParametersInfo();
    }

    private void FixedUpdate()
    {
        if(panelListHeight>0f && !panelFlag)
        {
            RectTransform panelRT = ParameterGameObject.transform.GetComponent<RectTransform>();
            //panelRT.sizeDelta = new Vector2(0, this.panelListHeight);
            panelRT.offsetMin = new Vector2(0, -this.panelListHeight);
            panelFlag = true;
        }
        List<string> variables = new List<string>(objectParameter.Keys);
        foreach (var variable in variables)
        {
            if (!customParameters.ContainsKey(variable))
            {
                SubmitParametersButton.interactable = false;
                break;
            }
            SubmitParametersButton.interactable = true;
        }


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
                    SetupVariableParameters<int>(variable, description);
                    break;
                case "FLOAT":
                    SetupVariableParameters<float>(variable, description);
                    break;
                case "STRING":
                    SetupVariableParameters<string>(variable, description);
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
                    string matrixItemType = objectParameter[variable]["item-type"].ToString();
                    if (matrixItemType == "INT")
                        SetupMatrixParameters<int>(variable, description);
                    break;
            }
        }
    }
    private void SetupVariableParameters<T>(string variable,string description)
    {
        Dictionary<string, int> children = new Dictionary<string, int>(){
            {"VariableName",0 },
            {"Label",1 },
            {"StringInputField",2 },
            {"Description",3 },
            {"AlertBox",4}
        };
        /*
        * Instantiates SingleParameterPrefab
        */
        GameObject variableParameter = Instantiate(Resources.Load("SingleParameterPrefab") as GameObject);
        variableParameter.transform.parent = ParameterGameObject.transform;
        variableParameter.transform.localScale = new Vector3(1, 1, 1);
        /*
        * Child holds the text component within SingleParameterPrefab and sets variable name
        */
        var child = variableParameter.transform.GetChild(children["VariableName"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(variable);
        /*
        * Adding Labels before inputs
        */
        string type = typeof(T).Name;
        if (type == "Int32" || type == "Int64" || type == "Int16")
        {
            type = "INTEGER";
        }
        else if(type == "Float64" || type == "single" || type == "double" || type == "decimal" || type == "Single" || type == "Double" || type == "Decimal")
        {
            type = "FLOAT";
        }
        else if(type == "string" || type == "String")
        {
            type = "STRING";
        }
        child = variableParameter.transform.GetChild(children["Label"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(string.Format("Enter the Value (of type {0}):",type));
        /*
        * Child holds the description text component and sets description
        */
        child = variableParameter.transform.GetChild(children["Description"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(description);
        /*
        * SingleInputField holds the variableinputfield component within SingleParameter Prefab
        * and onEdit completion stores its value
        */
        T value;
        InputField valueInputField = variableParameter.transform.GetChild(children["StringInputField"]).gameObject.GetComponent<InputField>();
        valueInputField.onValueChanged.AddListener(delegate
        {
            try
            {
                value = Parse<T>(valueInputField.text);
                child = variableParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
                customParameters[variable] = JToken.FromObject(value);
            }
            catch (ARgorithmException e)
            {
                child = variableParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
                customParameters.Remove(variable);
            }
            catch
            {
                child = variableParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Error in Input");
                customParameters.Remove(variable);
            }

        });

        RectTransform rt = variableParameter.transform.GetComponent<RectTransform>();
        this.panelListHeight += rt.rect.height + 50;

    }

    private void SetupArrayParameters<T>(string variable,string description)
    {
        Dictionary<string, int> children = new Dictionary<string, int>(){
            {"VariableName",0 },
            {"ArraySizeLabel",1 },
            {"ArraySizeInputField",2 },
            {"Label",3 },
            {"ArrayElementInputField",4},
            {"Description",5 },
            {"AlertBox",6 }
        };
        /*
        * Instantiates Array Parameter Prefab
        */
        GameObject arrayParameter = Instantiate(Resources.Load("ArrayParameterPrefab") as GameObject);
        arrayParameter.transform.parent = ParameterGameObject.transform;
        arrayParameter.transform.localScale = new Vector3(1, 1, 1);
        /*
        * Child holds the text component within Array Parameter Prefab and sets variable name
        */
        var child = arrayParameter.transform.GetChild(children["VariableName"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(variable);
        /*
        * Change Label component within Array Parameter Prefab and sets the Label
        */
        string type = typeof(T).Name;
        if (type == "Int32" || type == "Int64" || type == "Int16")
        {
            type = "INTEGER";
        }
        else if (type == "Float64" || type == "single" || type == "double" || type == "decimal" || type == "Single" || type == "Double" || type == "Decimal")
        {
            type = "FLOAT";
        }
        else if (type == "string" || type == "String")
        {
            type = "STRING";
        }
        child = arrayParameter.transform.GetChild(children["Label"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(string.Format("Enter the elements (of type {0}):",type));
        /*
        * Child holds the description text component and sets description
        */
        child = arrayParameter.transform.GetChild(children["Description"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(description);
        /*
        * sizeInputField holds the array size inputfield component within Array Parameter Prefab
        * and onEdit completion store in size variable
        */
        InputField sizeInputField = arrayParameter.transform.GetChild(children["ArraySizeInputField"]).gameObject.GetComponent<InputField>();
        int arraySize = 0;
        sizeInputField.onValueChanged.AddListener(delegate
        {
            try
            {
                arraySize = int.Parse(sizeInputField.text);
                if (arraySize <= 0)
                {
                    throw new ARgorithmException("Size cannot be lesser than or equal to Zero");
                }
                child = arrayParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
            }
            catch(ARgorithmException e)
            {
                child = arrayParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
                customParameters.Remove(variable);
            }
            catch 
            {
                child = arrayParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Invalid input in size field");
                customParameters.Remove(variable);
            }
        });
      
        /*
        * arrayElementsInputField holds the array elementsinputfield component within Array Parameter Prefab
        * and onEdit completion store in arrayElements variable
        */
        T[] arrayElements = new T[arraySize];

        InputField arrayElementsInputField = arrayParameter.transform.GetChild(children["ArrayElementInputField"]).gameObject.GetComponent<InputField>();
        arrayElementsInputField.onValueChanged.AddListener(delegate
        {
            try
            {
                T[] arrayInputs = System.Array.ConvertAll(arrayElementsInputField.text.Split(' '), Parse<T>);
                if(arrayInputs.Length != arraySize)
                {
                    throw new ARgorithmException("Array size doesn't match with input size");
                }
                arrayElements = arrayInputs;
                child = arrayParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
                customParameters[variable] = JArray.FromObject(arrayElements);
            }
            catch (ARgorithmException e)
            {
                child = arrayParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
                customParameters.Remove(variable);
            }
            catch
            {
                child = arrayParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Input Error in Array Elements");
                customParameters.Remove(variable);
            }
        });
        RectTransform rt = arrayParameter.transform.GetComponent<RectTransform>();
        this.panelListHeight += rt.rect.height + 50;
    }

    private void SetupMatrixParameters<T>(string variable,string description)
    {
        Dictionary<string, int> children = new Dictionary<string, int>(){
            {"VariableName",0 },
            {"RowInputField",1 },
            {"ColumnInputField",2 },
            {"Label",3 },
            {"MatrixElementInput", 4 },
            {"Description",5 },
            {"AlertBox",6 }
        };
        /*
        * Instantiates Matrix Parameter Prefab
        */
        GameObject matrixParameter = Instantiate(Resources.Load("MatrixParameterPrefab") as GameObject);
        matrixParameter.transform.parent = ParameterGameObject.transform;
        matrixParameter.transform.localScale = new Vector3(1, 1, 1);
        /*
        * Child holds the text component within Matrix Parameter Prefab and sets variable name
        */
        var child = matrixParameter.transform.GetChild(children["VariableName"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(variable+"\n"+"Enter the row and column sizes (of type INTEGER)");
        /*
        * Child holds the description text component and sets description
        */
        child = matrixParameter.transform.GetChild(children["Description"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(description);
        /*
        * child holds the Label component within Matrix Parameter Prefab and sets the Label
        */
        string type = typeof(T).Name;
        if (type == "Int32" || type == "Int64" || type == "Int16")
        {
            type = "INTEGER";
        }
        else if (type == "Float64" || type == "single" || type == "double" || type == "decimal" || type == "Single" || type == "Double" || type == "Decimal")
        {
            type = "FLOAT";
        }
        else if (type == "string" || type == "String")
        {
            type = "STRING";
        }
        child = matrixParameter.transform.GetChild(children["Label"]).gameObject;
        child.transform.GetComponent<TextMeshProUGUI>().SetText(string.Format("Enter the Elements (of type {0}):", type));
        /*
        * rowInputField holds the row size inputfield component within Matrix Parameter Prefab
        * and onEdit completion store in size 
        */
        InputField rowInputField = matrixParameter.transform.GetChild(children["RowInputField"]).gameObject.GetComponent<InputField>();
        int rowSize = 0;
        rowInputField.onValueChanged.AddListener(delegate
        {
            try
            {
                rowSize = int.Parse(rowInputField.text);
                if (rowSize <= 0)
                {
                    throw new ARgorithmException("Size of row cannot be lesser than or equal to Zero");
                }
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
            }
            catch(ARgorithmException e)
            {
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
                customParameters.Remove(variable);
            }
            catch
            {
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Row size has to be an INTEGER");
                customParameters.Remove(variable);
            }
        });
        /*
        * columnInputField holds the col size inputfield component within Matrix Parameter Prefab
        * and onEdit completion store in size
        */
        InputField colInputField = matrixParameter.transform.GetChild(children["ColumnInputField"]).gameObject.GetComponent<InputField>();
        int colSize = 0;
        rowInputField.onValueChanged.AddListener(delegate
        {
            try
            {
                colSize = int.Parse(colInputField.text);
                if (colSize <= 0)
                {
                    throw new ARgorithmException("Size of column cannot be lesser than or equal to Zero");
                }
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
            }
            catch (ARgorithmException e)
            {
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
                customParameters.Remove(variable);
            }
            catch
            {
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Column size has to be an INTEGER");
                customParameters.Remove(variable);
            }
        });
        /*
        * matrixElementsInputField holds the matrix elementsinputfield component within MAtrix Parameter Prefab
        * and onEdit completion store in matrixElements variable
        */
        T[] matrixElements = new T[rowSize*colSize];
        InputField matrixElementsInputField = matrixParameter.transform.GetChild(children["MatrixElementInput"]).gameObject.GetComponent<InputField>();
        matrixElementsInputField.onValueChanged.AddListener(delegate
        {
            try
            {
                T[] matrixInputs = System.Array.ConvertAll(matrixElementsInputField.text.Split(' '), Parse<T>);
                if(matrixInputs.Length != (rowSize * colSize))
                {
                    throw new ARgorithmException("Input Elements size doesn't match matrix size provided");
                }
                matrixElements = matrixInputs;
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("");
                customParameters[variable] = JArray.FromObject(matrixElements);
            }
            catch (ARgorithmException e)
            {
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText(e.Message);
                customParameters.Remove(variable);
            }
            catch
            {
                child = matrixParameter.transform.GetChild(children["AlertBox"]).gameObject;
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Input Error in Matrix Elements");
                customParameters.Remove(variable);
            }
        });

        RectTransform rt = matrixParameter.transform.GetComponent<RectTransform>();
        this.panelListHeight += rt.rect.height + 50;

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
        string parametersInStringFormat = JsonConvert.SerializeObject(customParameters, Formatting.Indented);
        JObject parameters = JObject.Parse(parametersInStringFormat);
        
        StartCoroutine(
            APIClient.Instance.run(
                new ExecutionRequest
                {
                    argorithmID = argorithmID,
                    parameters = parameters
                },
                (r) => callback(r, argorithmID)
             )
        );
        
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

        This function should send the states to the ARgorithm Parser which in turn send to ARStage
        */
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
