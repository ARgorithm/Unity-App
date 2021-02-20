using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace ARgorithm.Models{
    /*
    The models namespace consists of JSON serializable schemas 
    */

    [Serializable]
    public class Response{
        /*
        The standard API response class. Returns string status
        */
        public string status;
    }

    [Serializable]
    public class ConnectionRawResponse{
        /*
        Used to parse the response to connect request
        */
        public string auth;
    }

    [Serializable]
    public class ConnectionResponse:Response{
        /*
        The standard API response class for connect response
        */
    }

    [Serializable]
    public class Account{
        /*
        The model for user account
        */
        public string email;
        public string password;
    }

    [Serializable]
    public class CreationResponse:Response{
        /*
        The standard API response class for connect response
        */
    }

    [Serializable]
    public class LoginRawResponse{
        /*
        Schema used to parse access token from login api response
        */
        public string access_token;
        public string token_type;
    }

    [Serializable]
    public class LoginResponse:Response{
        /*
        The standard API response class for login response
        */
    }

    [Serializable]
    public class ARgorithmModel{
        /*
        The Model class for an ARgorithm
        */
        public string argorithmID;
        public string maintainer;
        public string description;
        public string function;
        public string filename;
        public JObject parameters;
        public JObject example;
    }

    [Serializable]
    public class ARgorithmCollection{
        /*
        The Model class for List of ARgorithms
        */
        public ARgorithmModel[] items;
    }

    [Serializable]
    public class ExecutionRequest{
        /*
        The argorithm/run API request model to get states based on parameters
        */
        public string argorithmID;
        public JObject parameters;
    }

    [Serializable]
    public class State{
        /*
        The model for ARgorithmState
        */
        public string state_type;
        public JObject state_def;
        public string comments;
    }

    [Serializable]
    public class ExecutionResponse{
        /*
        The argorithm/run API response class with status and states
        */
        
        public string status;
        public List<State> data;
    }

}
