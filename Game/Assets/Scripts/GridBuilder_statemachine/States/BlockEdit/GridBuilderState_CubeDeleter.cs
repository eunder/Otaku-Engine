using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.IO;

public class GridBuilderState_CubeDeleter : IGridBuilderState 
{

    int hitCount = 0;
    GameObject currentHittingObject;
    float blockVolume;
    public IGridBuilderState DoState(GridBuilderStateMachine gridBuilder)
    {
        if(gridBuilder.newStateOnceEvent == false)
        {
            OnEnter(gridBuilder);
            gridBuilder.newStateOnceEvent = true;
        }


        OnStay(gridBuilder);

            if(Input.GetMouseButtonDown(1))
            {
                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateIdle;
            }
           
        return gridBuilder.GridBuilderStateCubeDeleter;
    }


    
        void OnEnter(GridBuilderStateMachine gridBuilder)
        {
              gridBuilder.toolTipText.text = "Left Mouse: Destroy <color=white>| RIght Mouse: <color=red>Go Back <color=white>";
                    gridBuilder.mouseLooker.enabled = true;
              gridBuilder.grid.gameObject.SetActive(false);
              gridBuilder.EditorCube.SetActive(false);
              gridBuilder.Hammer_Model.SetActive(true);

        }   




        void OnStay(GridBuilderStateMachine gridBuilder)
        {
  
   if(Input.GetMouseButtonDown(0))
        {
    
        RaycastHit hit;
        Ray ray = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridBuilder.collidableLayers_layerMask))
        {
            Vector3 assumedScale = hit.transform.GetComponent<BlockFaceTextureUVProperties>().GetBlockScaleBasedLocalVertexPositions();


            blockVolume = assumedScale.x * assumedScale.y * assumedScale.z;


            if(hit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
            {
                if(currentHittingObject == null)
                {
                    currentHittingObject = hit.transform.gameObject;
                }

                if(currentHittingObject == hit.transform.gameObject)
                {
                    hitCount++;
                    gridBuilder.gridBuilderDestroyer_AudioSource.pitch =  gridBuilder.pitchCurve.Evaluate(blockVolume);
                    gridBuilder.gridBuilderDestroyer_AudioSource.PlayOneShot(gridBuilder.gridBuilderDestroyerHit_AudioClip);
                    gridBuilder.Hammer_anim.Play("ArmatureAction_001", -1, 0f);


                }
                else
                {
                    currentHittingObject = hit.transform.gameObject;
                    hitCount = 1;
                    gridBuilder.gridBuilderDestroyer_AudioSource.pitch =  gridBuilder.pitchCurve.Evaluate(blockVolume);
                    gridBuilder.gridBuilderDestroyer_AudioSource.PlayOneShot(gridBuilder.gridBuilderDestroyerHit_AudioClip);
                    gridBuilder.Hammer_anim.Play("ArmatureAction_001", -1, 0f);

                }

                hit.transform.DORewind ();
                hit.transform.gameObject.transform.DOShakePosition(0.5f, 0.1f, 77, 70);

          
            if(hitCount >= 3) // break event
            {
                   BlockBaseCubePositionFinder_Singleton.Instance.RemoveFromCube();

                   GameObject.Destroy(hit.transform.gameObject);

                    SaveAndLoadLevel.Instance.allLoadedBlocks.Remove(hit.transform.gameObject);
                    SaveAndLoadLevel.Instance.allLoadedGameObjects.Remove(hit.transform.gameObject);

                    GameObject destructionParticle = GameObject.Instantiate(gridBuilder.deletion_Particle, hit.transform.gameObject.transform.position, hit.transform.gameObject.transform.rotation);

                            ParticleSystem.ShapeModule _editableShape = destructionParticle.GetComponent<ParticleSystem>().shape;

 
                            _editableShape.position = new Vector3(0,0,0);
                            _editableShape.scale = new Vector3(assumedScale.x, assumedScale.y, assumedScale.z);
                

                destructionParticle.GetComponent<ParticleSystem>().startSize = gridBuilder.scale(0.0f, 200f, 0.005f, 0.15f, blockVolume);
            
                hitCount = 0;

                gridBuilder.gridBuilderDestroyer_AudioSource.pitch =  gridBuilder.pitchCurve.Evaluate(blockVolume);
                gridBuilder.gridBuilderDestroyer_AudioSource.PlayOneShot(gridBuilder.gridBuilderDestroyerGlass_AudioClip);
            }


            }   
        }
        }

        }

        void OnExit(GridBuilderStateMachine gridBuilder)
        {
              gridBuilder.toolTipText.text = "";
              gridBuilder.Hammer_Model.SetActive(false);
              gridBuilder.playerObjectInteractionStateMachine.enabled = true;
              gridBuilder.mouseLooker.wheelPickerIsTurnedOn = false;

        }
}
