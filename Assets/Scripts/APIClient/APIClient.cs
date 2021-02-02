using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using APIClient.Models;
using APIClient.Singleton;

namespace APIClient
{

    public class APIClient : Singleton<APIClient>
    {
        private string serverEndpoint = "https://argorithm.el.r.appspot.com";

        // public IEnumerator connect(string url, System.Action<ConnectionResponse> callback){}

        // public IEnumerator create(Account user,System.Action<CreationResponse> callback){}

        // public IEnumerator login(Account user,System.Action<AuthResponse> callback){}

        // public IEnumerator list(System.Action<List<ARgorithm>> callback){}

    }

}
