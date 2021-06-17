using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

using ARgorithm.Models;

namespace ARgorithm.Structure.Typing
{
    /*
    The ARgorithm.Structure.Typing Namespace deals with the naming of ARgorithm Objects
    */
    public class ContentType{
        /*
        Sometimes strings that we parse from JTokens can be object references when it comes to nested structures
        ContentType allows us to handle both cases with one class
        */
        public string Value;
        public bool isObjectRef;
        public string type;
        string[] types = new string[18]{"None","Object","Array","Constructor","Property","Comment",
                "Integer","Float","String","Boolean","Null","Undefined","Date","Raw","Bytes","Guid",
                "Uri","TimeSpan" };
        private string objectRefPattern = @"^\$ARgorithmToolkit\.([A-Za-z]+)\:([0-9]+)$"; 

        public ContentType(){}

        public ContentType(JToken jt){
            JTokenType typeJT = jt.Type;
            int c = (int)typeJT;
            type = types[c];
            Regex rgx = new Regex(this.objectRefPattern);
            Match match = rgx.Match( (string) jt);
            if(match.Success){
                this.Value = match.Groups[1].Value;
                this.isObjectRef = true;
            }else{
                this.Value = (string) jt;
                this.isObjectRef = false;
            }
        }

        public ContentType(string str){
            Regex rgx = new Regex(this.objectRefPattern);
            Match match = rgx.Match(str);
            if(match.Success){
                this.Value = match.Groups[1].Value;
                this.isObjectRef = true;
            }else{
                this.Value = str;
                this.isObjectRef = false;
            }
        }
    }

    public class NDimensionalArray{
        /*
        The Arrays parsed from JSON response doesnt have fixed dimensions that can prove to be bothersome
        NDimensionalArray handles arrays of all dimensions by reshaping all of them as a 1-D list
        */
        private int dimensions;
        private List<int> shape;
        
        public List<ContentType> innerCol;
        
        public int Dimensions{
            get {return this.dimensions;}
        }

        public List<int> Shape{
            get {return this.shape;}
        }

        public static List<int> ToListIndex(JToken index){
            List<int> _index;
            if (index.Type == JTokenType.Integer)
            {
                _index = new List<int>();
                _index.Add((int)index);
            }
            else
            {
                _index = ((JArray)index).ToObject<List<int>>();
            }
            return _index;
        } 

        public NDimensionalArray(){}

        public NDimensionalArray(JArray arr){
            JArray jarray = arr;
            this.dimensions = 1;
            this.shape = new List<int>();
            int count = jarray.Count;
            this.shape.Add(count);
            while(count >= 0){
                JToken jt = jarray.First;
                if(jt.Type == JTokenType.Array){
                    this.dimensions += 1;
                    jarray = (JArray) jt;
                    count = jarray.Count;
                    this.shape.Add(count);
                }else{
                    break;
                }
            }
            this.innerCol = new List<ContentType>();
            foreach (JToken outer in arr)
            {
                if(this.dimensions == 1){
                    this.innerCol.Add(new ContentType(outer));
                }else{
                    JArray outerarr = (JArray) outer;
                    foreach (JToken inner in outerarr)
                    {
                        if(this.dimensions == 2){
                            this.innerCol.Add(new ContentType(inner));
                        }else{
                            JArray innerarr = (JArray) inner;
                            foreach (JToken item in innerarr){
                                this.innerCol.Add(new ContentType(item));
                            }
                        }
                    }        
                }
            }

        }

        public ContentType this[int index]{
            get{
                if(this.dimensions == 1){
                    return this.innerCol[index];
                }
                throw new NotSupportedException("Only works if NDimensionalArray.Dimensions == 1");
            }
            set{
                if(this.dimensions == 1){
                    this.innerCol[index] = value;
                }else{
                    throw new NotSupportedException("Only works if NDimensionalArray.Dimensions == 1");
                }
                
            }
        }

        public ContentType this[int index1, int index2]{
            get{
                if(this.dimensions == 2){
                    int index = this.shape[1] * index1 + index2;
                    return this.innerCol[index];
                }
                throw new NotSupportedException("Only works if NDimensionalArray.Dimensions == 1");
            }
            set{
                if(this.dimensions == 2){
                    int index = this.shape[1] * index1 + index2;
                    this.innerCol[index] = value;
                }else{
                    throw new NotSupportedException("Only works if NDimensionalArray.Dimensions == 2");
                }
            }
        }

        public ContentType this[int index1,int index2,int index3]{
            get{
                if(this.dimensions == 3){
                    int index = this.shape[1] * this.shape[2] * index1 + this.shape[2] * index2 + index3;
                    return this.innerCol[index];
                }
                throw new NotSupportedException("Only works if NDimensionalArray.Dimensions == 3");
            }
            set{
                if(this.dimensions == 3){
                    int index = this.shape[1] * this.shape[2] * index1 + this.shape[2] * index2 + index3;
                    this.innerCol[index] = value;
                }else{
                    throw new NotSupportedException("Only works if NDimensionalArray.Dimensions == 3");
                }
            }
        }

        public ContentType this[JToken jt]{
            get{
                if(jt.Type == JTokenType.Integer){
                    int index = (int) jt;
                    return this[index];
                }else if(jt.Type == JTokenType.Array){
                    JArray jarr = (JArray) jt;
                    if(jarr.Count == 2){
                        int index1 = (int) jarr[0];
                        int index2 = (int) jarr[1];
                        return this[index1,index2];
                    }
                    else if(jarr.Count == 3){
                        int index1 = (int) jarr[0];
                        int index2 = (int) jarr[1];
                        int index3 = (int) jarr[2];
                        return this[index1,index2,index3];
                    }
                }
                throw new NotSupportedException("JToken value not supported");
            }
            set{
                if(jt.Type == JTokenType.Integer){
                    int index = (int) jt;
                    this[index] = value;
                }else if(jt.Type == JTokenType.Array){
                    JArray jarr = (JArray) jt;
                    if(jarr.Count == 2){
                        int index1 = (int) jarr[0];
                        int index2 = (int) jarr[1];
                        this[index1,index2] = value;
                    }
                    else if(jarr.Count == 3){
                        int index1 = (int) jarr[0];
                        int index2 = (int) jarr[1];
                        int index3 = (int) jarr[2];
                        this[index1,index2,index3] = value;
                    }else{
                        throw new NotSupportedException("JToken value not supported");
                    }
                }else{
                    throw new NotSupportedException("JToken value not supported");
                }
                
            }
        }
    }
}