using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ARgorithm.Structure.Typing;
using Newtonsoft.Json.Linq;


interface IArrow
{
    ContentType faceValue
    {
        get; set;
    }

    Vector3 position
    {
        get; set;
    }

    Vector3 scale
    {
        get; set;
    }

    Quaternion rotation
    {
        get; set;
    }

    GameObject arrow
    {
        get; set;
    }
}

public class QueueTestingScript : MonoBehaviour
{
    class VariableArrow<T> : IArrow
    {
        private ContentType _faceValue;
        private Vector3 _position;
        private Vector3 _scale;
        private GameObject _arrow;
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
                for (int i = 0; i < this.arrow.transform.childCount; i++)
                {
                    var child = this.arrow.transform.GetChild(i).gameObject;
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

        public VariableArrow(ContentType value)
        {
            this.arrow = (GameObject)Instantiate(Resources.Load("Arrow") as GameObject);
            string type = typeof(T).Name;
            var cubeRenderer = this.arrow.GetComponent<Renderer>();
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
            this._scale = this.arrow.transform.localScale;
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
                this.arrow.transform.localPosition = value;
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
                this.arrow.transform.localScale = value;
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
                this.arrow.transform.rotation = value;
                this._rotation = value;
            }
        }

        public GameObject arrow
        {
            get
            {
                return this._arrow;
            }

            set
            {
                this._arrow = value;
            }
        }
    }

    private IArrow variableObject;
    public GameObject placeHolder;
    private IArrow back;
    // Array of cubeclass holds the Gameobjects
    // Start is called before the first frame update
    // Declare push pop top
    LinkedList<IArrow> queueOfArrows;
    //start is binded to start button
    public void startButton()
    {
        var body = new List<int> { 1, 2, 3, 4, 5 };
        QueueDeclare<int>(body, placeHolder);
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
    public void frontButton()
    {
        Front();
    }

    private void QueueDeclare<T>(List<int> body, GameObject placeHolder)
    {
        this.placeHolder = placeHolder;
        this.queueOfArrows = new LinkedList<IArrow>();
        if (body.Count == 0)
            return;
        foreach(var data in body)
        {
            var arrow = new VariableArrow<int>(new ContentType(data));
            queueOfArrows.AddLast(arrow);
        }
    }

    public void Push(int value)
    {
        var topOfStack = new VariableArrow<int>(new ContentType(value));
        if (queueOfArrows.Count == 0)
        {
            topOfStack.position = this.placeHolder.transform.position;
            topOfStack.arrow.transform.SetParent(placeHolder.transform);
            queueOfArrows.Enqueue(topOfStack);
            return;
        }
        topOfStack.position = this.queueOfArrows.Peek().arrow.transform.position;
        topOfStack.position += new Vector3(topOfStack.scale.x * 1.5f, 0, 0);
        topOfStack.arrow.transform.SetParent(placeHolder.transform);
        queueOfArrows.Enqueue(topOfStack);
    }

    public void Pop()
    {
        if (this.queueOfArrows.Count == 0)
            return;
        var topOfStack = this.queueOfArrows.Peek();
        this.queueOfArrows.Dequeue();
        Destroy(topOfStack.arrow);
    }

    public void Front()
    {
        if (this.queueOfArrows.Count == 0)
            return;
        Color targetColor = new Color(1, 1, 1, 1);
        Material materialToChange;
        materialToChange = this.queueOfArrows.Peek().arrow.GetComponent<Renderer>().material;
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
