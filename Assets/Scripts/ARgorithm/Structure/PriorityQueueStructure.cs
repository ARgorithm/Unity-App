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
    public class PriorityQueueStructure : BaseStructure
    {
        string name = "";
        GameObject structure;
        PriorityQueueAnimator animator;
        public PriorityQueueStructure()
        {
            structure = new GameObject("PriorityQueueStructure");
            animator = structure.AddComponent(typeof(PriorityQueueAnimator)) as PriorityQueueAnimator;
        }
        public override void Operate(State state, GameObject placeholder)
        {
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    this.Declare(state, placeholder);
                    break;
                case "poll":
                    this.Poll(state);
                    break;
                case "offer":
                    this.Offer(state);
                    break;
                case "peek":
                    this.Peek(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("priorityqueue_{0} state type is not supported", funcType));
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

        private void Poll(State state) 
        {
            animator.Poll();    
        }

        private void Offer(State state) 
        {
            ContentType element = new ContentType((JToken)state.state_def["element"]);
            animator.Offer(element);
        }

        private void Peek(State state)
        {
            animator.Peek();
        }

        public override void Undo(State state)
        {
            base.Undo(state);
        }
    }
}
