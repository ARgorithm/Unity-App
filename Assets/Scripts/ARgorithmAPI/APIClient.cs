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
        private string token = null;

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
                        
                        case 405: callback(
                            new CreationResponse{
                                status="NOT_ALLOWED"
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

        public IEnumerator login(Account user,System.Action<LoginResponse> callback){
            WWWForm form = new WWWForm();
            form.AddField("username" , user.email);
            form.AddField("password" , user.password);
            using(UnityWebRequest webRequest = UnityWebRequest.Post(serverEndpoint+"/users/login",form)){
                yield return webRequest.SendWebRequest();
                
                if(webRequest.isNetworkError){
                    callback(new LoginResponse {
                        status="FAILURE"
                    });
                }

                if (webRequest.isDone){
                    switch (webRequest.responseCode)
                    {
                        case 200:
                            LoginRawResponse response = JsonUtility.FromJson<LoginRawResponse>(
                                webRequest.downloadHandler.text
                            );
                            token = response.access_token;
                            callback(new LoginResponse{
                                status="SUCCESS"
                            });
                            break;
                        
                        case 404: 
                            callback(new LoginResponse{
                                status="NOT_FOUND"
                            }); 
                            break;
                        
                        case 401: 
                            callback(new LoginResponse{
                                status="INCORRECT_PASSWORD"
                            }); 
                            break;
                        
                        case 405: 
                            callback(new LoginResponse{
                                status="NOT_ALLOWED"
                            }); 
                            break;
                        
                        default: 
                            callback(new LoginResponse{
                                status="FAILURE"
                            });break;
                    }
                }

                Debug.Log(token);
            }
        }

        // public IEnumerator list(System.Action<List<ARgorithm>> callback){}

    }

}
