using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ARgorithm.Structure.Typing;

namespace ARgorithm.Animations
{
    abstract class GenericVariableCube
    {
        public abstract GameObject cube
        {
            get;
            set;
        }
        public abstract ContentType faceValue
        {
            get;
            set;
        }
        public abstract Vector3 position
        {
            get;
            set;
        }
        public abstract Vector3 scale
        {
            get;
            set;
        }
        public abstract Quaternion rotation 
        {
            get;
            set;
        }

    }
    
    public class VariableAnimator: MonoBehaviour
    {
        class VariableCube<T> : GenericVariableCube
        {
            private ContentType _faceValue;
            private Vector3 _position;
            private Vector3 _scale;
            private  GameObject _cube;
            private Quaternion _rotation;
            public override GameObject cube
            {
                get
                {
                    return this._cube;
                }
                set
                {
                    this._cube = value;
                }
            }
            public override ContentType faceValue 
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
                        else if (type == "Boolean")
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

            public VariableCube(ContentType value)
            {
                this.cube = (GameObject)Instantiate(Resources.Load("Cube") as GameObject);
                string type = typeof(T).Name;
                var cubeRenderer = this.cube.GetComponent<Renderer>();
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
                else if (type == "Boolean")
                {
                    cubeRenderer.material.SetColor("_Color", Color.cyan);
                }
                this._scale = this.cube.transform.localScale;
                this.faceValue = value;
            }

            public override Vector3 position
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

            public override Vector3 scale
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

            public override Quaternion rotation
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
        private GenericVariableCube variableObject;
        public void Declare(ContentType variable,GameObject placeHolder)
        {
            string type = variable.type;
            Debug.Log(type);
            switch (type)
            {
                case "Integer":
                    VariableDeclare<int>(variable, placeHolder);
                    break;
                case "Float":
                    VariableDeclare<float>(variable, placeHolder);
                    break;
                case "String":
                    VariableDeclare<string>(variable, placeHolder);
                    break;
                case "Boolean":
                    VariableDeclare<bool>(variable, placeHolder);
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        private void VariableDeclare<T>(ContentType variable, GameObject placeHolder)
        {
            var variableCube = new VariableCube<T>(variable);
            variableObject = (VariableCube<T>)variableCube;
            variableCube.cube.transform.SetParent(placeHolder.transform);
            variableCube.position = placeHolder.transform.position;
            variableCube.rotation = placeHolder.transform.rotation;
            float offset = variableCube.cube.transform.localScale.x * 0.5f;
            variableCube.position += new Vector3(0, offset, 0);
        }
        public void Set(ContentType value)
        {
            this.variableObject.faceValue = value;
        }

        public void Highlight(ContentType value)
        {
            variableObject.faceValue = value;
            Color targetColor = new Color(1, 1, 1, 1);
            Material materialToChange;
            materialToChange = variableObject.cube.GetComponent<Renderer>().material;
            StartCoroutine(LerpFunctionHighlight (materialToChange, targetColor, Constants.ITER_TIMER));
        }

        IEnumerator LerpFunctionHighlight (Material materialToChange, Color endValue, float duration)
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
