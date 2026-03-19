using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using SFB;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class PlayerObjectInteractionState_Equipped_Painter : IPlayerObjectInteractionState 
{
        bool entered = false;
        bool clickedToExit = false;
        string filePathToSaveImages;

        bool pressedMouseButtonDownOnce = false;

        bool wholeBlockPaintModeTurnedOn = false;


        float shift_hold = 0f;

    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }

        if(clickedToExit == true)
        {
            OnExit(playerObjectInteraction);
            return playerObjectInteraction.PlayerObjectInteractionStateIdle;
        }


        OnStay(playerObjectInteraction);

        return playerObjectInteraction.PlayerObjectInteractionStateEquippedPainter;
    }


    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        playerObjectInteraction.painter_model.SetActive(true);
        playerObjectInteraction.painter_head_renderer.material = playerObjectInteraction.painterCurrentMaterial;
        playerObjectInteraction.painter_head_renderer.material.SetColor("_Color",playerObjectInteraction.painterColor);
        playerObjectInteraction.toolTipText.text = "Left Mouse: Paint <color=white>| RIght Mouse: Copy Material <color=white> | Shift(Hold): Material List | Shift: Switch Mode | C: Unlock Mouse | <color=red>Q: Go Back";
        playerObjectInteraction.blockQuadHighLighter.gameObject.SetActive(true);

        playerObjectInteraction.UpdatePainterCurrentMaterialHelpfulData();

        EvaluatePainterModeIcon(playerObjectInteraction);
    }

        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {

        if(Input.GetMouseButtonDown(0) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {

            //prevent user from painting in the xray viewmode
            if(ObjectVisibilityViewManager.Instance.currentViewMode == 2)
            {
                UINotificationHandler.Instance.SpawnNotification("Cannot paint in this view mode!");
                pressedMouseButtonDownOnce = false;
            }
            else
            {
                pressedMouseButtonDownOnce = true;
            }
        }

        if(pressedMouseButtonDownOnce == true)
        {
      if(Input.GetMouseButton(0) && playerObjectInteraction.painterSettings_Canvas.activeSelf == false && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
           {
                Ray ray = playerObjectInteraction.gameObject.transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.painter_collidableLayers_layerMask))
                {  
                    if(playerObjectInteraction.painterCurrentMaterial != null) // prevents errors when the painter head material is null(deleted)
                    {

                        //on paint poster
                        if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>()) // && playerObjectInteraction.painterCurrentMaterial != playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<Renderer>().sharedMaterial
                        {
                                //dont paint over posters with depth stencils
                                if(playerObjectInteraction.objectHitInfo.transform.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count <= 0)
                                {
                                    PaintPoster(playerObjectInteraction);
                                }
                            
                        }   


                        //on paint block
                        if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>()) // && playerObjectInteraction.painterCurrentMaterial != playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<Renderer>().sharedMaterial
                        {
                            if(wholeBlockPaintModeTurnedOn)
                            {
                                PaintEntireBlock(playerObjectInteraction);
                            }
                            else
                            {
                                PaintBlockFace(playerObjectInteraction);
                            }
                        }
                    }
                }
           }
        }

        //color picking
        if(Input.GetMouseButtonDown(1) && playerObjectInteraction.painterSettings_Canvas.activeSelf == false && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {
            //prevent user from painting in the xray viewmode
            if(ObjectVisibilityViewManager.Instance.currentViewMode == 2)
            {
                UINotificationHandler.Instance.SpawnNotification("Cannot paint in this view mode!");
            }
            else
            {
                Ray ray = playerObjectInteraction.gameObject.transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.painter_collidableLayers_layerMask))
                {
                    //on pick poster
                    if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>())
                    {
                        //make sure players cant copy depth layer stuff...
                        if(playerObjectInteraction.objectHitInfo.transform.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count <= 0 
                        && playerObjectInteraction.objectHitInfo.transform.GetComponent<Renderer>().sharedMaterial)
                        {
                            playerObjectInteraction.painterColor = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>()._color;
                            playerObjectInteraction.painterCurrentMaterial = playerObjectInteraction.objectHitInfo.transform.GetComponent<Renderer>().sharedMaterial;
                            playerObjectInteraction.painterCurrentMaterial.name = playerObjectInteraction.objectHitInfo.transform.GetComponent<Renderer>().sharedMaterial.name;
                           
                            //set UVs to dedfault(because posters cannot have UVs adjusted)
                            playerObjectInteraction.UVOffSet_Store = new Vector2 (0.0f,0.0f);
                            playerObjectInteraction.UVScale_Store =new Vector2 (1.0f,1.0f);
                            playerObjectInteraction.UVRotation_Store = 0f;


                            playerObjectInteraction.painter_head_renderer.material = playerObjectInteraction.objectHitInfo.transform.GetComponent<Renderer>().sharedMaterial;
                            playerObjectInteraction.painter_head_renderer.material.SetColor("_Color",playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>()._color);

                
                            //sfx
                            playerObjectInteraction.copySplatter_Particle.Play();
                            playerObjectInteraction.copySplatter_Particle.transform.gameObject.GetComponent<ParticleSystemRenderer>().material = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;
                            playerObjectInteraction.copySplatter_Particle.transform.gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color",playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>()._color);

                            playerObjectInteraction.paintRoll_Anim.Play("Dip", -1, 0f);
                            playerObjectInteraction.paintRollerDip_AudioSource.Play();

                            playerObjectInteraction.UpdatePainterCurrentMaterialHelpfulData();
                        }
                    }

                    //on pick block
                    else if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().GetMaterialBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting) != null) // prevents errors when clicking on block with deleted material
                    {
                        if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>()) // && playerObjectInteraction.painterCurrentMaterial != playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<Renderer>().material
                        {
                            playerObjectInteraction.painterColor = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().GetMaterialColorBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting);
                            playerObjectInteraction.painterCurrentMaterial = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().GetMaterialBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting);
                            playerObjectInteraction.painterCurrentMaterial.name = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().GetMatNameBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting);

                            playerObjectInteraction.UVOffSet_Store = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting];
                            playerObjectInteraction.UVScale_Store = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting];
                            playerObjectInteraction.UVRotation_Store = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVRotation[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting];

                            playerObjectInteraction.painter_head_renderer.material = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().GetMaterialBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting);
                            playerObjectInteraction.painter_head_renderer.material.SetColor("_Color",playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().GetMaterialColorBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting));


                            //sfx
                            playerObjectInteraction.copySplatter_Particle.Play();
                            playerObjectInteraction.copySplatter_Particle.transform.gameObject.GetComponent<ParticleSystemRenderer>().material = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().GetMaterialBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting);
                            playerObjectInteraction.copySplatter_Particle.transform.gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color",playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().GetMaterialColorBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting));

                            playerObjectInteraction.paintRoll_Anim.Play("Dip", -1, 0f);
                            playerObjectInteraction.paintRollerDip_AudioSource.Play();

                            playerObjectInteraction.UpdatePainterCurrentMaterialHelpfulData();
                        }
                    }
                }
            }
        }
        
              if(Input.GetKeyDown(KeyCode.Q) && playerObjectInteraction.painterSettings_Canvas.activeSelf == false && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
            {
                clickedToExit = true;  
            }


            //HOLDING SHIFT MECHANIC
            //------------------------

            //switch painter mode (quick shift press)
            if(Input.GetKeyUp(KeyCode.LeftShift) && playerObjectInteraction.painterSettings_Canvas.activeSelf == false && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
            {
                if(shift_hold <= ConfigFileHandler.toolHoldAndTapAmount)
                {
                    if(wholeBlockPaintModeTurnedOn)
                    {
                        UINotificationHandler.Instance.SpawnNotification("Enabled: Face Mode!", UINotificationHandler.NotificationStateType.ping);
                        wholeBlockPaintModeTurnedOn = false;
                        EvaluatePainterModeIcon(playerObjectInteraction);
                    }
                    else
                    {
                        UINotificationHandler.Instance.SpawnNotification("Enabled: Whole Block painting mode!", UINotificationHandler.NotificationStateType.ping);
                        wholeBlockPaintModeTurnedOn = true;
                        EvaluatePainterModeIcon(playerObjectInteraction);
                    }
                }
            }

            //hold shift
           if(Input.GetKey(KeyCode.LeftShift) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
            {
              shift_hold += Time.deltaTime;
            }
            else
            {
              shift_hold = 0.0f;
            }


            //open material menu (long shift press)
            if(shift_hold >= ConfigFileHandler.toolHoldAndTapAmount && playerObjectInteraction.painterSettings_Canvas.activeSelf == false)
            {                
                shift_hold = 0f; //used to prevent message spam on hold

                playerObjectInteraction.colorpicker.color = new Color(playerObjectInteraction.buttonImage.color.r, playerObjectInteraction.buttonImage.color.g, playerObjectInteraction.buttonImage.color.b, playerObjectInteraction.alphaSlider.value); //sets the color picker color to prevent errors on changing alpha(it would change to black)
                playerObjectInteraction.painterSettings_Canvas.SetActive(true);
                playerObjectInteraction.buttonImage.color = new Color(playerObjectInteraction.painterColor.r, playerObjectInteraction.painterColor.g, playerObjectInteraction.painterColor.b, 1.0f);
                playerObjectInteraction.alphaSlider.value = playerObjectInteraction.painterColor.a;
                FillPosterMaterialList(playerObjectInteraction);
            }


            if(playerObjectInteraction.painterSettings_Canvas.activeSelf == true)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                playerObjectInteraction.mouseLooker.enabled = false;
                playerObjectInteraction.blockQuadHighLighter.gameObject.SetActive(false);
            }
            else if(Input.GetKey(KeyCode.C))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                playerObjectInteraction.mouseLooker.enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                playerObjectInteraction.mouseLooker.enabled = true;
                playerObjectInteraction.blockQuadHighLighter.gameObject.SetActive(true);
            }

            if(EscapeToggleToolBar.toolBarisOpened)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }




        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                entered = false; // reset
                playerObjectInteraction.mouseLooker.wheelPickerIsTurnedOn = false;
                playerObjectInteraction.blockQuadHighLighter.gameObject.SetActive(false);

                playerObjectInteraction.pickedUpObject = false;
                clickedToExit = false;
                playerObjectInteraction.painter_model.SetActive(false);

                pressedMouseButtonDownOnce = false;

                  Cursor.lockState = CursorLockMode.Locked;
                 playerObjectInteraction.mouseLooker.enabled = true;

        }


    void OnPickFolderPath() {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", Application.dataPath, true);
        if (paths.Length > 0) {
            filePathToSaveImages = paths[0] + "\\";
            Debug.Log("folder path picked: " + filePathToSaveImages);
        }
    }


    //CHECK IF PROPERTIES MATCH MECHANIC (to make sure the same face/block does not get painted again!)


    bool CheckIfFaceUVsMatch(PlayerObjectInteractionStateMachine playerObjectInteraction, int faceIndex)
    {
        if(playerObjectInteraction.UVOffSet_Store != playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[faceIndex] 
        || playerObjectInteraction.UVScale_Store != playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[faceIndex]
        || playerObjectInteraction.UVRotation_Store != playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVRotation[faceIndex]  )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckIfFaceMaterialsMatch(PlayerObjectInteractionStateMachine playerObjectInteraction, int faceIndex)
    {
        if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().GetMatNameBasedOnIndex(faceIndex) != playerObjectInteraction.painterCurrentMaterial.name)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CheckIfFaceColorsMatch(PlayerObjectInteractionStateMachine playerObjectInteraction, int faceIndex)
    {
        if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().GetMaterialColorBasedOnIndex(faceIndex) != playerObjectInteraction.painterColor)
        {
            return true;
        }
        else
        {
            return false;
        }
    } 



    bool CheckIfBlockFace_HasAnyParametersThatDontMatch(PlayerObjectInteractionStateMachine playerObjectInteraction ,int faceIndex)
    {
        if(
           playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().GetMatNameBasedOnIndex(faceIndex) != playerObjectInteraction.painterCurrentMaterial.name 
        || playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().GetMaterialColorBasedOnIndex(faceIndex) != playerObjectInteraction.painterColor
        || CheckIfFaceUVsMatch(playerObjectInteraction, faceIndex)
        )
        {
            return true;
        }
            return false;
    }

    bool CheckIfPoster_HasAnyParametersThatDontMatch(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial == null)
        {
            return true;
        }

        if(
           playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.name != playerObjectInteraction.painterCurrentMaterial.name 
        || playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>()._color != playerObjectInteraction.painterColor
        )
        {
            return true;
        }
            return false;
    }


    bool CheckIfEntireBlock_MatchesPainterProperties(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(CheckIfBlockFace_HasAnyParametersThatDontMatch(playerObjectInteraction, 0))
        {
            return true;
        }
        if(CheckIfBlockFace_HasAnyParametersThatDontMatch(playerObjectInteraction, 1))
        {
            return true;
        }
        if(CheckIfBlockFace_HasAnyParametersThatDontMatch(playerObjectInteraction, 2))
        {
            return true;
        }
        if(CheckIfBlockFace_HasAnyParametersThatDontMatch(playerObjectInteraction, 3))
        {
            return true;
        }
        if(CheckIfBlockFace_HasAnyParametersThatDontMatch(playerObjectInteraction, 4))
        {
            return true;
        }
        if(CheckIfBlockFace_HasAnyParametersThatDontMatch(playerObjectInteraction, 5))
        {
            return true;
        }

        return false;
    }

