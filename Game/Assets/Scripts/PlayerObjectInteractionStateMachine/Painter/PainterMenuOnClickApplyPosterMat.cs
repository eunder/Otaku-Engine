using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainterMenuOnClickApplyPosterMat : MonoBehaviour
{
    public GameObject poster;
    public void _PainterMenuOnClickApplyPosterMat()
    {   
        PainterChangeColor.Instance.ApplyMaterialToPainter(poster.GetComponent<Renderer>().sharedMaterial);

        PlayerObjectInteractionStateMachine.Instance.UpdatePainterCurrentMaterialHelpfulData();
    }

}
