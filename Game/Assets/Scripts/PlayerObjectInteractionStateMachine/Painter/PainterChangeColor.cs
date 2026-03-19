using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PainterChangeColor : MonoBehaviour
{

    private static PainterChangeColor _instance;
    public static PainterChangeColor Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }




    public Image buttonImage;

    public bool colorWheelisOpen = false;
    public string shaderColorProperty = "";
    public UIColorPickerLogic colorpicker;
    public PlayerObjectInteractionStateMachine playerInteractionStateMachine;

    public void ColorButtonPressed(Image buttImage)
    {
        buttonImage = buttImage;
        shaderColorProperty = "_TopColor";
        OpenColorWheel();
    }
   
    public void OpenColorWheel()
    {
        colorpicker.InitialColor = playerInteractionStateMachine.painterColor;
        colorpicker.color = new Color(playerInteractionStateMachine.painterColor.r, playerInteractionStateMachine.painterColor.g, playerInteractionStateMachine.painterColor.b, playerInteractionStateMachine.painterColor.a);

        colorpicker.ActivateColorPicker();
        colorWheelisOpen = true;
    }

    public void ApplyMaterialToPainter(Material materialResourceName)
    {
            if(playerInteractionStateMachine.painter_head_renderer != null)
            {
            playerInteractionStateMachine.painter_head_renderer.sharedMaterial = materialResourceName;
            playerInteractionStateMachine.painter_head_renderer.sharedMaterial.SetColor("_Color",playerInteractionStateMachine.painterColor);
            playerInteractionStateMachine.painterCurrentMaterial = materialResourceName;
            }
    }

    //technically this should not be here, this class is for changing the COLOR, not the alpha
    public void AlphaSlider (float value)
    {
        playerInteractionStateMachine.colorpicker.color = new Color(playerInteractionStateMachine.colorpicker.color.r, playerInteractionStateMachine.colorpicker.color.g, playerInteractionStateMachine.colorpicker.color.b, value);
        playerInteractionStateMachine.painterColor = playerInteractionStateMachine.colorpicker.color;
        playerInteractionStateMachine.painter_head_renderer.material.SetColor("_Color",playerInteractionStateMachine.colorpicker.color);

    }


    void Update()
    {
        if(Input.GetMouseButtonUp(0)) //bootleg soltion, find better logical way
        {
            buttonImage = null;
        }
        
        if(buttonImage)
        {
              buttonImage.color = new Color(colorpicker.color.r, colorpicker.color.g, colorpicker.color.b, 1.0f); //makes sure the button transparency dosnt change

                playerInteractionStateMachine.painterColor = playerInteractionStateMachine.colorpicker.color;

                //assigns color to painter head
                playerInteractionStateMachine.painter_head_renderer.material.SetColor("_Color",playerInteractionStateMachine.colorpicker.color);

        }
    }
}
