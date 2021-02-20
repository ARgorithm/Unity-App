using UnityEngine;

/*
The ARgorithm.Utils namespace will contain all the utility classes and objects that will be required for ARgorithm execution
For example: Parser. Other classes that could be implemented in this namespace include
 - A Stage where all the structures will be rendered
 - The Eventlist which would be a wrapper around a list of delegates
 - The ObjectMap which will be a string(id) to object mapping
 - The StateLog which will allow us to traverse and verify previous states.
*/

namespace ARgorithm.Utils
{
    public class Parser{
        /*
        The Parser class is the class that will be used to extract data from the `ARgorithm.Models.ExecutionResponse`
        This will return the Stage with the Eventlist, StateLog and ObjectMap

        Note:
            We could make the parser a singleton object like `ARgorithm.Client.APIClient` 
        */
    }
}