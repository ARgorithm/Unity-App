using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ARgorithm.Structure.Typing;

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

namespace ARgorithm.Animations
{
    public class QueueAnimator : MonoBehaviour
    {
        class VariableArrow : IArrow
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
                        string text = _faceValue.Value.ToString();
                        child.GetComponent<TextMeshPro>().SetText(text);
                    }
                }
            }

            public VariableArrow(ContentType value)
            {
                this.arrow = (GameObject)Instantiate(Resources.Load("Arrow") as GameObject);
                var cubeRenderer = this.arrow.GetComponent<Renderer>();
                cubeRenderer.material.SetColor("_Color", Color.magenta);
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

        //Variables required for all the functions and stores the state of the system.
        private GameObject placeholder;
        private LinkedList<IArrow> queueOfArrows;
        public void Declare(string name, List<ContentType> body, GameObject placeHolder)
        {
            this.placeholder = placeHolder;
            this.queueOfArrows = new LinkedList<IArrow>();
            if (body.Count == 0)
                return;
            var back = new VariableArrow(body[0]);
            back.position = placeholder.transform.position;
            back.arrow.transform.SetParent(placeholder.transform);
            this.queueOfArrows.AddLast(back);
            for (int i = 1; i < body.Count; i++)
            {
                var arrowObj = new VariableArrow(body[i]);
                arrowObj.arrow.transform.SetParent(placeholder.transform);
                arrowObj.position = this.queueOfArrows.Last.Value.position;
                arrowObj.position -= new Vector3(arrowObj.scale.x * 1.25f, 0, 0);
                this.queueOfArrows.AddLast(arrowObj);
            }
        }

        public void Pop() 
        {
            if (this.queueOfArrows.Count == 0)
                return;
            /*foreach (var arrow in queueOfArrows)
            {
                arrow.position += new Vector3(arrow.scale.x * 1.25f, 0, 0);
            }
            var arrowFirst = queueOfArrows.First.Value;
            arrowFirst.arrow.transform.SetParent(null);
            Destroy(arrowFirst.arrow);
            queueOfArrows.RemoveFirst();*/
            StartCoroutine(LerpPopFunction(Constants.POP_TIMER));
        }
        //Function to Animate the Removal of an Arrow at the front of the Queue 
        //and moving the queue forward (Recentering of the queue in 3D space)
        IEnumerator LerpPopFunction(float duration)
        {
            var arrowFirst = queueOfArrows.First.Value;
            float time = 0;
            Vector3 startPosition = arrowFirst.arrow.transform.position;
            Vector3 targetPosition = arrowFirst.arrow.transform.position + new Vector3(0.05f, 0, 0);
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

            //Destroys Gameobject and removes from queue here
            arrowFirst.arrow.transform.SetParent(null);
            Destroy(arrowFirst.arrow);
            queueOfArrows.RemoveFirst();

            //Moving the queue forward
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
        public void Push(ContentType element)
        {
            var arrow = new VariableArrow(element);
            if (queueOfArrows.Count == 0)
            {
                arrow.position = this.placeholder.transform.position;
                arrow.arrow.transform.SetParent(placeholder.transform);
                queueOfArrows.AddLast(arrow);
                StartCoroutine(LerpPushFunction(arrow.arrow, Constants.PUSH_TIMER));
                return;
            }
            arrow.position = this.queueOfArrows.Last.Value.arrow.transform.position;
            arrow.position -= new Vector3(arrow.scale.x * 1.25f, 0, 0);
            arrow.arrow.transform.SetParent(placeholder.transform);
            queueOfArrows.AddLast(arrow);
            StartCoroutine(LerpPushFunction(arrow.arrow, Constants.PUSH_TIMER));
        }
        //Function to Animate the Addition of an Arrow to the back of the Queue 
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
        //Function to Highlight the Arrow of Interest in the 3D space
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

        public void PopLast()
        {
            if (this.queueOfArrows.Count == 0)
                return;
            var arrowLast = queueOfArrows.Last.Value;
            arrowLast.arrow.transform.SetParent(null);
            Destroy(arrowLast.arrow);
            queueOfArrows.RemoveLast();
        }

        public void PushFirst(ContentType element)
        {
            var arrow = new VariableArrow(element);
            if (queueOfArrows.Count == 0)
            {
                arrow.position = this.placeholder.transform.position;
                arrow.arrow.transform.SetParent(placeholder.transform);
                queueOfArrows.AddFirst(arrow);
                return;
            }
            foreach (var ar in queueOfArrows)
            {
                ar.position -= new Vector3(arrow.scale.x * 1.25f, 0, 0);
            }
            arrow.position = placeholder.transform.position;
            arrow.arrow.transform.SetParent(placeholder.transform);
            queueOfArrows.AddFirst(arrow);

        }
    }
}