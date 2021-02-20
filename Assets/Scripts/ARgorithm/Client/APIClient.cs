using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ARgorithm.Models;
using ARgorithm.Singleton;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace ARgorithm.Client
{
    public class APIClient : Singleton<APIClient>
    {
        /*
        APIclient is the main class handling ARgorithmServer API requests.
        Its datamembers are stored in PlayerPrefs 
        */
        private string serverEndpoint{
            // The server endpoint to which you have to connect to
            get { return PlayerPrefs.GetString("endpoint"); }
            set { PlayerPrefs.SetString("endpoint", value); }
        }
        private string auth{
            // ENABLED if auth is enabled at endpoint
            get { return PlayerPrefs.GetString("auth"); }
            set { PlayerPrefs.SetString("auth", value); }
        }
        private string token{
            // stored last used JWT token
            get { return PlayerPrefs.GetString("token"); }
            set { PlayerPrefs.SetString("token", value); }
        }

        public IEnumerator connect(string url, System.Action<ConnectionResponse> callback){
            /*
            Estabilishes connection with given endpoint and returns data regarding auth functionality at server

            The ConnectionResponse.status can have following values
             - FAILURE: Error has occured
             - AUTH: Connection has been estabilished and Authentication is required
             - SUCCESS: Connection has been estabilished and Authentication is not required

            */
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
                    ConnectionRawResponse response = JsonConvert.DeserializeObject<ConnectionRawResponse>(
                        webRequest.downloadHandler.text
                    );
                    serverEndpoint = url;
                    if (response.auth == "ENABLED")
                    {
                        callback(new ConnectionResponse {
                            status="AUTH"
                        });    
                    }else
                    {
                        callback(new ConnectionResponse {
                            status="SUCCESS"
                        });
                    }
                    auth = response.auth;
                }
            }
        }

        public IEnumerator create(Account user,System.Action<CreationResponse> callback){
            /*
            Creates user account

            The CreationResponse.status can have following values
             - FAILURE: Error has occured
             - SUCCESS: User account has been created
             - EXISTING: Email is already registered
             - NOT_ALLOWED: Authentication feature at endpoint is disabled, skip login

            */
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
            /*
            Logs in to user account and updates authorization token

            The LoginResponse.status can have following values
             - FAILURE: Error has occured
             - SUCCESS: Login successful
             - NOT_ALLOWED: Authentication feature at endpoint is disabled, skip login
             - NOT_FOUND: Email is not registered
             - INCORRECT_PASSWORD: Password is incorrect

            */
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
                            LoginRawResponse response = JsonConvert.DeserializeObject<LoginRawResponse>(
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
            }
        }

        public IEnumerator list(System.Action<ARgorithmCollection> callback){
            /*
            Returns list of ARgorithms

            returns empty list if server error or no argorithms on server
            */
            using(UnityWebRequest webRequest = UnityWebRequest.Get(serverEndpoint+"/argorithms/list")){
                yield return webRequest.SendWebRequest();
                if(webRequest.isNetworkError){
                    callback(new ARgorithmCollection{
                        items = {},
                    });
                }

                if(webRequest.isDone){
                    string value = "{\"items\":" + webRequest.downloadHandler.text + "}";
                    ARgorithmCollection response = JsonConvert.DeserializeObject<ARgorithmCollection>(value);
                    callback(response);
                }
            }
        }

        public IEnumerator verify(System.Action<LoginResponse> callback){
            /*
            Verifies whether stored JWT token is valid or not. 
            When connecting to endpoint, if token is valid then login can be skipped

            The LoginResponse.status can have following values
             - FAILURE: Error has occured
             - SUCCESS: Login successful
             - RESET: Will have to login again
            */
            WWWForm form = new WWWForm();
            using(UnityWebRequest webRequest = UnityWebRequest.Post(serverEndpoint+"/users/verify",form)){
                
                webRequest.SetRequestHeader("authorization","Bearer "+token);
                
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
                            callback(new LoginResponse{
                                status="SUCCESS"
                            });
                            break;
            
                        case 401: 
                            callback(new LoginResponse{
                                status="RESET"
                            }); 
                            break;

                        default:
                            callback(new LoginResponse{
                                status="FAILURE"
                            }); 
                            break;
                    }
                }
            }
        }

        public IEnumerator run(ExecutionRequest exec,System.Action<ExecutionResponse> callback){
            /*
            Sends execution request to server and gets the states generated on executing
            ARgorithm to given parameters

            The ExecutionResponse returned can have the following status:
            - FAILURE: Something went wrong, empty states
            - run_example: The states are generated on default parameters
            - run_parameters: The states are generated on given parameters
            */
            string body = JsonConvert.SerializeObject(exec);
            
            using(UnityWebRequest webRequest = new UnityWebRequest(serverEndpoint+"/argorithms/run","POST")){
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);
                webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("authorization","Bearer "+token);
                webRequest.SetRequestHeader("Content-Type","application/json");
                yield return webRequest.SendWebRequest();

                if(webRequest.isNetworkError){
                    callback(new ExecutionResponse{
                        status="FAILURE",
                        data={}
                    });
                }
                
                if (webRequest.isDone){
                    switch (webRequest.responseCode)
                    {
                        case 200:
                            ExecutionResponse res = JsonConvert.DeserializeObject<ExecutionResponse>(webRequest.downloadHandler.text);
                            callback(res);
                            break;

                        default:
                            callback(new ExecutionResponse{
                                status="FAILURE",
                                data={}
                            }); 
                            break;
                    }
                }
            }
        }

    }

}
