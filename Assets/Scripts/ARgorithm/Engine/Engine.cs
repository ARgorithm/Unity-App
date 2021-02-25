using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

using ARgorithm.Structure;
using ARgorithm.Models;
/*
The ARgorithm.Engine namespace will contain all the classes and objects that will be required for physical behaviour of the ARgorithm 
For example: Stage. 
Info:
    The statelog will be instantiated during event execution inside the Stage and not when the states are being parsed.
*/
namespace ARgorithm.Engine
{
    public class Stage{
        // value Generics to be changed to ContentType
        private readonly StageData stageData;
        private int index;
        private Dictionary<string, Vector3> idToPositionMap;
        public Stage(StageData stageData) 
        {
            this.stageData = stageData;
            this.index = -1;
            this.idToPositionMap = new Dictionary<string, Vector3>();
        }

        public void Next()
        {
            index++;
            State args = stageData.states[index];
            if (args.state_type != "comment")
            {
                /*ask user to set position of object if not set already*/
            }
            // GameObject placeholder
            // stageData.eventList[index](args , placeholder);
            
        }

        public void Prev()
        {
            State args = stageData.states[index];
            JObject stateDef = args.state_def;
            string id = (string) stateDef["id"];
            string funcType = args.state_type.Split('_').ToList()[1];
            BaseStructure currStructure = stageData.objectMap[id];
            currStructure.Undo(funcType);
            index--;
        }

    }
}
