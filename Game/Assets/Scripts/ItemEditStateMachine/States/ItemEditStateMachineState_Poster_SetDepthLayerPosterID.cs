using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Poster_SetDepthLayerPosterID : IItemEditStateMachine 
{
        bool entered = false;

        bool exitEvent = false;


    public IItemEditStateMachine DoState(ItemEditStateMachine itemEditor)
    {
        if(entered == false)
        {
            OnEnter(itemEditor);
            entered = true;
        }


        OnStay(itemEditor);

           if(exitEvent)
            {
               OnExit(itemEditor);
               return itemEditor.ItemEditStateMachineStatePoster;
            }
        return itemEditor.ItemEditStateMachineStatePosterSetDepthLayerPosterID;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {                
                itemEditor.posterEditor_CANVAS.SetActive(false);

                itemEditor.posterEditorSetDepthLayerPosterID_CANVAS.SetActive(true);

                Cursor.lockState = CursorLockMode.Locked;
                SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;


                itemEditor.playerGameObject.SetActive(true);
                
                PlayerMovementTypeKeySwitcher.Instance.EnableMovementBasedOnCurrentMovement();

    }

        void OnStay(ItemEditStateMachine itemEditor)
        {

            if(Input.GetMouseButtonDown(0))
            {

                Ray ray = PlayerObjectInteractionStateMachine.Instance.playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask))
                {  
                    if(hit.transform.GetComponent<PosterMeshCreator>())
                    {
                        if(hit.transform.gameObject != itemEditor.currentObjectEditing)
                        {
                        itemEditor.depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().IdOfPosterToReference = hit.transform.name;
                        itemEditor.depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().SetMaterialFromReferencedPosterID_Wrapper();
                        itemEditor.depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();

                        //make sure to assign the depth layer to the poster mesh creator(so that posters know what depth layer they are assigned to)
                        hit.transform.GetComponent<PosterMeshCreator>().currentDepthLayerAssignedTo = itemEditor.depthLayer_currentlySelected;

                        exitEvent = true;
                        }
                    }
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                exitEvent = true;
            }

        }

        void OnExit(ItemEditStateMachine itemEditor)
        {
                entered = false; // reset
                exitEvent = false;

                itemEditor.posterEditor_CANVAS.SetActive(true);
                itemEditor.posterEditorSetDepthLayerPosterID_CANVAS.SetActive(false);


                Cursor.lockState = CursorLockMode.None;
                SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;
        }



}
