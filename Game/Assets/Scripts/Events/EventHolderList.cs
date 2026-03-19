using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHolderList : MonoBehaviour
{
    public List<SaveAndLoadLevel.Event> events = new List<SaveAndLoadLevel.Event>();

    public List<LineRenderer> lineRenderers = new List<LineRenderer>();

    public GameObject wheelPickerEventList_On;
    public GameObject wheelPickerEventList_Do;

    public void ClearLineRenderers()
    {
        foreach (LineRenderer lr in lineRenderers)
        {
            GameObject.Destroy(lr);
        }

        lineRenderers.Clear();
    }

    public void HighlightLinesOnHoverOverObject()
    {
        foreach (LineRenderer lr in lineRenderers)
        {
            lr.startColor = EventLineStylingManager.Instance.eventLineColorStart_Highlight;
            lr.endColor = EventLineStylingManager.Instance.eventLineColorEnd_Highlight;
    
            lr.startWidth = EventLineStylingManager.Instance.eventLineWidthStart_Highlight;
            lr.endWidth = EventLineStylingManager.Instance.eventLineWidthEnd_Highlight;
        }
    }

    public void Un_HighlightLinesOnHoverOverObject()
    {
        foreach (LineRenderer lr in lineRenderers)
        {
            lr.startColor = EventLineStylingManager.Instance.eventLineColorStart_Standard;
            lr.endColor = EventLineStylingManager.Instance.eventLineColorEnd_Standard;
            
            lr.startWidth = EventLineStylingManager.Instance.eventLineWidthStart_Standard;
            lr.endWidth = EventLineStylingManager.Instance.eventLineWidthEnd_Standard;
        }
    }

    public void HideLines()
    {
        foreach (LineRenderer lr in lineRenderers)
        {
            lr.startColor = new Color(0,0,0,0);
            lr.endColor = new Color(0,0,0,0);
            
            lr.startWidth = EventLineStylingManager.Instance.eventLineWidthStart_Standard;
            lr.endWidth = EventLineStylingManager.Instance.eventLineWidthEnd_Standard;
        }
    }

    //used for object interactable ticker
    public bool ContainsOnClickEvent()
    {
        foreach(SaveAndLoadLevel.Event e in events)
        {
            if(e.onAction == "OnClick")
            {
                return true;
            }
        }

        return false;
    }

    public void ResetEventsThatHaveTriggered()
    {
        foreach(SaveAndLoadLevel.Event e in events)
        {
            e.hasTriggered = false;
        }
    }
}
