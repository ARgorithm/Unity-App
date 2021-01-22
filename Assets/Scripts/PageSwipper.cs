using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/* This script controls the scrolling under Gameobject UI->ARgorithmCloud Menu->Panel List Holder
Don't do changes to the below script */

public class PageSwipper : MonoBehaviour , IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation;
    private Vector3 initialpos;
    // Start is called before the first frame update
    void Start()
    {
        panelLocation = transform.position;
        initialpos = panelLocation;
    }

    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.y - data.position.y;
        transform.position = panelLocation - new Vector3(0, difference, 0);
        
    }
    /* Prevents the swipping downwards below a certain point*/
    public float easing = 0.5f;

    public void OnEndDrag(PointerEventData data)
    {
        if (transform.position.y < initialpos.y)
        {
            StartCoroutine(SmoothMove(transform.position, initialpos, easing));
            panelLocation = initialpos;
        }
        else
        {
            panelLocation = transform.position;
        }
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while(t<= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

    }
}
