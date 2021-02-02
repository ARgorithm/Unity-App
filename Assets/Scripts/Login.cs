using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ARgorithmAPI;
using ARgorithmAPI.Models;

public class Login : MonoBehaviour
{
    public InputField NewEmailInput;
    public InputField NewPasswordInput;
    public InputField NewRePasswordInput;
    public Button CreateAccountButton;

    public GameObject AlertBox;

    public InputField ExistingEmailInput;
    public InputField ExistingPasswordInput;
    public Button LoginButton;

    

    //Function that use ARgorithm APIclient to create Account
    //The CreationResponse.status can have following values
    //FAILURE: Error has occured
    //SUCCESS: User account has been created
    //EXISTING: Email is already registered
    //NOT_ALLOWED: Authentication feature at endpoint is disabled, skip login
    public void createAccount(string email,string password)
    {
        Debug.Log(email + " " + password);
        StartCoroutine(
            APIClient.Instance.create(
                new Account
                {
                    email = email,
                    password = password
                },
                (r) => callback(r)
            )
        );
    }

    void callback(CreationResponse c)
    {
        Debug.Log(c.status);
        switch (c.status)
        {
            case "SUCCESS":
                {
                    //Account is created 
                    //Login the user with the credentials
                    break;
                }
            case "EXISTING":
                {
                    //Alert the user email is already registered 
                    //use login to login
                    break;
                }
            default :
                {
                    //Navigate to Main Menu
                    //Alert the user Try connecting to server again
                    break;
                }
        }
    }

    //Function that use ARgorithm APIclient to LOGIN to existing Account
    //The LoginResponse.status can have following values
    //FAILURE: Error has occured
    //SUCCESS: Login successful
    //NOT_ALLOWED: Authentication feature at endpoint is disabled, skip login
    //NOT_FOUND: Email is not registered
    //INCORRECT_PASSWORD: Password is incorrect
    public void login(string email, string password)
    {
        StartCoroutine(
            APIClient.Instance.login(
                new Account
                {
                    email = email,
                    password = password
                },
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
                    //Navigate to cloud Menu
                    break;
                }
            case "NOT_FOUND":
                {
                    //Alert Email was not found
                    break;
                }
            case "INCORRECT_PASSWORD":
                {
                    //Alert the user the password is incorrect
                    break;
                }
            default:
                {
                    //Navigate to Main Menu
                    //Alert the user Try connecting to server again
                    break;
                }

        }
    }


    // Start is called before the first frame update
    void Start()
    {
        CreateAccountButton.interactable = false;
        LoginButton.interactable = false;

        //Button that takes in New Email and Password sends an API call to 
        //add the user to AR cloud server (incomplete function) 
        CreateAccountButton.onClick.AddListener(() =>
        {
            if (NewPasswordInput.text == NewRePasswordInput.text)
            {
                //Debug.Log("Creating New Account Function");
                createAccount(NewEmailInput.text, NewPasswordInput.text);
            }

        });

        //Button that takes in Email and Password sends an API call to check 
        //whether they are registered users on AR cloud server (incomplete function) 
        LoginButton.onClick.AddListener(() =>
        {
            //Debug.Log("Existing Account Function");
            login(ExistingEmailInput.text, ExistingPasswordInput.text);
        });



        //To check whether the Re-entered Password is same as the entered Password in
        //Create account. The below functions also validates the password and email if they are 
        //in the right format whenever the values change in the inputfields.
        Regex passwordRegex = new Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,25}$");
        Regex emailRegex = new Regex("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$");

        NewEmailInput.onValueChanged.AddListener(delegate
        {
            Match emailMatch = emailRegex.Match(NewEmailInput.text);
            Match passwordMatch = passwordRegex.Match(NewPasswordInput.text);

            if (!emailMatch.Success)
            {
                Alert("Not a valid Email ID");
                CreateAccountButton.interactable = false;
            }
            else if (!passwordMatch.Success)
            {
                Alert("Your password must contain at least one digit number, Capital letter " +
                    "and one special character");
                CreateAccountButton.interactable = false;
            }
            else if (NewPasswordInput.text != NewRePasswordInput.text)
            {
                Alert("Passwords are not the same");
                CreateAccountButton.interactable = false;
            }
            else
            {
                Alert("");
                CreateAccountButton.interactable = true;

            }

        });

        NewPasswordInput.onValueChanged.AddListener(delegate
        {
            Match emailMatch = emailRegex.Match(NewEmailInput.text);
            Match passwordMatch = passwordRegex.Match(NewPasswordInput.text);

            if (!emailMatch.Success)
            {
                Alert("Not a valid Email ID");
                CreateAccountButton.interactable = false;
            }
            else if (!passwordMatch.Success)
            {
                Alert("Your password must contain at least one digit number, Capital letter " +
                    "and one special character");
                CreateAccountButton.interactable = false;
            }
            else if (NewPasswordInput.text != NewRePasswordInput.text)
            {
                Alert("Passwords are not the same");
                CreateAccountButton.interactable = false;
            }
            else
            {
                Alert("");
                CreateAccountButton.interactable = true;

            }
        });

        NewRePasswordInput.onValueChanged.AddListener(delegate
        {
            Match emailMatch = emailRegex.Match(NewEmailInput.text);
            Match passwordMatch = passwordRegex.Match(NewPasswordInput.text);

            if (!emailMatch.Success)
            {
                Alert("Not a valid Email ID");
                CreateAccountButton.interactable = false;
            }
            else if (!passwordMatch.Success)
            {
                Alert("Your Password should contain");
                CreateAccountButton.interactable = false;
            }
            else if (NewPasswordInput.text != NewRePasswordInput.text)
            {
                Alert("Passwords are not the same");
                CreateAccountButton.interactable = false;
            }
            else
            {
                Alert("");
                CreateAccountButton.interactable = true;

            }
        });

        ExistingEmailInput.onValueChanged.AddListener(delegate
        {
            if (ExistingEmailInput.text.Length > 0 && ExistingPasswordInput.text.Length > 0)
            {
                LoginButton.interactable = true;
            }
        });

        ExistingPasswordInput.onValueChanged.AddListener(delegate
        {
            if (ExistingEmailInput.text.Length > 0 && ExistingPasswordInput.text.Length > 0)
            {
                LoginButton.interactable = true;
            }
        });

        //Alert Box adds UI text ryt below Create Account Button to Alert the users 
        //Or give the user some kind of information like, invalid password, username, 
        //unable to connect to server etc. Remember to reset the Alert box to empty string.
        void Alert(string text)
        {
            AlertBox.GetComponent<TextMeshProUGUI>().SetText(text);
        }

    }

}