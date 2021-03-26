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
    public class StackStructure : BaseStructure
    {
        public string name = "";
        private StackAnimator animator;
        private GameObject structure;
        public StackStructure()
        {
            this.structure = new GameObject("StringStructure");
            this.animator = structure.AddComponent(typeof(StackAnimator)) as StackAnimator;
        }
        public override void Operate(State state, GameObject placeholder)
        {
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    this.Declare(state, placeholder);
                    break;
                case "push":
                    this.Push(state);
                    break;
                case "pop":
                    this.Pop(state);
                    break;
                case "top":
                    this.Top(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("stack_{0} state type is not supported", funcType));
            }
        }

        private void Declare(State state, GameObject placeholder)
        {
            this.rendered = true;
            this.name = (string)state.state_def["variable_name"];
            JArray jt = (JArray)state.state_def["body"];
            Stack<ContentType>body = new Stack<ContentType>();
            foreach (JToken x in jt)
                body.Push(new ContentType(x));
            animator.Declare(this.name, body.ToList(), placeholder);
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

        private void Top(State state)
        {
            animator.Top();
        }
        public override void Undo(State state)
        {
            base.Undo(state);
        }
    }
}
