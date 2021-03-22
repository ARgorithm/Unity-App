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
    public class VariableStructure : BaseStructure{
        /*
        VariableStructure extends BaseStructure to handle the `ARgorithmToolkit.Utils.Variable`.
        This will also initialize and control the physical variable gameobject that will be shown to user
        */
        public string name = "";
        private ContentType value;
        private VariableAnimator animator;
        private GameObject structure;

        public VariableStructure()
        {
            structure = new GameObject("VariableStructure");
            this.animator = structure.AddComponent(typeof(VariableAnimator)) as VariableAnimator;
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
                    this.Declare(state, placeholder);
                    break;
                case "iter":
                    this.Highlight(state);
                    break;
                default:
                    throw new UnsupportedStateException(String.Format("variable_{0} state type is not supported", funcType));
            }
        }
        private void Declare(State state, GameObject placeholder)
        {
            // Creates Array GameObjects
            this.rendered = true;
            this.name = (string)state.state_def["variable_name"];
            JToken jt = (JToken)state.state_def["value"];
            this.value = new ContentType(jt);
            animator.Declare(this.value, placeholder);
        }

        private void Highlight(State state)
        {
            JToken value;
            if(state.state_def.TryGetValue("value",out value))
            {
                this.value = new ContentType(value);
            }
            animator.Highlight(this.value);
        }
        public override void Undo(State state)
        {
            string funcType = state.state_type.Split('_').ToList()[1];
            switch (funcType)
            {
                case "declare":
                    break;
                case "highlight":
                    JToken lastValue;
                    if (state.state_def.TryGetValue("last_value", out lastValue))
                    {
                        this.value = new ContentType(lastValue);
                        this.animator.Set(this.value);
                    }
                    break;
                default:
                    break;
            }
        }

    }
    
}

