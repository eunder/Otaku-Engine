using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StencilDepthLayerUIButton_Holder : MonoBehaviour
{
    public GameObject currentAssignedDepthLayer;
    public void ClickedOnDepthLayerButton()
    {
        ItemEditStateMachine.Instance.frameLayer_currentlySelected = null;
        ItemEditStateMachine.Instance.frameLayerButton_currentlySelected = null;
        ItemEditStateMachine.Instance.depthLayer_currentlySelected = currentAssignedDepthLayer;
        ItemEditStateMachine.Instance.depthLayerButton_currentlySelected = gameObject;
        ItemEditStateMachine.Instance.RecalculateSliderValues();

        ItemEditStateMachine.Instance.Highlight_GeneralLayers();
    }

    //HIGHLIGHT OBJECT ON HOVER MECHANIC

    public void OnPointerEnter()
    {
        HighLightManager.Instance.UnHighLightObjects(ItemEditStateMachine.Instance.depthLayerListOfLayersCreated);

        HighLightManager.Instance.SetHighLightMaterial(HighLightManager.Instance.highlightMat_DepthLayer);
        HighLightManager.Instance.HighLightObject(currentAssignedDepthLayer, new Color(1,0,0,0.5f));
    }

    public void OnPointerExit()
    {
        HighLightManager.Instance.UnHighLightObject(currentAssignedDepthLayer);
    }

    //MOVING LAYER UP/DOWN MECHANIC
    public void ClickedToMoveLayerUp()
    {
        currentAssignedDepthLayer.GetComponent<Poster_DepthStencilFrame>().MoveLayerUpInList();
    }
    public void ClickedToMoveLayerDown()
    {
        currentAssignedDepthLayer.GetComponent<Poster_DepthStencilFrame>().MoveLayerDownInList();
    }


}
