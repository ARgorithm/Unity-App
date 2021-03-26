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
    public class SetStructure : BaseStructure
    {
        string name = "";
        GameObject structure;
        SetAnimator animator;
        public SetStructure()
        {
            structure = new GameObject("SetStructure");
            this.animator = structure.AddComponent(typeof(SetAnimator)) as SetAnimator;
        }
        public override void Operate(State state, GameObject placeholder)
        {
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    this.Declare(state, placeholder);
                    break;
                case "add":
                    this.Add(state);
                    break;
                case "remove":
                    this.Remove(state);
                    break;
                case "find":
                    this.Find(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("set_{0} state type is not supported", funcType));
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

        private void Add(State state)
        {
            ContentType key = new ContentType((JToken)state.state_def["key"]);
            animator.Add(key);
        }

        private void Remove(State state)
        {
            ContentType key = new ContentType((JToken)state.state_def["key"]);
            animator.Remove(key);
        }

        private void Find(State state)
        {
            ContentType key = new ContentType((JToken)state.state_def["key"]);
            animator.Find(key);
        }

        public override void Undo(State state)
        {
            base.Undo(state);
        }
    }
}
