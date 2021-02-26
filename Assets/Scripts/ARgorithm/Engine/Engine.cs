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
        private GameObject placeHolder;
        public Stage(StageData stageData, GameObject initialPlaceHolder) 
        {
            this.stageData = stageData;
            this.index = -1;
            this.idToPositionMap = new Dictionary<string, Vector3>();
            this.placeHolder = initialPlaceHolder;
        }

        public void Next()
        {
            index++;
            if (index >= stageData.size)
                return;
            State args = stageData.states[index];
            Debug.Log(args.state_type);
            if (args.state_type != "comment")
            {
                /*ask user to set position of object if not set already*/
            }
            string id = (string)args.state_def["id"];
            string funcType = args.state_type.Split('_').ToList()[1];
            if (stageData.objectMap[id].rendered && funcType=="declare")
            {
                placeHolder.SetActive(true);
                return;
            }
                
            stageData.eventList[index](args , this.placeHolder);
            
        }

        public void Prev()
        {
            
            State args = stageData.states[index];
            if (args.state_type == "comments")
            {
                index--;
                return;
            }
            JObject stateDef = args.state_def;
            string funcType = args.state_type.Split('_').ToList()[1];
            if (funcType == "declare")
            {
                placeHolder.SetActive(false);
            }
            string id = (string) stateDef["id"];
            BaseStructure currStructure = stageData.objectMap[id];
            currStructure.Undo(args);
            index--;
        }

    }
}
