using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARgorithmAPI.Models{
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
        The standard API response class for connect response
        */
    }

    [Serializable]
    public class ARgorithm{
        /*
        The Model class for an ARgorithm
        */
        public string argorithmID;
        public string maintainer;
        public string description;
        public string function;
        public string filename;
        public object parameters;
        public object example;
    }

    [Serializable]
    public class ARgorithmCollection{
        /*
        The Model class for List of ARgorithms
        */
        public ARgorithm[] items;
    }

}
