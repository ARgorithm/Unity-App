using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ARgorithmAPI;
using ARgorithmAPI.Models;

public class MainMenu : MonoBehaviour
{
    public Button ConnectToCloudButton;
    public Button ConnectToLocalServerButton;
    public InputField ServerEndpointInput;

    //Fucntion to verify in case we get AUTH as a response from connect
    //The LoginResponse.status can have following values
    //FAILURE: Error has occured
    //SUCCESS: Login successful
    //RESET: Will have to login again

    void verify()
    {
        StartCoroutine(
            APIClient.Instance.verify(
                (r) => callback(r)
            )
        );
    }

    void callback(LoginResponse c)
    {
        Debug.Log(c.status);
        switch (c.status)
        {
            case "SUCCESS":
                {
                    //Move to ARgorithm Cloud Menu
                    break;
                }
            default:
                {
                    //Move to Login Menu
                    break;
                }
        }
    }

    //Function to create connect to ARgorithm server
    //The ConnectionResponse.status can have following values
    //FAILURE: Error has occured
    //AUTH: Connection has been estabilished and Authentication is required
    //SUCCESS: Connection has been estabilished and Authentication is not required

    public void connect(string uri = "https://argorithm.el.r.appspot.com")
    {
        StartCoroutine(
            APIClient.Instance.connect(
                uri,
                (r) => callback(r)
            )
        );
    }

    void callback(ConnectionResponse c)
    {
        Debug.Log(c.status);
        switch (c.status)
        {
            //Coroutine APIclient.Intance.verify
            case "AUTH":
                {
                    Debug.Log("AUTH switch");
                    verify();
                    break;
                }
            case "SUCCESS":
                {
                    //Move to ARgorithm Cloud Menu
                    break;
                }
            default:
                {
                    Debug.Log("Failure");
                    //Alert the user for Error!!
                    break;
                }
            
        }
    }

    void Start()
    {
        //Should check whether the user is already logged into the App and or not.
        //If logged in show ARgorithm Cloud Menu 
        //else show Login Menu
        ConnectToCloudButton.onClick.AddListener(() =>
        {
            Debug.Log("Connect to Cloud Server Button");
            connect();
        });

        //Connects to Locally generated Server endpoint
        //Takes "Enter server endpoint" inputfield value at 
        ConnectToLocalServerButton.onClick.AddListener(() =>
        {
            Debug.Log("Connect to Local Server Button");
            Debug.Log(ServerEndpointInput.text);
            connect(ServerEndpointInput.text);
        });
    }
}
