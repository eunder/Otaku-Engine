using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEditStateMachineState_Idle : IItemEditStateMachine 
{
        bool entered = false;
        float timePassed = 0.0f;
        float timeEnd = 4.0f;

    public IItemEditStateMachine DoState(ItemEditStateMachine itemEditor)
    {
        if(entered == false)
        {
            OnEnter(itemEditor);
            entered = true;
        }


        OnStay(itemEditor);

           if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<Block>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateBlock;
            }
            if(itemEditor.currentItemType == CustomeItemType.TypeOfItem.poster)
            {
               OnExit(itemEditor);
               return itemEditor.ItemEditStateMachineStatePoster;
            }
            if(itemEditor.currentItemType == CustomeItemType.TypeOfItem.door)
            {
               OnExit(itemEditor);
               return itemEditor.ItemEditStateMachineStateDoor;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateDialogue;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateAudioSource;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<Counter>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateCounter;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<PlayerMoverEventComponent>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStatePlayerMover;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<StringEntity>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateString;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<State>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateState;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<PathNode>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStatePath;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<GlobalParameterPointerEntity>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateGlobalEntityPointer;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<Date>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateDate;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<LightEntity>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateLight;
            }
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<PrefabEntity>())
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStatePrefab;
            }
        return itemEditor.ItemEditStateMachineStateIdle;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
            itemEditor.noteCANVAS.SetActive(false);
            timePassed = 0.0f;

            itemEditor.depthLayer_currentlySelected = null;

            if(SaveAndLoadLevel.Instance.isLevelLoaded)
            PlayerMovementTypeKeySwitcher.Instance.EnableMovementBasedOnCurrentMovement();

            itemEditor.playerGameObject.SetActive(true);

            if(EscapeToggleToolBar.toolBarisOpened == false)
            {
            Cursor.lockState = CursorLockMode.Locked;
            itemEditor.mouseLooker.wheelPickerIsTurnedOn = false;
     //       itemEditor.playerInteractionStateMachine.enabled = true;
            }

    }

        void OnStay(ItemEditStateMachine itemEditor)
        {
                timePassed += Time.deltaTime;
        }

        void OnExit(ItemEditStateMachine itemEditor)
        {
                if(itemEditor.currentObjectEditing.GetComponent<Note>())
                {
                itemEditor.noteCANVAS.SetActive(true);
                itemEditor.noteInputField.text = itemEditor.currentObjectEditing.GetComponent<Note>().note;
                }


                entered = false; // reset
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                itemEditor.mouseLooker.wheelPickerIsTurnedOn = true;
               // itemEditor.playerInteractionStateMachine.enabled = false;
                itemEditor.openAndCloseMenu_audioSource.PlayOneShot(itemEditor.openMenu_audioClip, 1.0f);
                PlayerMovementBasic.Instance.enabled = false;
                PlayerMovementNoClip.Instance.enabled = false;

        }



}
