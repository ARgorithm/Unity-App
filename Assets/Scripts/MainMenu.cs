using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ARgorithm.Client;
using ARgorithm.Models;

public class MainMenu : MonoBehaviour
{
    public Button ConnectToCloudButton;
    public Button ConnectToLocalServerButton;
    public InputField ServerEndpointInput;

    public GameObject ArgorithmCloudMenu;
    public GameObject Mainmenu;
    public GameObject LoginMenu;

    public GameObject AlertBoxMain;

    //Alert Box adds UI text ryt below Connect to local server to Alert the users 
    //Or give the user some kind of information like, invalid password, username, 
    //unable to connect to server etc. Remember to reset the Alert box to empty string.
    void AlertMain(string text)
    {
        AlertBoxMain.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    //Function to verify in case we get AUTH as a response from connect function
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
        switch (c.status)
        {
            case "SUCCESS":
                {
                    //Move to ARgorithm Cloud Menu
                    Mainmenu.SetActive(false);
                    ArgorithmCloudMenu.SetActive(true);
                    break;
                }
            default:
                {
                    //Move to Login Menu
                    Mainmenu.SetActive(false);
                    LoginMenu.SetActive(true);
                    break;
                }
        }
    }

    //Function to create session connection to ARgorithm server
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
        switch (c.status)
        {
            case "AUTH":
                {
                    verify();
                    break;
                }
            case "SUCCESS":
                {
                    //Move to ARgorithm Cloud Menu
                    Mainmenu.SetActive(false);
                    ArgorithmCloudMenu.SetActive(true);
                    break;
                }
            default:
                {
                    //Alert the user for Error!!
                    AlertMain("Connection Error");
                    break;
                }
            
        }
    }

    

    void Start()
    {
        //Makes Connect to local server button not interactable
        ConnectToLocalServerButton.interactable = false;

        ConnectToCloudButton.onClick.AddListener(() =>
        {
            connect();
        });

        //To verfiy the server endpoint value that was entered
        ServerEndpointInput.onValueChanged.AddListener(delegate
        {
            ConnectToLocalServerButton.interactable = true;
        });

        //Connects to Locally generated Server endpoint
        //Takes "Enter server endpoint" as input value
        ConnectToLocalServerButton.onClick.AddListener(() =>
        {
            connect(ServerEndpointInput.text);
        });
    }
    
}
