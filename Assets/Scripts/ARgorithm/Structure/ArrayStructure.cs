using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using ARgorithm.Models;

namespace ARgorithm.Structure
{
    public class ArrayStructure : BaseStructure{
        /*
        ArrayStructure extends BaseStructure to handle the `ARgorithmToolkit.Array`.
        This will also initialize and control the physical array gameobject that will be shown to user

        Warning:
            Currently this class is just an empty class to demonstrate the eventlist execution and uniquely handle states
        */
        public string name = "";
        public ArrayStructure(){
            // constructor
        }

        public override ARgorithmEvent getEvent(string func_type){
            /*
            override the virtual function of parent class to handle array states
            */
            switch (func_type)
            {
                case "declare":
                    return new ARgorithmEvent(this.declare);
                case "iter":
                    return new ARgorithmEvent(this.iter);
                case "compare":
                    return new ARgorithmEvent(this.compare);
                case "swap":
                    return new ARgorithmEvent(this.swap);
                default:
                    break;
            }
            return new ARgorithmEvent(this.error);
        }

        public void declare(State state){
            this.rendered = true;
            this.name = (string) state.state_def["variable_name"];
            Debug.Log(this.name+" declared");
        }
        public void iter(State state){
            Debug.Log(this.name+" iter");
        }

        public void compare(State state){
            Debug.Log(this.name+" compare");
        }

        public void swap(State state){
            Debug.Log(this.name+" swap");
        }

    }
}