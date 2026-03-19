using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Prefab : IItemEditStateMachine 
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
        return itemEditor.ItemEditStateMachineStatePrefab;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.prefab_CANVAS.SetActive(true);

         
                  itemEditor.prefab_mapToLoad_InputField.text = itemEditor.currentObjectEditing.GetComponent<PrefabEntity>().mapToLoad;
                  itemEditor.prefab_loadOnStart_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PrefabEntity>().loadOnStart;
     }

        void OnStay(ItemEditStateMachine itemEditor)
        {

                if(itemEditor.currentObjectEditing)
                {
                  itemEditor.currentObjectEditing.GetComponent<PrefabEntity>().mapToLoad = itemEditor.prefab_mapToLoad_InputField.text;
                  itemEditor.currentObjectEditing.GetComponent<PrefabEntity>().loadOnStart = itemEditor.prefab_loadOnStart_Toggle.isOn;
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
                itemEditor.prefab_CANVAS.SetActive(false);
        }



}
