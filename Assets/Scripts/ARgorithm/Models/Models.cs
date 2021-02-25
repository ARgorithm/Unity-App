using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ARgorithm.Structure;
using System.Linq;
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

    public class Eventlist : List<ARgorithmEvent>
    {
        /*
        Eventlist extends a List of ARgorithmEvent
        we can add any extra functionality we want from Eventlist here
        */
    }

    public class ObjectMap : Dictionary<string, BaseStructure>
    {
        /*
        ObjectMap extends a Map mapping between id and BaseStructure
        */
        public void Add(string id, string struct_type)
        {
            /*
            Method overloading to add structures to map using the structure type which we derive from the state_type
            */
            try
            {
                if (!this.ContainsKey(id))
                {
                    switch (struct_type)
                    {
                        case "array":
                            this.Add(id, new ArrayStructure());
                            break;
                        default:
                            this.Add(id, new BaseStructure());
                            break;
                    }
                }
            }
            catch (ArgumentException) { }
        }
    }
    public class StageData
    {
        /*
        StageData contains the output of parsing
        */
        public Eventlist eventList;
        public ObjectMap objectMap;
        public List<State> states;
        public int size;
    }



    [Serializable]
    public class ExecutionResponse{
        /*
        The argorithm/run API response class with status and states
        */
        
        public string status;
        public List<State> data;

        public StageData convertStageData()
        {
            ObjectMap objectmap = new ObjectMap();
            Eventlist eventlist = new Eventlist();

            foreach (State state in this.data)
            {
                if (state.state_type != "comment")
                {
                    JObject state_def = state.state_def;
                    string id = (string)state_def["id"];
                    string structType = state.state_type.Split('_').ToList()[0];
                    objectmap.Add(id, structType);
                    eventlist.Add(objectmap[id].Operate);
                }
                else
                {
                    eventlist.Add(BaseStructure.Comment);
                }
            }

            StageData sd = new StageData
            {
                eventList = eventlist,
                objectMap = objectmap,
                states = this.data,
                size = this.data.Count
            };

            return sd;
        }
    }

}
