using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameUIButton_FrameHolder : MonoBehaviour
{
    public GameObject currentAssignedFrame;
    public void ClickedOnFrameButton()
    {
        ItemEditStateMachine.Instance.frameLayer_currentlySelected = currentAssignedFrame;
        ItemEditStateMachine.Instance.frameLayerButton_currentlySelected = gameObject;

        ItemEditStateMachine.Instance.depthLayer_currentlySelected = null;
        ItemEditStateMachine.Instance.depthLayerButton_currentlySelected = null;


        ItemEditStateMachine.Instance.RecalculateSliderValues();
        ItemEditStateMachine.Instance.UpdateButtonColors();

        ItemEditStateMachine.Instance.Highlight_GeneralLayers();

    }
}
