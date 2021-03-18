using System.Collections;
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
        private NDimensionalArray body;

        public void Declare(NDimensionalArray body, GameObject placeHolder)
        {
            /*
            Sets Array represented by Cubes at placeholder
            */
            this.body = body;
            GameObject array = new GameObject("array");
            switch (body.Dimensions)
            {
                case 1:
                    Array1DDeclare(array, placeHolder);
                    break;
                case 2:
                    Array2DDeclare(array, placeHolder);
                    break;
                case 3:
                    Array3DDeclare(array, placeHolder);
                    break;
                default:
                    break;
            }
        }

        private void Array1DDeclare(GameObject array,GameObject placeHolder)
        {
            List<int> shape = body.Shape;

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
            float distanceX = arrayOfCubes[0].scale.x * 2;
            float arrayScale = (arrayOfCubes[0].scale.x + distanceX) * shape[0] - (distanceX * 2);
            float offset = arrayOfCubes[0].scale.x * 0.5f;
            float xPosition = 0;
            float yPosition = 0;
            Vector3 midpoint = new Vector3(0, 0, 0);
            for (int i = 0; i < shape[0]; i++)
            {
                // changing local position
                xPosition = i * distanceX - (arrayScale * 0.5f) + offset;
                yPosition = offset;
                arrayOfCubes[i].rotation = array.transform.rotation;
                arrayOfCubes[i].position = new Vector3(xPosition, yPosition, 0);
                midpoint += new Vector3(xPosition, yPosition, 0);
            }
            midpoint = midpoint / (shape[0]);

            for (int i = 0; i < shape[0]; i++)
            {
                xPosition = i * distanceX - (arrayScale * 0.5f) + offset;
                yPosition = -offset;
                arrayOfCubes[i].position -= midpoint;
                GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
                indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
                indexGameObject.transform.SetParent(array.transform);
                indexGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                indexGameObject.transform.localPosition = new Vector3(xPosition, yPosition, 0) - midpoint;
            }
            array.transform.position += new Vector3(0, offset, 0);
        }

        private void Array2DDeclare(GameObject array, GameObject placeHolder)
        {
            List<int> shape = body.Shape;

            // Handles 2-D arrays
            arrayOfCubes = new Cube[shape[0] * shape[1]];
            array.transform.SetParent(placeHolder.transform);
            array.transform.position = placeHolder.transform.position;
            array.transform.rotation = placeHolder.transform.rotation;
            for (int i = 0; i < shape[0] * shape[1]; i++)
            {
                int xIndex = i / shape[1];
                int yIndex = i % shape[1];
                arrayOfCubes[i] = new Cube(i, body[xIndex, yIndex]);
                arrayOfCubes[i].cube.transform.parent = array.transform;
            }
            float distanceX = arrayOfCubes[0].scale.x * 2;
            float arrayScale = (arrayOfCubes[0].scale.x + distanceX) * shape[0] - (distanceX * 2);
            float offset = arrayOfCubes[0].scale.x * 0.5f;
            float xPosition = 0;
            float zPosition = 0;
            float yPosition = 0;
            Vector3 midpoint = new Vector3(0, 0, 0);

            for (int i = 0; i < shape[0] * shape[1]; i++)
            {
                xPosition = (i % shape[1]) * distanceX - (arrayScale * 0.5f) + offset;
                yPosition = ((shape[0] * shape[1] - 1 - i) / shape[1]) * distanceX - (arrayScale * 0.5f) + offset;
                zPosition = 0;
                // changing local position
                arrayOfCubes[i].rotation = array.transform.rotation;
                arrayOfCubes[i].position = new Vector3(xPosition, yPosition, zPosition);
                midpoint += new Vector3(xPosition, yPosition, zPosition);
            }
            midpoint = midpoint / (shape[0] * shape[1]);
            for (int i = 0; i < shape[0] * shape[1]; i++)
            {
                arrayOfCubes[i].position -= midpoint;
            }
            for (int i = 0; i < shape[1]; i++)
            {
                xPosition = i * distanceX - (arrayScale * 0.5f) + offset;
                yPosition = ((shape[0] * shape[1] - 1) / shape[1]) * distanceX - (arrayScale * 0.5f) + offset * 3.5f;
                GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
                indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
                indexGameObject.transform.SetParent(array.transform);
                indexGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                indexGameObject.transform.localPosition = new Vector3(xPosition, yPosition, 0) - midpoint;
            }
            for (int i = 0; i < shape[0]; i++)
            {
                xPosition = -(arrayScale * 0.5f) - offset * 2;
                yPosition = (shape[0] - i - 1) * distanceX - (arrayScale * 0.5f) + offset;
                GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
                indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
                indexGameObject.transform.SetParent(array.transform);
                indexGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                indexGameObject.transform.localPosition = new Vector3(xPosition, yPosition, 0) - midpoint;
            }
            array.transform.position += new Vector3(0, shape[0] * arrayOfCubes[0].scale.x - offset, 0);
        }

        private void Array3DDeclare(GameObject array, GameObject placeHolder)
        {
            List<int> shape = body.Shape;
            // Handles 3-D arrays
            arrayOfCubes = new Cube[shape[0] * shape[1] * shape[2]];
            array.transform.SetParent(placeHolder.transform, false);
            array.transform.position = placeHolder.transform.position;
            array.transform.rotation = placeHolder.transform.rotation;
            //index i is mapped as body[x,y,z]
            for (int i = 0; i < shape[0] * shape[1] * shape[2]; i++)
            {
                int[] indices = to3D(i, shape[1], shape[2]);
                int xIndex = indices[0], yIndex = indices[1], zIndex = indices[2];
                arrayOfCubes[i] = new Cube(i, body[xIndex, yIndex, zIndex]);
                arrayOfCubes[i].cube.transform.SetParent(array.transform, false);
            }
            float distanceX = arrayOfCubes[0].scale.x * 2;
            float arrayScale = (arrayOfCubes[0].scale.x + distanceX) * shape[0] - (distanceX * 2);
            float offset = arrayOfCubes[0].scale.x * 0.5f;
            float xPosition = 0;
            float zPosition = 0;
            float yPosition = 0;
            Vector3 midpoint = new Vector3(0, 0, 0);
            for (int i = 0; i < shape[0] * shape[1] * shape[2]; i++)
            {
                // changing local position
                xPosition = (i % shape[2]) * distanceX - (arrayScale * 0.5f) + offset;
                yPosition = -((i / shape[2]) % shape[1]) * distanceX - (arrayScale * 0.5f) + offset;
                zPosition = ((i) / (shape[1] * shape[2])) * distanceX - (arrayScale * 0.5f) + offset;

                arrayOfCubes[i].rotation = array.transform.rotation;
                arrayOfCubes[i].position = new Vector3(xPosition, yPosition, zPosition);
                midpoint += new Vector3(xPosition, yPosition, zPosition);
            }
            midpoint = midpoint / (shape[0] * shape[1] * shape[2]);
            for (int i = 0; i < shape[0] * shape[1] * shape[2]; i++)
            {
                arrayOfCubes[i].position -= midpoint;
            }
            for (int i = 0; i < shape[2]; i++)
            {
                xPosition = i * distanceX - (arrayScale * 0.5f) + offset;
                yPosition = -(arrayScale * 0.5f) + offset * 3.5f;
                zPosition = -(arrayScale * 0.5f);
                GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
                indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
                indexGameObject.transform.SetParent(array.transform);
                indexGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                indexGameObject.transform.localPosition = new Vector3(xPosition - midpoint.x, yPosition - midpoint.y, zPosition - midpoint.z);
            }
            for (int i = 0; i < shape[1]; i++)
            {
                xPosition = -(arrayScale * 0.5f) - offset * 1.5f;
                yPosition = -i * distanceX - (arrayScale * 0.5f) + offset;
                zPosition = -(arrayScale * 0.5f);
                GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
                indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
                indexGameObject.transform.SetParent(array.transform);
                indexGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                indexGameObject.transform.localPosition = new Vector3(xPosition - midpoint.x, yPosition - midpoint.y, zPosition - midpoint.z);
            }
            for (int i = 0; i < shape[0]; i++)
            {
                zPosition = i * distanceX - (arrayScale * 0.5f) + offset;
                yPosition = -(arrayScale * 0.5f) + 3.5f * offset;
                xPosition = -(arrayScale * 0.5f);
                GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
                indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
                indexGameObject.transform.SetParent(array.transform);
                indexGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                indexGameObject.transform.localPosition = new Vector3(xPosition - midpoint.x, yPosition - midpoint.y, zPosition - midpoint.z);
                indexGameObject.transform.Rotate(0, 90, 0);
            }
            array.transform.position += new Vector3(0, shape[1] * arrayOfCubes[0].scale.x - offset, 0);
        }

        private int[] to3D(int i, int yLength, int zLength)
        {
            int x = i / (yLength * zLength);
            int y = (i / zLength) % yLength;
            int z = i % zLength;
            return new int[] { x, y, z };
        }
        /* Swap Function
         */
        public void Swap(List<int> index1,List<int> index2)
        {
            Cube cubeObjectA, cubeObjectB,t;
            int indexOfObjectA;
            List<int> shape = body.Shape;
            int i1, i2;
            // Swaps position of 2 Cubes stored at index1 and index2
            switch (index1.Count)
            {
                case 1:
                    cubeObjectA = arrayOfCubes[index1[0]];
                    cubeObjectB = arrayOfCubes[index2[0]];
                    /*swapping the index values of the gameobjects
                     */
                    t = arrayOfCubes[index1[0]];
                    arrayOfCubes[index1[0]] = arrayOfCubes[index2[0]];
                    arrayOfCubes[index2[0]] = t;

                    indexOfObjectA = cubeObjectA.index;
                    cubeObjectA.index = cubeObjectB.index;
                    cubeObjectB.index = indexOfObjectA;
                    /*does the animation for the swapping of the cubes
                     */
                    StartCoroutine(LerpFunctionSwap(cubeObjectA, cubeObjectB, Constants.SWAP_TIMER));
                    /*swapping the cube classes
                     */
                    break;
                case 2:
                    i1 = index1[0] * shape[1] + index1[1];
                    i2 = index2[0] * shape[1] + index2[1];
                    cubeObjectA = arrayOfCubes[i1];
                    cubeObjectB = arrayOfCubes[i2];
                    /*swapping the index values of the gameobjects
                     */
                    t = arrayOfCubes[i1];
                    arrayOfCubes[i1] = arrayOfCubes[i2];
                    arrayOfCubes[i2] = t;

                    indexOfObjectA = cubeObjectA.index;
                    cubeObjectA.index = cubeObjectB.index;
                    cubeObjectB.index = indexOfObjectA;
                    /*does the animation for the swapping of the cubes
                     */
                    StartCoroutine(LerpFunctionSwap(cubeObjectA, cubeObjectB, Constants.SWAP_TIMER));
                    /*swapping the cube classes
                    */
                    break;
                case 3:
                    i1 = index1[0] + shape[1] * (index1[1] + shape[2] * index1[2]);
                    i2 = index2[0] + shape[1] * (index2[1] + shape[2] * index2[2]);
                    cubeObjectA = arrayOfCubes[i1];
                    cubeObjectB = arrayOfCubes[i2];
                    /*swapping the index values of the gameobjects
                     */
                    t = arrayOfCubes[i1];
                    arrayOfCubes[i1] = arrayOfCubes[i2];
                    arrayOfCubes[i2] = t;

                    indexOfObjectA = cubeObjectA.index;
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
            List<int> shape = body.Shape;
            int i1;
            Cube cube;
            Material materialToChange;
            Color targetColor = new Color(1, 0, 0, 1);
            switch (body.Dimensions)
            {
                case 1:
                    i1 = index[0];
                    cube = arrayOfCubes[i1];
                    cube.faceValue = value;
                    break;
                case 2:
                    i1 = index[0] + index[1] * shape[1];
                    cube = arrayOfCubes[i1];
                    cube.faceValue = value;
                    break;
                case 3:
                    i1 = index[0] + shape[1] * index[1] + shape[1] * shape[2] * index[2];
                    cube = arrayOfCubes[i1];
                    cube.faceValue = value;
                    break;
                default:
                    cube = null;
                    break;
            }

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
            int i1;
            int i2;
            List<int> shape = body.Shape;

            switch (body.Dimensions)
            {
                case 1:
                    i1 = index1[0];
                    i2 = index2[0];
                    break;
                case 2:
                    i1 = index1[0] + index1[1] * shape[1];
                    i2 = index2[0] + index2[1] * shape[1];
                    break;
                case 3:
                    i1 = index1[0] + index1[1] * shape[1] + index1[2] * shape[1] * shape[2];
                    i2 = index2[0] + index2[1] * shape[1] + index2[2] * shape[1] * shape[2];
                    break;
                default:
                    i1 = 0;
                    i2 = 0;
                    break;
            }
            Cube cubeObjectA = arrayOfCubes[i1];
            Cube cubeObjectB = arrayOfCubes[i2];
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
            int i1 = 0;
            List<int> shape = body.Shape;

            // Function to update array without animations
            switch (index.Count)
            {
                case 1:
                    i1 = index[0];
                    break;
                case 2:
                    i1 = index[0] + index[1] * shape[1];
                    break;
                case 3:
                    i1 = index[0] + index[1] * shape[1] + index[2] * shape[1] * shape[2];
                    break;
                default:
                    break;
            }
            arrayOfCubes[i1].faceValue = value;
        }


    }
}
