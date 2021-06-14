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
        private GameObject placeholder;
        private LinkedList<IArrow> queueOfArrows;
        public void Declare(string name, List<ContentType> body, GameObject placeHolder)
        {
            this.placeholder = placeHolder;
            this.queueOfArrows = new LinkedList<IArrow>();
            if (body.Count == 0)
                return;
            var back = new VariableArrow(body[0]);
            back.position = placeHolder.transform.position;
            back.arrow.transform.SetParent(placeHolder.transform);
            this.queueOfArrows.AddLast(back);
            for (int i = 1; i < body.Count; i++)
            {
                var arrowObj = new VariableArrow(body[i]);
                arrowObj.arrow.transform.SetParent(placeHolder.transform);
                arrowObj.position = this.queueOfArrows.Last.Value.position;
                arrowObj.position -= new Vector3(arrowObj.scale.x * 1.25f, 0, 0);
                this.queueOfArrows.AddLast(arrowObj);
            }
        }

        public void Pop() 
        {
            if (this.queueOfArrows.Count == 0)
                return;
            foreach (var arrow in queueOfArrows)
            {
                arrow.position += new Vector3(arrow.scale.x * 1.25f, 0, 0);
            }
            var arrowFirst = queueOfArrows.First.Value;
            Destroy(arrowFirst.arrow);
            queueOfArrows.RemoveFirst();
        }

        public void Push(ContentType element)
        {
            var arrow = new VariableArrow(element);
            if (queueOfArrows.Count == 0)
            {
                arrow.position = this.placeholder.transform.position;
                arrow.arrow.transform.SetParent(placeholder.transform);
                queueOfArrows.AddLast(arrow);
                return;
            }
            arrow.position = this.queueOfArrows.Last.Value.arrow.transform.position;
            arrow.position -= new Vector3(arrow.scale.x * 1.25f, 0, 0);
            arrow.arrow.transform.SetParent(placeholder.transform);
            queueOfArrows.AddLast(arrow);
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
}