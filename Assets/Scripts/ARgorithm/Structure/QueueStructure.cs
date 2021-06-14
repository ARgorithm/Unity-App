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
    public class QueueStructure : BaseStructure
    {
        string name = "";
        GameObject structure;
        QueueAnimator animator;

        public QueueStructure()
        {
            structure = new GameObject("QueueStructure");
            animator = structure.AddComponent(typeof(QueueAnimator)) as QueueAnimator;
        }
        public override void Operate(State state, GameObject placeholder)
        {
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    this.Declare(state, placeholder);
                    break;
                case "pop":
                    this.Pop(state);
                    break;
                case "push":
                    this.Push(state);
                    break;
                case "front":
                    this.Front(state);
                    break;
                case "back":
                    this.Back(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("queue_{0} state type is not supported", funcType));
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

        private void Push(State state)
        {
            ContentType element = new ContentType((JToken)state.state_def["element"]);
            animator.Push(element);
        }

        private void Pop(State state)
        {
            animator.Pop();
        }

        private void Front(State state)
        {
            animator.Front();
        }

        private void Back(State state)
        {
            animator.Back();
        }

        public override void Undo(State state)
        {
            // Called to undo a change enforced by `state`
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    break;
                case "push":
                    animator.PopLast();
                    break;
                case "pop":
                    ContentType element = new ContentType((JToken)state.state_def["element"]);
                    animator.PushFirst(element);
                    break;
                case "front":
                    break;
                case "back":
                    
                    break;
                default:
                    break;
            }
        }
    }
}
