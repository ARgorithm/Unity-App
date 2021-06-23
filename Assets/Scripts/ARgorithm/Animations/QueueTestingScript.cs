using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ARgorithm.Structure.Typing;
using Newtonsoft.Json.Linq;



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
                this.arrow.transform.position = value;
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

    public void backButton()
    {
        Back();
    }
    //Add Last and Remove First. (deque)
    private void QueueDeclare<T>(List<int> body, GameObject placeHolder)
    {
        this.placeHolder = placeHolder;
        this.queueOfArrows = new LinkedList<IArrow>();
        if (body.Count == 0)
            return;
        var back = new VariableArrow<int>(new ContentType(body[0]));
        back.position = placeHolder.transform.position;
        back.arrow.transform.SetParent(placeHolder.transform);
        this.queueOfArrows.AddLast(back);
        for(int i=1; i<body.Count; i++)
        {
            var arrowObj = new VariableArrow<int>(new ContentType(body[i]));
            arrowObj.arrow.transform.SetParent(placeHolder.transform);
            arrowObj.position = this.queueOfArrows.Last.Value.position;
            arrowObj.position -= new Vector3(arrowObj.scale.x * 1.25f, 0, 0);
            this.queueOfArrows.AddLast(arrowObj);
        }
    }

    public void Push(int element)
    {
        var arrow = new VariableArrow<int>(new ContentType(element));
        if (queueOfArrows.Count == 0)
        {
            arrow.position = this.placeHolder.transform.position;
            arrow.arrow.transform.SetParent(placeHolder.transform);
            queueOfArrows.AddLast(arrow);
            StartCoroutine(LerpPushFunction(arrow.arrow, Constants.COMPARE_TIMER));

            return;
        }
        arrow.position = this.queueOfArrows.Last.Value.arrow.transform.position;
        arrow.position -= new Vector3(arrow.scale.x * 1.25f, 0, 0);
        arrow.arrow.transform.SetParent(placeHolder.transform);
        queueOfArrows.AddLast(arrow);
        StartCoroutine(LerpPushFunction(arrow.arrow,Constants.COMPARE_TIMER));
    }
    IEnumerator LerpPushFunction(GameObject arrow, float duration)
    {
        float time = 0;
        Vector3 startPosition = arrow.transform.position;
        Vector3 targetPosition = arrow.transform.position + new Vector3(-0.05f, 0, 0);
        Material materialToChange = arrow.GetComponent<Renderer>().material;
        Color endValueOfColor = materialToChange.color;
        Color startValueOfColor = new Color(0, 0, 0, 0);
        while (time < duration)
        {
            arrow.transform.position = Vector3.Lerp(targetPosition, startPosition, time / duration);
            materialToChange.color = Color.Lerp(startValueOfColor, endValueOfColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        arrow.transform.position = startPosition;
        materialToChange.color = endValueOfColor;
    }

    public void Pop()
    {
        if (this.queueOfArrows.Count == 0)
            return;
        /*foreach(IArrow arrow in queueOfArrows)
        {
            arrow.position += new Vector3(arrow.scale.x * 1.25f, 0, 0);
        }*/
        StartCoroutine(LerpPopFunction(Constants.COMPARE_TIMER));
    }
    IEnumerator LerpPopFunction(float duration)
    {
        var arrowFirst = queueOfArrows.First.Value;
        float time = 0;
        Vector3 startPosition = arrowFirst.arrow.transform.position;
        Vector3 targetPosition = arrowFirst.arrow.transform.position + new Vector3(0.05f,0, 0);
        Material materialToChange = arrowFirst.arrow.GetComponent<Renderer>().material; ;
        Color startValueOfColor = materialToChange.color;
        Color endValueOfColor = new Color(0, 0, 0, 0);
        while (time < duration)
        {
            arrowFirst.arrow.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            materialToChange.color = Color.Lerp(startValueOfColor, endValueOfColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        arrowFirst.arrow.transform.position = targetPosition;
        materialToChange.color = endValueOfColor;
        Destroy(arrowFirst.arrow);
        queueOfArrows.RemoveFirst();

        foreach (var arrow in this.queueOfArrows)
        {
            time = 0;
            Vector3 QstartPosition = arrow.arrow.transform.position;
            Vector3 QtargetPosition = QstartPosition + new Vector3(arrow.arrow.transform.localScale.x * 1.25f, 0, 0);
            while (time < duration)
            {
                arrow.arrow.transform.position = Vector3.Lerp(QstartPosition, QtargetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            arrow.arrow.transform.position = QtargetPosition;
        }
    }
    
    public void Front()
    {
        if (this.queueOfArrows.Count == 0)
            return;
        Color targetColor = new Color(1, 1, 1, 1);
        Material materialToChange;
        materialToChange = this.queueOfArrows.First.Value.arrow.GetComponent<Renderer>().material;
        StartCoroutine(LerpFunctionHighlight(materialToChange, targetColor, Constants.ITER_TIMER));
    }

    public void Back()
    {
        if (this.queueOfArrows.Count == 0)
            return;
        Color targetColor = new Color(1, 1, 1, 1);
        Material materialToChange;
        materialToChange = this.queueOfArrows.Last.Value.arrow.GetComponent<Renderer>().material;
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
