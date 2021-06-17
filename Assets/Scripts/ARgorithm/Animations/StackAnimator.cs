using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ARgorithm.Structure.Typing;

namespace ARgorithm.Animations
{
    interface ITile
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

        GameObject tile
        {
            get; set;
        }
    }


    public class StackAnimator : MonoBehaviour
    {
        class VariableTile : ITile
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
                        string text = _faceValue.Value.ToString();
                        child.GetComponent<TextMeshPro>().SetText(text);
                    }
                }
            }

            public VariableTile(ContentType value)
            {
                this.tile = (GameObject)Instantiate(Resources.Load("Tile") as GameObject);
                var cubeRenderer = this.tile.GetComponent<Renderer>();
                cubeRenderer.material.SetColor("_Color", Color.cyan);
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
        private GameObject placeHolder;
        private Stack<ITile> stackOfTiles;
        public void Declare(string name, List<ContentType> body, GameObject place)
        {
            this.placeHolder = place;
            this.stackOfTiles = new Stack<ITile>();
            if (body.Count == 0)
                return;
            var bottom = new VariableTile(body[0]);
            bottom.position = placeHolder.transform.position;
            bottom.position += new Vector3(0, bottom.scale.y * 0.5f, 0);
            bottom.tile.transform.SetParent(placeHolder.transform);
            this.stackOfTiles.Push(bottom);
            for (int i = 1; i < body.Count; i++)
            {
                var tileObj = new VariableTile(body[i]);
                tileObj.tile.transform.SetParent(placeHolder.transform);
                tileObj.position = bottom.position;
                tileObj.rotation = placeHolder.transform.rotation;
                float offset = tileObj.scale.y * 0.5f;
                tileObj.position += new Vector3(0, offset + tileObj.scale.y, 0);
                this.stackOfTiles.Push(tileObj);
                bottom = tileObj;
            }
        }

        public void Push(ContentType element)
        {
            var topOfStack = new VariableTile(element);
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
}