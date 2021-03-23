using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ARgorithm.Models;
using ARgorithm.Animations;
using ARgorithm.Structure.Typing;

namespace ARgorithm.Structure
{
    public class ArrayStructure : BaseStructure
    {
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
        
        public override void Operate(State state, GameObject placeholder)
        {
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

        public override void Undo(State state)
        {
            // Called to undo a change enforced by `state`
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
                        List<int> _index = NDimensionalArray.ToListIndex(index);
                        this.animator.Set(_index , this.body[index]);
                    }
                    break;
                case "compare":
                    break;
                case "swap":
                    JToken index1 = state.state_def["index1"];
                    JToken index2 = state.state_def["index2"];
                    ContentType temp = this.body[index1];
                    this.body[index1] = this.body[index2];
                    this.body[index2] = temp;
                    List<int> _index1 = NDimensionalArray.ToListIndex(index1);
                    List<int> _index2 = NDimensionalArray.ToListIndex(index2);
                    this.animator.Set(_index1 , this.body[index1]);
                    this.animator.Set(_index2 , this.body[index2]);
                    break;
                default:
                    break;
            }
        }

        private void Declare(State state, GameObject placeholder)
        {
            // Creates Array GameObjects
            this.rendered = true;
            this.name = (string) state.state_def["variable_name"];
            JArray jt = (JArray) state.state_def["body"];
            this.body = new NDimensionalArray(jt);
            animator.Declare(this.name, body, placeholder);
        }
        private void Iter(State state)
        {
            // Highlight certain index of Array and might update value
            JToken index = state.state_def["index"];
            JToken value;
            if (state.state_def.TryGetValue("value",out value))
            {
                this.body[index] = new ContentType(value);    
            }
            List<int> _index = NDimensionalArray.ToListIndex(index);
            animator.Iter(_index, this.body[index]);
        }

        private void Compare(State state)
        {
            // Highlights two values in array to indicate comparision operation
            JToken index1 = state.state_def["index1"];
            JToken index2 = state.state_def["index2"];
            List<int> _index1 = NDimensionalArray.ToListIndex(index1);
            List<int> _index2 = NDimensionalArray.ToListIndex(index2);
            animator.Compare(_index1, _index2);
        }

        private void Swap(State state)
        {
            // Swaps value stored at indexes
            JToken index1 = state.state_def["index1"];
            JToken index2 = state.state_def["index2"];
            ContentType temp = this.body[index1];
            this.body[index1] = this.body[index2];
            this.body[index2] = temp;
            List<int> _index1 = NDimensionalArray.ToListIndex(index1);
            List<int> _index2 = NDimensionalArray.ToListIndex(index2);
            animator.Swap(_index1, _index2);
        }
    }
}