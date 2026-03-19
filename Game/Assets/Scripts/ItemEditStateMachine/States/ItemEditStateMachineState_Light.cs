using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class ItemEditStateMachineState_Light: IItemEditStateMachine 
{
        bool entered = false;

    public IItemEditStateMachine DoState(ItemEditStateMachine itemEditor)
    {
        if(entered == false)
        {
            OnEnter(itemEditor);
            entered = true;
        }


        OnStay(itemEditor);

           if(itemEditor.currentObjectEditing == null)
            {
               OnExit(itemEditor);
               return itemEditor.ItemEditStateMachineStateIdle;
            }
        return itemEditor.ItemEditStateMachineStateLight;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {

        
                itemEditor.lightEditor_CANVAS.SetActive(true);


                itemEditor.lightEditor_lightType_Dropdown.value = GlobalUtilityFunctions.SetInputFieldValueFromString(itemEditor.lightEditor_lightType_Dropdown, itemEditor.currentObjectEditing.GetComponent<LightEntity>().lightType);
                itemEditor.lightEditor_range_InputField.text = itemEditor.currentObjectEditing.GetComponent<LightEntity>().range.ToString();
                itemEditor.lightEditor_strength_InputField.text = itemEditor.currentObjectEditing.GetComponent<LightEntity>().strength.ToString();
                itemEditor.lightEditor_spotAngle_InputField.text = itemEditor.currentObjectEditing.GetComponent<LightEntity>().spotAngle.ToString();
                itemEditor.lightEditor_shadowType_Dropdown.value = GlobalUtilityFunctions.SetInputFieldValueFromString(itemEditor.lightEditor_shadowType_Dropdown, itemEditor.currentObjectEditing.GetComponent<LightEntity>().shadowType);

                itemEditor.lightEditor_color_Image.color = itemEditor.currentObjectEditing.GetComponent<LightEntity>().color;


     }

        void OnStay(ItemEditStateMachine itemEditor)
        {

            if(itemEditor.currentObjectEditing)
            {
                itemEditor.currentObjectEditing.GetComponent<LightEntity>().lightType = itemEditor.lightEditor_lightType_Dropdown.options[itemEditor.lightEditor_lightType_Dropdown.value].text;
                itemEditor.currentObjectEditing.GetComponent<LightEntity>().range = float.Parse(itemEditor.lightEditor_range_InputField.text);
                itemEditor.currentObjectEditing.GetComponent<LightEntity>().strength = float.Parse(itemEditor.lightEditor_strength_InputField.text);
                itemEditor.currentObjectEditing.GetComponent<LightEntity>().spotAngle = float.Parse(itemEditor.lightEditor_spotAngle_InputField.text);
                itemEditor.currentObjectEditing.GetComponent<LightEntity>().shadowType = itemEditor.lightEditor_shadowType_Dropdown.options[itemEditor.lightEditor_shadowType_Dropdown.value].text;

                itemEditor.currentObjectEditing.GetComponent<LightEntity>().color = itemEditor.lightEditor_color_Image.color;
               


                itemEditor.currentObjectEditing.GetComponent<LightEntity>().UpdateParameters();
                itemEditor.currentObjectEditing.GetComponent<LightEntity>().UpdateColorOfWidget();
            }

                    
            if(Input.GetMouseButtonDown(1))
            {
                PlayerMovementTypeKeySwitcher.Instance.EnableMovementBasedOnCurrentMovement();
                itemEditor.mouseLooker.wheelPickerIsTurnedOn = false;
            }
            if(Input.GetMouseButtonUp(1))
            {
                PlayerMovementBasic.Instance.enabled = false;
                PlayerMovementNoClip.Instance.enabled = false;
                itemEditor.mouseLooker.wheelPickerIsTurnedOn = true;
            }



            //Notes
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<Note>())
            {
                itemEditor.currentObjectEditing.GetComponent<Note>().note = itemEditor.noteInputField.text;
            }
        }

        void OnExit(ItemEditStateMachine itemEditor)
        {
                entered = false; // reset
                itemEditor.lightEditor_CANVAS.SetActive(false);
        }

        





}
