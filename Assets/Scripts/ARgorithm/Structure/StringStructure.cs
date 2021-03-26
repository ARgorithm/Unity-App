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
    public class StringStructure : BaseStructure
    {
        public string name = "";
        private StringAnimator animator;
        private GameObject structure;
        public StringStructure()
        {
            this.structure = new GameObject("StringStructure");
            this.animator = structure.AddComponent(typeof(StringAnimator)) as StringAnimator;
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
                case "append":
                    this.Append(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("string_{0} state type is not supported", funcType));
            }
        } 

        private void Declare(State state, GameObject placeholder)
        {
            this.rendered = true;
            this.name = (string)state.state_def["variable_name"];
            string body = (string)state.state_def["body"];
            animator.Declare(name, body, placeholder);
        }

        private void Iter(State state)
        {
            int index = (int)state.state_def["index"];
            animator.Iter(index);
        }

        private void Append(State state)
        {
            string s = (string)state.state_def["element"];
            animator.Append(s);
        }
        public override void Undo(State state)
        {
            base.Undo(state);
        }
    }
}

