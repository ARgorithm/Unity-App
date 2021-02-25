using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using ARgorithm.Models;
/*
The ARgorithm.Structure stores all the data structure handler classes.
*/

namespace ARgorithm.Structure
{
    public delegate void ARgorithmEvent(State state , GameObject gameObject);
    /*
    The ARgorithmEvent delegate defines a function type for all events that would 
    need to be animated during ARgorithm rendering process
    */

    public class UnsupportedStateException : System.Exception
    {
        public new string Message = "Recieved State which is not supported";
        public new string HelpLink = "https://github.com/ARgorithm/Unity-App/issues";
        public UnsupportedStateException() : base() {}
        
        public UnsupportedStateException(string message) : base(message) {}
    }

    public class BaseStructure{
        /*
        The BaseStructure class will be the parent class to all data structures.
        Will contain common functions that will be needed for all data structures
        */

        public bool rendered = false;
        // As the ObjectMap is created. The rendered attribute is used to flag whether this object requires to be rendered yet or not

        public virtual void Operate(State state, GameObject gameObject){
            /*
            This function will get called by Stage and overriden by child classes
            */
            throw new UnsupportedStateException("BaseStructure methods are not allowed");
        }

        public virtual void Undo(string funcType){
            /*
            The function overwritten by child classes for structure undo operation
            */
            throw new UnsupportedStateException("BaseStructure methods are not allowed");
        }

        public static void Comment(State state, GameObject gameObject){
            /*
            Static method to show comment events
            */
        }

    }
}