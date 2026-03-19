using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Poster : IItemEditStateMachine 
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

            if(itemEditor.currentObjectEditing == null)
            {
               OnExit(itemEditor);
               return itemEditor.ItemEditStateMachineStateIdle;
            }
        return itemEditor.ItemEditStateMachineStatePoster;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
            itemEditor.toolTipText.text = " | | RIght Mouse: <color=green>Move Player <color=white>| |";

            itemEditor.frameLayer_currentlySelected = null;
            timePassed = 0.0f;
            itemEditor.posterEditor_CANVAS.SetActive(true);


            itemEditor.posterAreaResizePreview.SetActive(false);
            itemEditor.posterAreaResizePreview.transform.position = itemEditor.currentObjectEditing.transform.position;
            itemEditor.posterAreaResizePreview.transform.rotation = itemEditor.currentObjectEditing.transform.rotation;
            itemEditor.posterAreaResizePreview.GetComponent<PosterFrameCreator_AreaPreview>().posterMeshCreator = itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>();

            //makes sure the slider for the poster matches the current objects poster thickness
           // itemEditor.frame_sizeSlider.value = itemEditor.currentObjectEditing.GetComponentInChildren<PosterMeshCreator_BorderFrame>().frameThickness;
            itemEditor.width_InputField.text = itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().box.x.ToString();
            itemEditor.height_InputField.text = itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().box.y.ToString();
            itemEditor.billboard_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PosterBillboard>().useBillboard;
            itemEditor.billboard_Character_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PosterBillboard>().useCharacterBillboard;


        if(itemEditor.frameLayer_currentlySelected != null)
        {
            itemEditor.frame_heightSlider.value = itemEditor.frame_heightSlider.minValue;
            itemEditor.frame_widthSlider.value = itemEditor.frame_widthSlider.minValue;
            itemEditor.frame_depthSlider.value = itemEditor.frame_depthSlider.minValue;
        }

 

        
            itemEditor.posterURL.text = itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().urlFilePath;


            itemEditor.UpdateFrameList();
            itemEditor.UpdateDepthLayerList();


            //  enable/disable media blocker
            if(itemEditor.currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count <= 0)
            {
                itemEditor.depthLayersBeingUsed_Blocker.SetActive(false);
            }
            else
            {
                itemEditor.depthLayersBeingUsed_Blocker.SetActive(true);
            }

            

        //update view settings UI
        itemEditor.posterViewSettings_ScrollingMode_DropDown.value = GlobalUtilityFunctions.SetInputFieldValueFromString(itemEditor.posterViewSettings_ScrollingMode_DropDown, itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().scrollingMode);
        itemEditor.posterViewSettings_AlignmentMode_DropDown.value = GlobalUtilityFunctions.SetInputFieldValueFromString(itemEditor.posterViewSettings_AlignmentMode_DropDown, itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().alignmentMode);
        itemEditor.posterViewSettings_ZoomForcedOffset_Slider.value = itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().zoomForcedOffset;
        itemEditor.posterViewSettings_RotationEffectAmount_Slider.value = itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().rotationEffectAmount;
        itemEditor.posterViewSettings_CanZoom_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().canZoom;
        itemEditor.posterViewSettings_ExtraBorder_Slider.value = itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().extraBorder;
        itemEditor.posterViewSettings_InverseLook_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().inverseLook;
    
    
        //update colorkey settings
        itemEditor.posterColorKeySettings_ColorKey_Image.color = itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().colorKey;
        itemEditor.posterColorKeySettings_Threshold_Slider.value = itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().threshold;
        itemEditor.posterColorKeySettings_transparencyThreshold_Slider.value = itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().transparencyThreshold;
        itemEditor.posterColorKeySettings_spillCorrection_Slider.value = itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().spillCorrection;
   
        //update texture filtering settings
        itemEditor.posterTextureFilteringSettings_enableTextureFiltering_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().textureFiltering;

        //update texture scrolling settings
        itemEditor.posterTextureScrollingX_InputField.text = itemEditor.currentObjectEditing.GetComponent<PosterTextureScroll>().scrollSpeed.x.ToString();
        itemEditor.posterTextureScrollingY_InputField.text = itemEditor.currentObjectEditing.GetComponent<PosterTextureScroll>().scrollSpeed.y.ToString();


        //update footstep sound settings
        itemEditor.posterFootstepPath_InputField.text = itemEditor.currentObjectEditing.GetComponent<PosterFootstepSound>().footstepSoundPath;


        //update xray settings
        itemEditor.posterXraySize_InputField.text = itemEditor.currentObjectEditing.GetComponent<PosterXraySettings>().xray_Size.ToString();
        itemEditor.posterXrayTransparency_InputField.text = itemEditor.currentObjectEditing.GetComponent<PosterXraySettings>().xray_Transparency.ToString();
        itemEditor.posterXraySoftness_InputField.text = itemEditor.currentObjectEditing.GetComponent<PosterXraySettings>().xray_Softness.ToString();


        itemEditor.currentObjectEditing.GetComponent<PosterXraySettings>().UpdateShaderWithXraySettings();

        itemEditor.posterXrayIsScannable_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<IsScannable>().isScannable; 

        //enable xray
        XrayRaycast.Instance.enabled = true;

    }
        async void OnStay(ItemEditStateMachine itemEditor)
        {
        if(itemEditor.currentObjectEditing)
        {

       
            itemEditor.currentObjectEditing.GetComponent<PosterBillboard>().useBillboard = itemEditor.billboard_Toggle.isOn;
            itemEditor.currentObjectEditing.GetComponent<PosterBillboard>().useCharacterBillboard = itemEditor.billboard_Character_Toggle.isOn;



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


                timePassed += Time.deltaTime;
            foreach (GameObject frame in itemEditor.currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList)
            {
                frame.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
            } 
            itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().box.x = float.Parse(itemEditor.width_InputField.text);
            itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().box.y = float.Parse(itemEditor.height_InputField.text);


        //frame layer window show and close
        if(itemEditor.frameLayer_currentlySelected != null)
        {
            
        if(Input.GetMouseButton(0) && itemEditor.colorpicker.gameObject.activeSelf) // click to set color.   the && prevents the color from being set from just clicking down...
        {
         itemEditor.frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimColors[itemEditor.posterEdgeIndex] = itemEditor.colorpicker.color;
        }
        
        //updates values
         itemEditor.frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimLuminance[0] = itemEditor.frame_luminanceSlider_outer.value;
         itemEditor.frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimLuminance[1] = itemEditor.frame_luminanceSlider_inner.value;
         itemEditor.frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();

            itemEditor.frameProperties_Window.SetActive(true);
        }
        else
        {
            itemEditor.frameProperties_Window.SetActive(false);
        }



        //depth layer window show and close
        if(itemEditor.depthLayer_currentlySelected != null)
        {
            itemEditor.depthLayerProperties_Window.SetActive(true);
        }
        else
        {
            itemEditor.depthLayerProperties_Window.SetActive(false);
        }

            //updaint color key color
            itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().colorKey = itemEditor.posterColorKeySettings_ColorKey_Image.color;
            itemEditor.currentObjectEditing.GetComponent<Renderer>().sharedMaterial.SetColor("_ColorKey", itemEditor.posterColorKeySettings_ColorKey_Image.color);

            //color key
            itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().colorKey = itemEditor.posterColorKeySettings_ColorKey_Image.color;
            itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().threshold =  itemEditor.posterColorKeySettings_Threshold_Slider.value;
            itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().transparencyThreshold = itemEditor.posterColorKeySettings_transparencyThreshold_Slider.value;
            itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().spillCorrection = itemEditor.posterColorKeySettings_spillCorrection_Slider.value;


            itemEditor.currentObjectEditing.GetComponent<PosterColorKeySettings>().ApplyColorKeySettingsToShader();




         if(itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().urlIsValid == false)
         {
        itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().width = itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().box.x;
        itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().height = itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().box.y;
        itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().image.Set(itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().width, itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().height);
         }
         

            //update view settings
            itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().scrollingMode = itemEditor.posterViewSettings_ScrollingMode_DropDown.options[itemEditor.posterViewSettings_ScrollingMode_DropDown.value].text;
            itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().alignmentMode = itemEditor.posterViewSettings_AlignmentMode_DropDown.options[itemEditor.posterViewSettings_AlignmentMode_DropDown.value].text;
            itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().zoomForcedOffset = itemEditor.posterViewSettings_ZoomForcedOffset_Slider.value;
            itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().rotationEffectAmount = itemEditor.posterViewSettings_RotationEffectAmount_Slider.value;
            itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().canZoom = itemEditor.posterViewSettings_CanZoom_Toggle.isOn;
            itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().extraBorder = itemEditor.posterViewSettings_ExtraBorder_Slider.value;
            itemEditor.currentObjectEditing.GetComponent<PosterViewSettings>().inverseLook = itemEditor.posterViewSettings_InverseLook_Toggle.isOn;


            //is toggable settings
            itemEditor.currentObjectEditing.GetComponent<IsScannable>().isScannable = itemEditor.posterXrayIsScannable_Toggle.isOn;


           
           //Notes
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<Note>())
            {
                itemEditor.currentObjectEditing.GetComponent<Note>().note = itemEditor.noteInputField.text;
            }

            if(itemEditor.currentObjectEditing.GetComponent<MeshCollider>().sharedMesh == null)
            {
                itemEditor.currentObjectEditing.GetComponent<PosterMeshCreator>().RebuildMeshCollider();
            }

            
 }
        }

        void OnExit(ItemEditStateMachine itemEditor)
        {
                //be sure to remove any frame highlighting
                HighLightManager.Instance.UnHighLightObjects(ItemEditStateMachine.Instance.depthLayerListOfLayersCreated);

                entered = false; // reset
                itemEditor.posterEditor_CANVAS.SetActive(false);

                itemEditor.posterAreaResizePreview.SetActive(false);

            itemEditor.openAndCloseMenu_audioSource.PlayOneShot(itemEditor.closeMenu_audioClip, 1.0f);


            //makes sure poster view preview isnt left on
            if(itemEditor.playerInteractionStateMachine.currentState == itemEditor.playerInteractionStateMachine.PlayerObjectInteractionStateViewingFrame)
            {
                itemEditor.playerInteractionStateMachine.LeavePosterViewingMode();

            }


                itemEditor.shaderPickingWindow.SetActive(false);

                //enable xray
                XrayRaycast.Instance.enabled = false;
        }



}
