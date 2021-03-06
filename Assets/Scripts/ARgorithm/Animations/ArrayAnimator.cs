﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ARgorithm.Structure.Typing;

public static class Constants
{
    /*
    Constant values used to control animation
    */
    public const float SWAP_TIMER = 0.5f;
    public const float COMPARE_TIMER = 0.5f;
    public const float ITER_TIMER = 0.5f;
}

namespace ARgorithm.Animations 
{
    public class ArrayAnimator:MonoBehaviour
    {

        /* Cube Class
         * Generates,sets and gets the index and face values on the cubes automatically
         */
        class Cube
        {
            private ContentType _faceValue;
            private int _index;
            private Vector3 _position;
            private Vector3 _scale;
            public GameObject cube;
            private Quaternion _rotation;
            public int index
            {
                get
                {
                    return _index;
                }
                set
                {
                    _index = value;
                    var child = this.cube.transform.GetChild(5).gameObject;
                    child.GetComponent<TextMeshPro>().SetText("[" + _index.ToString() + "]");
                }
            }

            public ContentType faceValue
            {
                get
                {
                    return _faceValue;
                }
                set
                {
                    _faceValue = value;
                    for (int i = 0; i < 5; i++)
                    {
                        var child = this.cube.transform.GetChild(i).gameObject;
                        child.GetComponent<TextMeshPro>().SetText(_faceValue.Value);
                    }
                }
            }

            public Cube(int index, ContentType value)
            {
                this.cube = (GameObject)Instantiate(Resources.Load("Cube") as GameObject);
                this._scale = this.cube.transform.localScale;
                this.faceValue = value;
                this.index = index;
            }

            public Vector3 position
            {
                get
                {
                    return _position;
                }
                set
                {
                    this.cube.transform.localPosition = value;
                    this._position = value;
                }
            }

            public Vector3 scale
            {
                get
                {
                    return _scale;
                }

                set
                {
                    this.cube.transform.localScale = value;
                    this._scale = value;
                }
            }

            public Quaternion rotation
            {
                get
                {
                    return _rotation;
                }
                set
                {
                    this.cube.transform.rotation = value;
                    this._rotation = value;
                }
            }
        }

        //Array of cubeclass holds the Gameobjects
        private Cube[] arrayOfCubes;

        public void Declare(NDimensionalArray body, GameObject placeHolder)
        {
            /*
            Sets Array represented by Cubes at placeholder
            */
            List<int> shape = body.Shape;
            int dimension = body.Dimensions;
            GameObject array = new GameObject("array");
            switch (dimension)
            {
                case 1:
                    Array1DDeclare(array, placeHolder, shape, body);
                    break;
                default:
                    break;
            }
        }

        private void Array1DDeclare(GameObject array,GameObject placeHolder,List<int> shape,NDimensionalArray body)
        {
            // Handles 1-D arrays
            arrayOfCubes = new Cube[shape[0]];
            array.transform.parent = placeHolder.transform;
            array.transform.position = placeHolder.transform.position;
            array.transform.rotation = placeHolder.transform.rotation;

            for (int i = 0; i < shape[0]; i++)
            {
                arrayOfCubes[i] = new Cube(i, body[i]);
                arrayOfCubes[i].cube.transform.parent = array.transform;

            }
            //Vector3 midpoint;
            float distanceX = arrayOfCubes[0].scale.x * 2;
            float arrayScale = (arrayOfCubes[0].scale.x + distanceX) * shape[0] - (distanceX * 2);
            float offset = arrayOfCubes[0].scale.x * 0.5f;
            for (int i = 0; i < shape[0]; i++)
            {
                // changing local position
                arrayOfCubes[i].rotation = array.transform.rotation;
                arrayOfCubes[i].position = new Vector3(i * distanceX - (arrayScale * 0.5f) + offset, offset, 0);
            }
        }
        /* Swap Function
         */
        public void Swap(List<int> index1,List<int> index2)
        {
            // Swaps position of 2 Cubes stored at index1 and index2
            switch (index1.Count)
            {
                case 1:
                    Cube cubeObjectA = arrayOfCubes[index1[0]];
                    Cube cubeObjectB = arrayOfCubes[index2[0]];
                    /*swapping the index values of the gameobjects
                     */
                    Cube t = arrayOfCubes[index1[0]];
                    arrayOfCubes[index1[0]] = arrayOfCubes[index2[0]];
                    arrayOfCubes[index2[0]] = t;

                    int indexOfObjectA = cubeObjectA.index;
                    cubeObjectA.index = cubeObjectB.index;
                    cubeObjectB.index = indexOfObjectA;
                    /*does the animation for the swapping of the cubes
                     */
                    StartCoroutine(LerpFunctionSwap(cubeObjectA, cubeObjectB, Constants.SWAP_TIMER));
                    /*swapping the cube classes
                     */
                   
                    break;
                default:
                    Debug.Log("Error in Swap function[Dimensions]");
                    break;
            }
        }

     

