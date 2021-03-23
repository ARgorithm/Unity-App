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
        public StringStructure()
        {

        }
        public override void Operate(State state, GameObject gameObject)
        {
            base.Operate(state, gameObject);
        }

        public override void Undo(State state)
        {
            base.Undo(state);
        }
    }
}

