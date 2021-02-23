using ARgorithm.Utils;
using System.Collections.Generic;
using ARgorithm.Structure;
using UnityEngine;
using ARgorithm.Models;
using Newtonsoft.Json.Linq;
/*
The ARgorithm.Engine namespace will contain all the classes and objects that will be required for physical behaviour of the ARgorithm 
For example: Stage. 
Info:
    The statelog will be instantiated during event execution inside the Stage and not when the states are being parsed.
*/
namespace ARgorithm.Engine
{
    public class Stage{
        private Dictionary<BaseStructure, JToken> stateLog;
        // value Generics to be changed to ContentType
        private readonly StageData stageData;
        private int index;
        
        public Stage(StageData stageData) 
        {
            this.stageData = stageData;
            this.index = -1;
        }

        public void Next()
        {
            index++;
            // stageData.eventList -> current index call method
            // stateData.states
            State args = stageData.states[index];
            stageData.eventList[index](args);
            if (args.state_type != "comment")
            {
                JObject state_def = args.state_def;
                string id = (string)state_def["id"];
                BaseStructure objectRef = stageData.objectMap[id];
                // Body of structure is unclear.
                stateLog[objectRef] = "";
            }
        }

        public void Prev()
        {
            State args = stageData.states[index];
            //required stateLog implementation;
            index--;
        }

        private void Place(GameObject obj)
        {
            // GameObjects required.

        }

    }
}