        IEnumerator LerpFunctionSwap(Cube objectA, Cube objectB, float duration)
        {
            float time = 0;
            Vector3 startPositionObjectA = objectA.position;
            Vector3 startPositionObjectB = objectB.position;
            Vector3 ObjectACenter = (startPositionObjectB + startPositionObjectA) * 0.5f;
            Vector3 ObjectBCenter = (startPositionObjectB + startPositionObjectA) * 0.5f;

            ObjectACenter -= new Vector3(0.001f, 0, 0);
            ObjectBCenter -= new Vector3(0, 0, 0.1f);


            while (time < duration)
            {
                objectA.position = Vector3.Slerp(startPositionObjectA - ObjectACenter, startPositionObjectB - ObjectACenter, time / duration) + ObjectACenter;
                objectB.position = Vector3.Slerp(startPositionObjectB - ObjectBCenter, startPositionObjectA - ObjectBCenter, time / duration) + ObjectBCenter;

                time += Time.deltaTime;
                yield return null;
            }

            objectA.position = startPositionObjectB;
            objectB.position = startPositionObjectA;
        }

        public void Iter(List<int> index,ContentType value)
        {
            /*
            Function to highlight one Cube
            */
            Cube cube = arrayOfCubes[index[0]];
            cube.faceValue = value;
            Material materialToChange;
            Color targetColor = new Color(1, 0, 0, 1);

            materialToChange = cube.cube.GetComponent<Renderer>().material;
            StartCoroutine(LerpFunctionIter(materialToChange, targetColor, Constants.ITER_TIMER));

        }

        IEnumerator LerpFunctionIter(Material materialToChange, Color endValue, float duration)
        {
            float time = 0;
            Color startValue = materialToChange.color;

            while (time < duration)
            {
                materialToChange.color = Color.Lerp(startValue, endValue, time / duration);

                time += Time.deltaTime;
                yield return null;
            }
            materialToChange.color = endValue;

            time = 0;
            while (time < duration)
            {
                materialToChange.color = Color.Lerp(endValue, startValue, time / duration);

                time += Time.deltaTime;
                yield return null;
            }
            materialToChange.color = startValue;
        }

        public void Compare(List<int> index1, List<int> index2)
        {
            /*
            Compare Function that highlights 2 cubes of the given indexes that are being compared
            */
            Cube cubeObjectA = arrayOfCubes[index1[0]];
            Cube cubeObjectB = arrayOfCubes[index2[0]];
            Material materialToChange, materialToChange2;
            Color targetColor = new Color(1, 0, 0, 1);

            materialToChange = cubeObjectA.cube.GetComponent<Renderer>().material;
            materialToChange2 = cubeObjectB.cube.GetComponent<Renderer>().material;

            StartCoroutine(LerpFunctionCompare(materialToChange, materialToChange2, targetColor, Constants.COMPARE_TIMER));

        }

        IEnumerator LerpFunctionCompare(Material materialToChange, Material materialToChange2, Color endValue, float duration)
        {
            float time = 0;
            Color startValue = materialToChange.color;
            Color startValue2 = materialToChange2.color;


            while (time < duration)
            {
                materialToChange.color = Color.Lerp(startValue, endValue, time / duration);
                materialToChange2.color = Color.Lerp(startValue, endValue, time / duration);

                time += Time.deltaTime;
                yield return null;
            }
            materialToChange.color = endValue;
            materialToChange2.color = endValue;

            time = 0;
            while (time < duration)
            {
                materialToChange.color = Color.Lerp(endValue, startValue, time / duration);
                materialToChange2.color = Color.Lerp(endValue, startValue, time / duration);

                time += Time.deltaTime;
                yield return null;
            }
            materialToChange.color = startValue;
            materialToChange2.color = startValue;

        }
        
        public void Set(List<int> index, ContentType value)
        {
            // Function to update array without animations
            switch (index.Count)
            {
                case 1:
                    arrayOfCubes[index[0]].faceValue = value;
                    break;
                default:
                    break;
            }
        }


    }
}
