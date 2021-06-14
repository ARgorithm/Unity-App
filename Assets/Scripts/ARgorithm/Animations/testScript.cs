using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ARgorithm.Structure.Typing;
using Newtonsoft.Json.Linq;


interface ITile
{
    ContentType faceValue
    {
        get;set;
    }

    Vector3 position
    {
        get;set;
    }

    Vector3 scale
    {
        get;set;
    }

    Quaternion rotation
    {
        get;set;
    }

    GameObject tile
    {
        get;set;
    }
}

public class testScript : MonoBehaviour
{
    class VariableTile<T> : ITile
    {
        private ContentType _faceValue;
        private Vector3 _position;
        private Vector3 _scale;
        private GameObject _tile;
        private Quaternion _rotation;
        public ContentType faceValue
        {
            get
            {
                return _faceValue;
            }
            set
            {
                _faceValue = value;
                for (int i = 0; i < this.tile.transform.childCount; i++)
                {
                    var child = this.tile.transform.GetChild(i).gameObject;
                    string text;
                    string type = typeof(T).Name;
             
                    if (type == "Int32")
                    {
                        text = _faceValue.Value.ToString();
                    }
                    else if (type == "Single")
                    {
                        string str = _faceValue.Value.ToString();
                        float f = float.Parse(str);
                        text = f.ToString("0.0000");
                    }
                    else if (type == "String")
                    {
                        text = _faceValue.Value.ToString();
                    }
                    else
                    {
                        text = "";
                    }
                    child.GetComponent<TextMeshPro>().SetText(text);
                }
            }
        }

        public VariableTile(ContentType value)
        {
            this.tile = (GameObject)Instantiate(Resources.Load("Tile") as GameObject);
            string type = typeof(T).Name;
            var cubeRenderer = this.tile.GetComponent<Renderer>();
            if (type == "Int32")
            {
                cubeRenderer.material.SetColor("_Color", Color.blue);
            }
            else if (type == "Single")
            {
                cubeRenderer.material.SetColor("_Color", Color.green);
            }
            else if (type == "String")
            {
                cubeRenderer.material.SetColor("_Color", Color.red);
            }
            this._scale = this.tile.transform.localScale;
            this.faceValue = value;
        }

        public Vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
                this.tile.transform.localPosition = value;
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
                this.tile.transform.localScale = value;
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
                this.tile.transform.rotation = value;
                this._rotation = value;
            }
        }

        public GameObject tile
        {
            get
            {
                return this._tile;
            }

            set
            {
                this._tile = value;
            }
        }
    }
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
                for (int i = 0; i < this.cube.transform.childCount; i++)
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

    

    private ITile variableObject;
    public GameObject placeHolder;

    // Array of cubeclass holds the Gameobjects
    // Start is called before the first frame update
    // Declare push pop top
    Stack<ITile> stackOfTiles;
    //start is binded to start button
    public void startButton()
    {
        var body = new List<int> {};
        StackDeclare<int>(body, placeHolder);
    }
    //push is bindded to push button
    public void pushButton()
    {
        Push(100);
    }
    //pop is binded to pop button
    public void popButton()
    {
        Pop();
    }
    //Show top is binded to top button
    public void topButton()
    {
        Top();
    }

    private void StackDeclare<T>(List<int> body,GameObject placeHolder)
    {
        this.placeHolder = placeHolder;
        this.stackOfTiles = new Stack<ITile>();
        if (body.Count == 0)
            return;
        var bottom = new VariableTile<T>(new ContentType(body[0]));
        bottom.position = placeHolder.transform.position;
        bottom.position += new Vector3(0, bottom.scale.y * 0.5f, 0);
        bottom.tile.transform.SetParent(placeHolder.transform);
        this.stackOfTiles.Push(bottom);
        for (int i = 1; i < body.Count; i++) 
        {
            var tileObj = new VariableTile<T>(new ContentType(body[i]));
            tileObj.tile.transform.SetParent(placeHolder.transform);
            tileObj.position = bottom.position;
            tileObj.rotation = placeHolder.transform.rotation;
            float offset = tileObj.scale.y * 0.5f;
            tileObj.position += new Vector3(0, offset+tileObj.scale.y, 0);
            this.stackOfTiles.Push(tileObj);
            bottom = tileObj;
        }
    }

    public void Push(int value)
    {
        var topOfStack = new VariableTile<int>(new ContentType(value));
        if (stackOfTiles.Count == 0)
        {
            topOfStack.position = this.placeHolder.transform.position;
            topOfStack.position += new Vector3(0, topOfStack.scale.y * 0.5f, 0);
            topOfStack.tile.transform.SetParent(placeHolder.transform);
            stackOfTiles.Push(topOfStack);
            return;
        }
        topOfStack.position = this.stackOfTiles.Peek().tile.transform.position;
        topOfStack.position += new Vector3(0, topOfStack.scale.y * 1.5f, 0);
        topOfStack.tile.transform.SetParent(placeHolder.transform);
        stackOfTiles.Push(topOfStack);
    }

    public void Pop()
    {
        if (this.stackOfTiles.Count == 0)
            return;
        var topOfStack = this.stackOfTiles.Peek();
        this.stackOfTiles.Pop();
        Destroy(topOfStack.tile);
    }

    public void Top()
    {
        if (this.stackOfTiles.Count == 0)
            return;
        Color targetColor = new Color(1, 1, 1, 1);
        Material materialToChange;
        materialToChange = this.stackOfTiles.Peek().tile.GetComponent<Renderer>().material;
        StartCoroutine(LerpFunctionHighlight(materialToChange, targetColor, Constants.ITER_TIMER));
    }

    IEnumerator LerpFunctionHighlight(Material materialToChange, Color endValue, float duration)
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
}
