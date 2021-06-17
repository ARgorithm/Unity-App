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
    public class MapStructure : BaseStructure
    {
        string name = "";
        GameObject structure;
        MapAnimator animator;
        public MapStructure()
        {
            structure = new GameObject("MapStructure");
            animator = structure.AddComponent(typeof(MapAnimator)) as MapAnimator;
        }
        public override void Operate(State state, GameObject placeholder)
        {
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    this.Declare(state, placeholder);
                    break;
                case "get":
                    this.Get(state);
                    break;
                case "set":
                    this.Set(state);
                    break;
                case "remove":
                    this.Remove(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("map_{0} state type is not supported", funcType));
            }
        }

        private void Declare(State state, GameObject placeholder)
        {
            this.rendered = true;
            this.name = (string)state.state_def["variable_name"];
            Dictionary<JToken, JToken> dict = JsonConvert.DeserializeObject<Dictionary<JToken, JToken>>((string)state.state_def["body"]);
            List<ContentType> key = new List<ContentType>();
            List<ContentType> value = new List<ContentType>();
            foreach (JToken x in dict.Keys)
                key.Add(new ContentType(x));
            foreach (JToken x in dict.Values)
                value.Add(new ContentType(x));
            animator.Declare(this.name, key, value, placeholder);
        }

        private void Get(State state)
        {
            ContentType key = new ContentType((JToken)state.state_def["key"]);
            animator.Get(key);
        }

        private void Set(State state)
        {
            ContentType key = new ContentType((JToken)state.state_def["key"]);
            ContentType value = new ContentType((JToken)state.state_def["value"]);
            animator.Set(key, value);
        }

        private void Remove(State state)
        {
            ContentType key = new ContentType((JToken)state.state_def["key"]);
            animator.Remove(key);
        }

        public override void Undo(State state)
        {
            base.Undo(state);
        }
    }
}
