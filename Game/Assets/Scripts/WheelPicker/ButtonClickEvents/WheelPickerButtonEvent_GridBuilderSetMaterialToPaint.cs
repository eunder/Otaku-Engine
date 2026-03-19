using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelPickerButtonEvent_GridBuilderSetMaterialToPaint : MonoBehaviour
{
    public Material materialToPaint;
    GridBuilderStateMachine gridBuilderStateMachine;

    void Start()
    {
        gridBuilderStateMachine = GameObject.Find("GridBuilderStateMachine").GetComponent<GridBuilderStateMachine>();

        if(materialToPaint.mainTexture != null)
        {
        GetComponent<RawImage>().texture = materialToPaint.mainTexture;
        }
        else
        {
        GetComponent<RawImage>().material = materialToPaint;
        }
    }

    public void SetBlockMaterialToPaint()
    {
        gridBuilderStateMachine.materialToApply = materialToPaint;
        gridBuilderStateMachine.materialNameToApply = materialToPaint.name;
    }
}
