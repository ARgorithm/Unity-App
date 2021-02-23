using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ARgorithm.Singleton;
using ARgorithm.Structure;
using ARgorithm.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
/*
The ARgorithm.Utils namespace will contain all the utility classes and objects that will be required for ARgorithm execution
For example: Parser. Other classes that could be implemented in this namespace include
 - The Eventlist which would be a wrapper around a list of `ARgorithm.Structure.ARgorithmEvent`
 - The ObjectMap which will be a string(id) to object mapping
 - The StageData which will contain instances of List<State>,Eventlist and ObjectMap. This class will contain all the data needed
 rendering purpose

Info:
    The statelog will be instantiated during event execution inside the Stage and not when the states are being parsed.
*/

namespace ARgorithm.Utils
{
    public class Eventlist : List<ARgorithmEvent>{
        /*
        Eventlist extends a List of ARgorithmEvent
        we can add any extra functionality we want from Eventlist here
        */
    }

    public class ObjectMap : Dictionary<string,BaseStructure>{
        /*
        ObjectMap extends a Map mapping between id and BaseStructure
        */
        public void Add(string id,string struct_type){
            /*
            Method overloading to add structures to map using the structure type which we derive from the state_type
            */
            try{
                switch (struct_type)
                {
                    case "array":
                        this.Add(id,new ArrayStructure());
                        break;
                    default:
                        this.Add(id,new BaseStructure());
                        break;
                }
            }
            catch (ArgumentException){}
        }
    }

    public class StageData{
        /*
        StageData contains the output of parsing
        */
        public Eventlist ev;
        public ObjectMap obmp;
        public List<State> states;
        public int size;
    }

    public class Parser : Singleton<Parser>{
        /*
        The Parser class is the class that will be used to extract data from the `ARgorithm.Models.ExecutionResponse`
        This will return the Stage with the Eventlist, StateLog and ObjectMap
        */
        public StageData Parse( List<State> states){
            /*
            parses the List<State> states to return a StageData object
            */

            ObjectMap objectmap = new ObjectMap();
            Eventlist eventlist = new Eventlist();

            foreach (State state in states)
            {
                if (state.state_type != "comment")
                {
                    JObject state_def = state.state_def;
                    string id =(string) state_def["id"];
                    string struct_type = state.state_type.Split('_').ToList()[0];
                    string func_type = state.state_type.Split('_').ToList()[1];
                    objectmap.Add(id,struct_type);
                    eventlist.Add(objectmap[id].getEvent(func_type));
                }
                else{
                    eventlist.Add(BaseStructure.comment);
                }
            }

            StageData sd = new StageData{
                ev=eventlist,
                obmp=objectmap,
                states=states,
                size=states.Count
            };

            return sd;
        }
    }
}