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
    public delegate void ARgorithmEvent(State state);
    /*
    The ARgorithmEvent delegate defines a function type for all events that would 
    need to be animated during ARgorithm rendering process
    */

    public class BaseStructure{
        /*
        The BaseStructure class will be the parent class to all data structures.
        Will contain common functions that will be needed for all data structures
        */

        protected bool rendered = false;
        // As the ObjectMap is created. The rendered attribute is used to flag whether this object requires to be rendered yet or not

        public virtual ARgorithmEvent getEvent(string func_type){
            /*
            virtual function to get the function of Structure object that will be stored in eventlist
            This function will be overriden by all sub classes

            func_type is the second half of state_type used to select a method of structure
            */
            return new ARgorithmEvent(error);
        }

        protected void error(State state){
            /*
            In case the func_type is not supported in the structure, we return this function to raise an error
            */
        }

        public static void comment(State state){
            /*
            Static method to show comment events
            */
        }

    }
}