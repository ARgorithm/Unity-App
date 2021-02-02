using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APIClient.Models{
    
    public class ConnectionResponse{
        public string status;
    }

    public class Account{
        public string email;
        public string password;
    }

    public class CreationResponse{
        public string status;
    }

    public class LoginResponse{
        public string status;
        public string token;
    }

    public class ARgorithm{
        public string argorithmID;
        public string maintainer;
        public string description;
    }

}
