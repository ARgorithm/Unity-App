using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARgorithmAPI.Models{
    
    [Serializable]
    public class Response{
        public string status;
    }

    [Serializable]
    public class ConnectionRawResponse{
        public string auth;
    }

    [Serializable]
    public class ConnectionResponse:Response{}

    [Serializable]
    public class Account{
        public string email;
        public string password;
    }

    [Serializable]
    public class CreationResponse:Response{}

    [Serializable]
    public class LoginRawResponse{
        public string access_token;
        public string token_type;
    }

    [Serializable]
    public class LoginResponse:Response{}

    [Serializable]
    public class ARgorithm{
        public string argorithmID;
        public string maintainer;
        public string description;
    }

}
