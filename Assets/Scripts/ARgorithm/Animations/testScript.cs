using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ARgorithm.Structure.Typing;


public class testScript : MonoBehaviour
{
    class Cube
    {
        private int _faceValue;
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
                /*
                var child = this.cube.transform.GetChild(5).gameObject;
                child.GetComponent<TextMeshPro>().SetText("[" + _index.ToString() + "]");
                */
            }
        }

        public int faceValue
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
                    child.GetComponent<TextMeshPro>().SetText(_faceValue.ToString());
                }
            }
        }

        public Cube(int index, int value)
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
    private int[,,] body = new int[3, 3, 3]{
                { { 1, 2, 3}, {4, 5, 6}, {7,8,9 }    },
                { { 10, 11, 12}, {13, 14, 15},{16,17,18 } },
                { { 19, 20, 21}, {22, 23, 24},{25,26,27 } }
            };
    // Start is called before the first frame update
    public void start()
    {
        GameObject placeHolderObject = new GameObject("placeHolder");
        placeHolderObject.transform.position = new Vector3(0, 0, 0);
       

        int[] shape1D = { 5 };
        int[] body1D = new int[5] { 1, 2, 3, 4, 5 };
        /*
         * 
         * int[,] body2D = new int[4, 4] {
            { 0, 1, 2, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 },
            { 9, 10, 11, 12 }
        };
         */

        int[,] body2D = new int[4, 4] {
            { 0, 1, 2, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 },
            { 9, 10, 11, 12 }
        }; 
        int[] shape2D = { body2D.GetLength(0), body2D.GetLength(1)};

        /*int[,,] body3D = new int[2, 3, 4]{
                { { 1, 2, 3 ,4}, {4, 5, 6,7}, {7,8,9,10 },    },
                { { 10, 11, 12,13}, {13, 14, 15,16},{16,17,18,19 } },
            };*/

        int[,,] body3D = new int[1, 2, 3]{
                { { 1, 2, 3}, {4, 5, 6} },
            };
        /*int[,,] body3D = new int[3, 3, 3]{
                { { 1, 2, 3}, {4, 5, 6}, {7,8,9 }    },
                { { 10, 11, 12}, {13, 14, 15},{16,17,18 } },
                { { 19, 20, 21}, {22, 23, 24},{25,26,27 } }
            };*/
        /*int[,,] body3D = new int[4, 4, 4]{
                { { 1, 2, 3 ,4}, {4, 5, 6,7}, {7,8,9,10 }, {4, 5, 6,7},   },
                { { 10, 11, 12,13}, {13, 14, 15,14},{16,17,18,19 } , {4, 5, 6,7}, },
                { { 19, 20, 21,22}, {22, 23, 24,25},{25,26,27,29 } , {4, 5, 6,7}, },
                { { 19, 20, 21,22}, {22, 23, 24,25},{25,26,27,29 }, {4, 5, 6,7},  },

            };*/

        int[] shape3D = { body3D.GetLength(0), body3D.GetLength(1), body3D.GetLength(2) };
        GameObject array = new GameObject("array");

        //Array1DDeclare(array, shape1D, body1D, placeHolderObject);
        //Array2DDeclare(array, shape2D, body2D, placeHolderObject);
        Array3DDeclare(array, shape3D, body3D,placeHolderObject);
    }

    

    public void SwapUtility()
    {
        List<int> index1 = new List<int>() {0,0,0};
        List<int> index2 = new List<int>() {0,2,0};
        Swap(index1,index2,body);
    }

    private void Array1DDeclare(GameObject array, int[] shape, int[] body, GameObject placeHolder)
    {
        // Handles 1-D arrays
        arrayOfCubes = new Cube[shape[0]];
        array.transform.SetParent(placeHolder.transform);
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
            arrayOfCubes[i].position = new Vector3(xPosition,yPosition , 0);
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
            indexGameObject.transform.position = new Vector3(xPosition, yPosition, 0) - midpoint;
        }
        array.transform.position += new Vector3(0,offset, 0);
    }

    private void Array2DDeclare(GameObject array,int[] shape, int[,] body, GameObject placeHolder)
    {
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
        for (int i = 0; i < shape[0] * shape[1] ; i++)
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
            indexGameObject.transform.position = new Vector3(xPosition, yPosition  , 0) - midpoint;
        }
        for (int i = 0; i < shape[0]; i++)
        {
            xPosition = -(arrayScale * 0.5f) - offset * 2;
            yPosition = (shape[0]-i-1) * distanceX - (arrayScale * 0.5f) + offset;
            GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
            indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
            indexGameObject.transform.SetParent(array.transform);
            indexGameObject.transform.position = new Vector3(xPosition, yPosition, 0) - midpoint;
        }
        array.transform.position += new Vector3(0, shape[0] * arrayOfCubes[0].scale.x - offset, 0);

    }

    private void Array3DDeclare(GameObject array, int[] shape, int[,,] body, GameObject placeHolder)
    {
        // Handles 3-D arrays
        arrayOfCubes = new Cube[shape[0] * shape[1] * shape[2]];
        array.transform.SetParent(placeHolder.transform,false);
        array.transform.position = placeHolder.transform.position;
        array.transform.rotation = placeHolder.transform.rotation;
        //index i is mapped as body[x,y,z]
        for (int i = 0; i < shape[0] * shape[1] * shape[2]; i++)
        {
            int[] indices = to3D(i, shape[1], shape[2]);
            int xIndex = indices[0], yIndex = indices[1], zIndex = indices[2];
            arrayOfCubes[i] = new Cube(i, body[xIndex, yIndex, zIndex]);
            arrayOfCubes[i].cube.transform.SetParent(array.transform,false);
        }
        float distanceX = arrayOfCubes[0].scale.x * 2;
        float arrayScale = (arrayOfCubes[0].scale.x + distanceX) * shape[0] - (distanceX * 2);
        float offset = arrayOfCubes[0].scale.x * 0.5f;
        float xPosition = 0;
        float zPosition = 0;
        float yPosition = 0;
        Vector3 midpoint = new Vector3(0,0,0);
        for (int i = 0; i < shape[0] * shape[1] * shape[2]; i++)
        {
            // changing local position
            xPosition = (i % shape[2]) * distanceX - (arrayScale * 0.5f) + offset;
            yPosition = -((i / shape[2]) % shape[1]) * distanceX - (arrayScale * 0.5f) + offset;
            zPosition = (( i) / (shape[1] * shape[2])) * distanceX - (arrayScale * 0.5f) + offset;

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
            yPosition = - (arrayScale * 0.5f) + offset*3.5f;
            zPosition = -(arrayScale * 0.5f) ;
            GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
            indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
            indexGameObject.transform.SetParent(array.transform);
            indexGameObject.transform.position = new Vector3(xPosition-midpoint.x, yPosition-midpoint.y, zPosition-midpoint.z) ;
        }
        for (int i = 0; i < shape[1]; i++)
        {
            xPosition = -(arrayScale * 0.5f) - offset * 1.5f;
            yPosition = -i * distanceX - (arrayScale * 0.5f) + offset;
            zPosition = -(arrayScale * 0.5f) ;
            GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
            indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
            indexGameObject.transform.SetParent(array.transform);
            indexGameObject.transform.position = new Vector3(xPosition-midpoint.x, yPosition-midpoint.y, zPosition - midpoint.z);
        }
        for(int i = 0; i < shape[0]; i++)
        {
            zPosition = i * distanceX - (arrayScale * 0.5f) + offset ;
            yPosition = -(arrayScale * 0.5f) + 3.5f * offset;
            xPosition = - (arrayScale * 0.5f);
            GameObject indexGameObject = Instantiate(Resources.Load("IndexPrefab") as GameObject);
            indexGameObject.GetComponent<TextMeshPro>().SetText("[" + i + "]");
            indexGameObject.transform.SetParent(array.transform);
            indexGameObject.transform.position = new Vector3(xPosition - midpoint.x, yPosition - midpoint.y, zPosition - midpoint.z);
            indexGameObject.transform.Rotate(0,90,0);
        }
        array.transform.position += new Vector3(0, shape[1] * arrayOfCubes[0].scale.x - offset,0);
    }

    public int[] to3D(int i, int yLength, int zLength)
    {
        int x = i / (yLength * zLength);
        int y = (i / zLength) % yLength;
        int z = i % zLength;
        return new int[] { x, y, z };
    }
    public void Swap(List<int> index1, List<int> index2, int[,,]body)
    {
        Cube cubeObjectA, cubeObjectB, t;
        int indexOfObjectA;
        int[] shape = { body.GetLength(0), body.GetLength(1), body.GetLength(2) };
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

}