bool CheckIfEntireBlock_MatchesMaterials(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(CheckIfFaceMaterialsMatch(playerObjectInteraction, 0))
        {
            return true;
        }
        if(CheckIfFaceMaterialsMatch(playerObjectInteraction, 1))
        {
            return true;
        }
        if(CheckIfFaceMaterialsMatch(playerObjectInteraction, 2))
        {
            return true;
        }
        if(CheckIfFaceMaterialsMatch(playerObjectInteraction, 3))
        {
            return true;
        }
        if(CheckIfFaceMaterialsMatch(playerObjectInteraction, 4))
        {
            return true;
        }
        if(CheckIfFaceMaterialsMatch(playerObjectInteraction, 5))
        {
            return true;
        }

        return false;
    }

bool CheckIfEntireBlock_MatchesColors(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(CheckIfFaceColorsMatch(playerObjectInteraction, 0))
        {
            return true;
        }
        if(CheckIfFaceColorsMatch(playerObjectInteraction, 1))
        {
            return true;
        }
        if(CheckIfFaceColorsMatch(playerObjectInteraction, 2))
        {
            return true;
        }
        if(CheckIfFaceColorsMatch(playerObjectInteraction, 3))
        {
            return true;
        }
        if(CheckIfFaceColorsMatch(playerObjectInteraction, 4))
        {
            return true;
        }
        if(CheckIfFaceColorsMatch(playerObjectInteraction, 5))
        {
            return true;
        }

        return false;
    }

