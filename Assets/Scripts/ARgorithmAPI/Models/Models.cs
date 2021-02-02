using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARgorithmAPI.Models{
    
    [Serializable]
    public class ConnectionRawResponse{
        public string auth;
    }

    [Serializable]
    public class ConnectionResponse{
        public string status;
    }

    [Serializable]
    public class Account{
        public string email;
        public string password;
    }

    [Serializable]
    public class CreationResponse{
        public string status;
    }

    [Serializable]
    public class LoginResponse{
        public string status;
        public string token;
    }

    [Serializable]
    public class ARgorithm{
        public string argorithmID;
        public string maintainer;
        public string description;
    }

}
