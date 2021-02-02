using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ARgorithmAPI.Models;
using ARgorithmAPI.Singleton;

namespace ARgorithmAPI
{

    public class APIClient : Singleton<APIClient>
    {
        private string serverEndpoint = "https://argorithm.el.r.appspot.com";
        private bool auth = false;

        public IEnumerator connect(string url, System.Action<ConnectionResponse> callback){
            using(UnityWebRequest webRequest = UnityWebRequest.Get(url+"/argorithm"))
            {
                yield return webRequest.SendWebRequest();
                
                if(webRequest.isNetworkError || webRequest.isHttpError){
                    callback(new ConnectionResponse {
                        status="FAILURE"
                    });
                }
                
                if(webRequest.isDone)
                {
                    ConnectionRawResponse response = JsonUtility.FromJson<ConnectionRawResponse>(
                        webRequest.downloadHandler.text
                    );
                    serverEndpoint = url;
                    Debug.Log(serverEndpoint);
                    if (response.auth == "ENABLED")
                    {
                        auth = true;
                        callback(new ConnectionResponse {
                            status="AUTH"
                        });    
                    }else
                    {
                        auth = false;
                        callback(new ConnectionResponse {
                            status="SUCCESS"
                        });
                    }
                }
            }
        }

        public IEnumerator create(Account user,System.Action<CreationResponse> callback){
            WWWForm form = new WWWForm();
            form.AddField("username" , user.email);
            form.AddField("password" , user.password);
            using(UnityWebRequest webRequest = UnityWebRequest.Post(serverEndpoint+"/users/register",form)){
                
                yield return webRequest.SendWebRequest();
                if(webRequest.isNetworkError){
                    callback(new CreationResponse {
                        status="FAILED"
                    });
                }

                if(webRequest.isDone){
                    switch (webRequest.responseCode)
                    {
                        case 200: callback(new CreationResponse {
                                status="SUCCESS"
                            });
                            break;
                        case 409: callback(
                            new CreationResponse {
                                status="EXISTING"
                            });
                            break;
                        default:callback(
                            new CreationResponse {
                                status="FAILURE"
                            });
                            break;
                    }
                }
                
            }
        }

        // public IEnumerator login(Account user,System.Action<AuthResponse> callback){}

        // public IEnumerator list(System.Action<List<ARgorithm>> callback){}

    }

}
