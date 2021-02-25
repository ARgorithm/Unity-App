using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ARgorithm.Models;
using ARgorithm.Structure.Typing;

namespace ARgorithm.Structure
{
    public class ArrayStructure : BaseStructure{
        /*
        ArrayStructure extends BaseStructure to handle the `ARgorithmToolkit.Array`.
        This will also initialize and control the physical array gameobject that will be shown to user
        */
        
        public string name = "";
        private NDimensionalArray body;
        private ArrayAnimator animator;
        private GameObject structure;
        public ArrayStructure(){
            // constructor
            structure = new GameObject("ArrayStructure");
            this.animator = structure.AddComponent(typeof(ArrayAnimator)) as ArrayAnimator;
        }
        
        public override void Operate(State state, GameObject placeholder){
            /*
            override the virtual function of parent class to handle array states
            */
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    this.Declare(state,placeholder);
                    break;
                case "iter":
                    this.Iter(state);
                    break;
                case "compare":
                    this.Compare(state);
                    break;
                case "swap":
                    this.Swap(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("array_{0} state type is not supported" , funcType));
            }
            
        }

        public override void Undo(State state){
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    break;
                case "iter":        
                    JToken index = state.state_def["index"];
                    JToken lastValue;
                    if (state.state_def.TryGetValue("last_value",out lastValue))
                    {
                        this.body[index] = new ContentType(lastValue);    
                    }
                    break;
                case "compare":
                    break;
                case "swap":
                    break;
                default:
                    break;
            }
        }

        private void Declare(State state, GameObject placeholder){
            this.rendered = true;
            this.name = (string) state.state_def["variable_name"];
            JArray jt = (JArray) state.state_def["body"];
            this.body = new NDimensionalArray(jt);
            animator.Declare(body, placeholder);
            // Debug.Log(this.name+" declared");
        }
        private void Iter(State state){
            JToken index = state.state_def["index"];
            JToken value;
            if (state.state_def.TryGetValue("value",out value))
            {
                this.body[index] = new ContentType(value);    
            }
            List<int> _index1;
            if (index.Type == JTokenType.Integer)
            {
                _index1 = new List<int>();
                _index1.Add((int)index);
            }
            else
            {
                _index1 = ((JArray)index).ToObject<List<int>>();
            }
            animator.Iter(_index1, this.body[index]);
            // Debug.Log(this.name+" iter");
        }

        private void Compare(State state){
            JToken index1 = state.state_def["index1"];
            JToken index2 = state.state_def["index2"];
            List<int> _index1;
            List<int> _index2;
            if (index1.Type == JTokenType.Integer)
            {
                _index1 = new List<int>();
                _index1.Add((int)index1);
                _index2 = new List<int>();
                _index2.Add((int)index2);
            }
            else
            {
                _index1 = ((JArray)index1).ToObject<List<int>>();
                _index2 = ((JArray)index2).ToObject<List<int>>();
            }
            // Debug.Log(this.name+" compare");
            animator.Compare(_index1, _index2);
        }

        private void Swap(State state){
            JToken index1 = state.state_def["index1"];
            JToken index2 = state.state_def["index2"];
            ContentType temp = this.body[index1];
            this.body[index1] = this.body[index2];
            this.body[index2] = temp;
            // Debug.Log(this.name+" swap");
            List<int> _index1;
            List<int> _index2;
            if (index1.Type == JTokenType.Integer)
            {
                _index1 = new List<int>();
                _index1.Add((int)index1);
                _index2 = new List<int>();
                _index2.Add((int)index2);
            }
            else
            {
                _index1 = ((JArray)index1).ToObject<List<int>>();
                _index2 = ((JArray)index2).ToObject<List<int>>();
            }
            animator.Swap(_index1, _index2);
        }
    }
}