bool CheckIfEntireBlock_MatchesUVs(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(CheckIfFaceUVsMatch(playerObjectInteraction, 0))
        {
            return true;
        }
        if(CheckIfFaceUVsMatch(playerObjectInteraction, 1))
        {
            return true;
        }
        if(CheckIfFaceUVsMatch(playerObjectInteraction, 2))
        {
            return true;
        }
        if(CheckIfFaceUVsMatch(playerObjectInteraction, 3))
        {
            return true;
        }
        if(CheckIfFaceUVsMatch(playerObjectInteraction, 4))
        {
            return true;
        }
        if(CheckIfFaceUVsMatch(playerObjectInteraction, 5))
        {
            return true;
        }

        return false;
    }


    void EvaluatePainterModeIcon(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(wholeBlockPaintModeTurnedOn)
        {
            playerObjectInteraction.painter_modeGUI_face.SetActive(false);
            playerObjectInteraction.painter_modeGUI_whole.SetActive(true);
        }
        else
        {
            playerObjectInteraction.painter_modeGUI_face.SetActive(true);
            playerObjectInteraction.painter_modeGUI_whole.SetActive(false);
        }
    }





    void PlayPainterSFX(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
                            playerObjectInteraction.paintRoll_Anim.Play("Armature|Paint", -1, 0f);
                            playerObjectInteraction.paintRollerPaint_AudioSource.Play();

                            float blockVolume = 1;
                            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<BlockFaceTextureUVProperties>())
                            {
                                blockVolume = playerObjectInteraction.objectHitInfo.transform.GetComponent<BlockFaceTextureUVProperties>().GetBlockVolume();
                            }

                            GameObject part = GameObject.Instantiate(playerObjectInteraction.painterRollPaint_Particle, playerObjectInteraction.objectHitInfo.point, Quaternion.identity);
                            part.GetComponent<ParticleSystem>().startSize  = playerObjectInteraction.painterParticleSize_AnimationCurve.Evaluate(blockVolume);
                            part.GetComponent<ParticleSystemRenderer>().material = playerObjectInteraction.painterCurrentMaterial;
                            part.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", playerObjectInteraction.painterColor);
   
                            playerObjectInteraction.UpdatePainterCurrentMaterialHelpfulData();
    }



    void FillPosterMaterialList(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        //clear mat list
        foreach(Transform child in playerObjectInteraction.customeMaterialList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }



        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            //if it is not a GUID... then expect it to be a source material
            System.Guid guid;
            if(System.Guid.TryParse(poster.GetComponent<PosterMeshCreator>().urlFilePath, out guid) == false)
            {
                            //create custom materials and put them in window
                    GameObject material = GameObject.Instantiate(playerObjectInteraction.materialMenuItem_prefab ,new Vector3(0,0,0), Quaternion.identity);
                    material.transform.SetParent(playerObjectInteraction.customeMaterialList.transform);
                    material.GetComponentInChildren<TextMeshProUGUI>().text = poster.GetComponent<Note>().note;
                    material.GetComponentInChildren<RawImage>().texture = poster.GetComponent<GeneralObjectInfo>().baseMatList[0].mainTexture;

                    material.GetComponentInChildren<PainterMenuOnClickApplyPosterMat>().poster = poster;
            }
        }
    }



    //FUNCTIONS THAT PAINT THE FACE OF POSTER AND BLOCK MECHANIC
    //these use multiple checks to see if properties are matching or not...

    void PaintPoster(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "All Face Properties")
        {
            if(CheckIfPoster_HasAnyParametersThatDontMatch(playerObjectInteraction))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<Renderer>().sharedMaterial = playerObjectInteraction.painterCurrentMaterial;
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>().SetMaterialProperties(playerObjectInteraction.painterColor);
                //dont change urlFIlePath of source posters
                if(playerObjectInteraction.painterCurrentMaterial.name != playerObjectInteraction.objectHitInfo.transform.gameObject.name)
                {
                    playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>().urlFilePath = playerObjectInteraction.painterCurrentMaterial.name;
                }

                
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
                PlayPainterSFX(playerObjectInteraction);
            }
        }
        else if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "Materials Only")
        {
            if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.name != playerObjectInteraction.painterCurrentMaterial.name)
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<Renderer>().sharedMaterial = playerObjectInteraction.painterCurrentMaterial;
                //dont change urlFIlePath of source posters
                if(playerObjectInteraction.painterCurrentMaterial.name != playerObjectInteraction.objectHitInfo.transform.gameObject.name)
                {
                    playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>().urlFilePath = playerObjectInteraction.painterCurrentMaterial.name;
                }


                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
                PlayPainterSFX(playerObjectInteraction);
            }
        }
        else if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "Color Only")
        {
            if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>()._color != playerObjectInteraction.painterColor)
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>().SetMaterialProperties(playerObjectInteraction.painterColor);
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
                PlayPainterSFX(playerObjectInteraction);
            }
        }
    }


    void PaintBlockFace(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "All Face Properties")
        {
            if(CheckIfBlockFace_HasAnyParametersThatDontMatch(playerObjectInteraction, playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting,playerObjectInteraction.painterColor);
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().ChangeFaceMatName(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting, playerObjectInteraction.painterCurrentMaterial.name);
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().ChangeFaceMat(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting, playerObjectInteraction.painterCurrentMaterial);
        
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting] = playerObjectInteraction.UVOffSet_Store;
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting] = playerObjectInteraction.UVScale_Store;
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVRotation[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting] = playerObjectInteraction.UVRotation_Store;
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();

                PlayPainterSFX(playerObjectInteraction);
            }
        }
        else if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "Materials Only")
        {
            if(CheckIfFaceMaterialsMatch(playerObjectInteraction, playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().ChangeFaceMatName(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting, playerObjectInteraction.painterCurrentMaterial.name);
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().ChangeFaceMat(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting, playerObjectInteraction.painterCurrentMaterial);
        
                PlayPainterSFX(playerObjectInteraction);
            }
        }
        else if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "Color Only")
        {
            if(CheckIfFaceColorsMatch(playerObjectInteraction, playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting,playerObjectInteraction.painterColor);
        
                PlayPainterSFX(playerObjectInteraction);
            }
        }
        else if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "UVs Only")
        {
            if(CheckIfFaceUVsMatch(playerObjectInteraction, playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting] = playerObjectInteraction.UVOffSet_Store;
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting] = playerObjectInteraction.UVScale_Store;
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UVRotation[playerObjectInteraction.blockQuadHighLighter.currentMaterialIndexHighlighting] = playerObjectInteraction.UVRotation_Store;
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();
        
                PlayPainterSFX(playerObjectInteraction);
            }
        }
    }
        
    void PaintEntireBlock(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "All Face Properties")
        {
            if(CheckIfEntireBlock_MatchesPainterProperties(playerObjectInteraction))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().PaintEntireBlock_Material(playerObjectInteraction.painterCurrentMaterial);
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().PaintEntireBlock_Color(playerObjectInteraction.painterColor);
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().PaintEntireBlock_SetUV(playerObjectInteraction.UVOffSet_Store, playerObjectInteraction.UVScale_Store, playerObjectInteraction.UVRotation_Store);
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();

                PlayPainterSFX(playerObjectInteraction);
            }
        }
        else if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "Materials Only")
        {
            if(CheckIfEntireBlock_MatchesMaterials(playerObjectInteraction))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().PaintEntireBlock_Material(playerObjectInteraction.painterCurrentMaterial);

                PlayPainterSFX(playerObjectInteraction);
            }
        }
        else if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "Color Only")
        {
            if(CheckIfEntireBlock_MatchesColors(playerObjectInteraction))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().PaintEntireBlock_Color(playerObjectInteraction.painterColor);

                PlayPainterSFX(playerObjectInteraction);

            }
        }
        else if(playerObjectInteraction.painter_PaintingSettings_DropDown.options[playerObjectInteraction.painter_PaintingSettings_DropDown.value].text == "UVs Only")
        {
            if(CheckIfEntireBlock_MatchesUVs(playerObjectInteraction))
            {
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().PaintEntireBlock_SetUV(playerObjectInteraction.UVOffSet_Store, playerObjectInteraction.UVScale_Store, playerObjectInteraction.UVRotation_Store);
                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();

                PlayPainterSFX(playerObjectInteraction);

            }
        }
    }




}
