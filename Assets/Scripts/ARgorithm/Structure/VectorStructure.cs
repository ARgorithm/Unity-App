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
    public class VectorStructure : BaseStructure
    {
        public string name = "";
        private VectorAnimator animator;
        private GameObject structure;
        public VectorStructure()
        {
            structure = new GameObject("VectorStructure");
            this.animator = structure.AddComponent(typeof(VectorAnimator)) as VectorAnimator;
        }
        public override void Operate(State state, GameObject placeholder)
        {
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    this.Declare(state, placeholder);
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
                case "remove":
                    this.Remove(state);
                    break;
                case "insert":
                    this.Insert(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("vector_{0} state type is not supported", funcType));
            }
        }

        private void Declare(State state, GameObject placeholder)
        {
            this.rendered = true;
            this.name = (string)state.state_def["variable_name"];
            JArray jt = (JArray)state.state_def["body"];
            List<ContentType> body = new List<ContentType>();
            foreach (JToken x in jt)
                body.Add(new ContentType(x));
            animator.Declare(this.name, body, placeholder);
        }

        private void Iter(State state)
        {
            int index = (int)state.state_def["index"];
            JToken value;
            if(state.state_def.TryGetValue("value", out value))
                animator.Iter(index, new ContentType(value));
        }

        private void Compare(State state)
        {
            int index1 = (int)state.state_def["index1"];
            int index2 = (int)state.state_def["index2"];
            animator.Compare(index1, index2);
        }

        private void Swap(State state)
        {
            int index1 = (int)state.state_def["index1"];
            int index2 = (int)state.state_def["index2"];
            animator.Swap(index1, index2);
        }

        private void Insert(State state)
        {
            int index = (int)state.state_def["index"];
            ContentType element = new ContentType((JToken)state.state_def["element"]);
            animator.Insert(index, element);
        }

        private void Remove(State state)
        {
            int index = (int)state.state_def["index"];
            ContentType element = new ContentType((JToken)state.state_def["element"]);
            animator.Remove(index);
        }

        public override void Undo(State state)
        {
            base.Undo(state);
        }
    }
}
