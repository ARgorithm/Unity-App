using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARgorithm.Structure.Typing;
using TMPro;

public static class Constants
{
    public const float SWAP_TIMER = 0.5f;
    public const float COMPARE_TIMER = 0.5f;
    public const float ITER_TIMER = 0.5f;
}

namespace ARgorithm.Structure 
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
            public GameObject cube;
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
                    this.cube.transform.position = value;
                    this._position = value;
                }
            }
        }

        //Array of cubeclass holds the Gameobjects
        private Cube[] arrayOfCubes;

        /* Declare Function 
         * Function to animate the cubes of the array
         * <INCOMPLETE> 
         * Switch case for ndimensionally arrays
         */
        public void Declare(NDimensionalArray body, GameObject placeHolder)
        {
            List<int> shape = body.Shape;
            int dimension = body.Dimensions;

            switch (dimension)
            {
                case 1:
                    arrayOfCubes = new Cube[shape[0]];
                    float midpoint = shape[0] - 1.0f;

                    for (int i = 0; i < shape[0]; i++)
                    {
                        arrayOfCubes[i] = new Cube(i,body[i]);
                        arrayOfCubes[i].position = new Vector3(i * 2.0F - midpoint, 0.5F, 0);
                        arrayOfCubes[i].cube.transform.parent = placeHolder.transform;
                    }

                    break;
                default:
                    break;
            }
        }

        /* Swap Function
         */
        public void Swap(List<int> index1,List<int> index2)
        {
            /*cubeObjectA and B stores cube class objects picked out using index values provided in the parameters
             * from variable arrayOfCubes which is of class Cube
             */
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

        /*Function to highlight one Cube
         */
        public void Iter(List<int> index,ContentType value)
        {
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

        /*Compare Function that highlights 2 cubes of the given indexes that are being compared
         */
        public void Compare(List<int> index1, List<int> index2)
        {
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

    }
}
