using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Block : IItemEditStateMachine 
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
        return itemEditor.ItemEditStateMachineStateBlock;
    }
    
        void OnEnter(ItemEditStateMachine itemEditor)
     {
                itemEditor.blockEditor_CANVAS.SetActive(true);         
                itemEditor.blockEditor_isTrigger_TOGGLE.isOn = itemEditor.currentObjectEditing.GetComponentInChildren<MeshCollider>().isTrigger;
                itemEditor.blockIsScannable_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<IsScannable>().isScannable; 

                if(itemEditor.currentObjectEditing.GetComponent<GeneralObjectInfo>().ignorePlayer)
                {
                    itemEditor.blockIgnorePlayer_Toggle.isOn = true;
                }
                else if(itemEditor.currentObjectEditing.GetComponent<GeneralObjectInfo>().ignorePlayerClick)
                {
                    itemEditor.blockIgnorePlayerClick_Toggle.isOn = true; 
                }
                else
                {
                    itemEditor.blockIgnorePlayer_Toggle.isOn = false;
                    itemEditor.blockIgnorePlayerClick_Toggle.isOn = false; 
                }

     }

        void OnStay(ItemEditStateMachine itemEditor)
        {
                if(itemEditor.currentObjectEditing)
                {
                    itemEditor.currentObjectEditing.GetComponent<IsScannable>().isScannable = itemEditor.blockIsScannable_Toggle.isOn;


                    if(itemEditor.blockEditor_isTrigger_TOGGLE.isOn)
                    {
                        itemEditor.currentObjectEditing.GetComponentInChildren<MeshCollider>().convex = true;
                        itemEditor.currentObjectEditing.GetComponentInChildren<Collider>().isTrigger = true;
                    }
                    else    
                    {
                        itemEditor.currentObjectEditing.GetComponentInChildren<Collider>().isTrigger = false;
                        itemEditor.currentObjectEditing.GetComponentInChildren<MeshCollider>().convex = false;

                        itemEditor.currentObjectEditing.GetComponent<BlockFaceTextureUVProperties>().SetLayerBasedOnBlockMaterial();
                    }

                      itemEditor.currentObjectEditing.GetComponent<GeneralObjectInfo>().isTrigger = itemEditor.blockEditor_isTrigger_TOGGLE.isOn; 

                      itemEditor.currentObjectEditing.GetComponent<GeneralObjectInfo>().ignorePlayer = itemEditor.blockIgnorePlayer_Toggle.isOn; 
                      itemEditor.currentObjectEditing.GetComponent<GeneralObjectInfo>().ignorePlayerClick = itemEditor.blockIgnorePlayerClick_Toggle.isOn; 

                      itemEditor.currentObjectEditing.GetComponent<GeneralObjectInfo>().UpdateGeneralObjectLayerProperties();

                }



                if(Input.GetMouseButtonDown(1))
                {
                    itemEditor.mouseLooker.wheelPickerIsTurnedOn = false;
                }
                if(Input.GetMouseButtonUp(1))
                {
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
                itemEditor.blockEditor_CANVAS.SetActive(false);  
        }



}
