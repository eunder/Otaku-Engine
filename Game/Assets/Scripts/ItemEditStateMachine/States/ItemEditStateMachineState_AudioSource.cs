using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_AudioSource : IItemEditStateMachine 
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
        return itemEditor.ItemEditStateMachineStateAudioSource;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.audioSourceEditor_CANVAS.SetActive(true);
         
                itemEditor.audioSource_audioPath_InputField.text = itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().audioPath;
                itemEditor.audioSource_pitch_InputField.text = itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().pitch.ToString();
                itemEditor.audioSource_spatialBlend_Slider.value = itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().spatialBlend;
                itemEditor.audioSource_minDistance_InputField.text = itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().minDistance.ToString();
                itemEditor.audioSource_maxDistance_InputField.text = itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().maxDistance.ToString();
                itemEditor.audioSource_repeat_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().repeat;
    
                itemEditor.audioSource_minDistance_VisualizationGameObject.SetActive(true);
                itemEditor.audioSource_maxDistance_VisualizationGameObject.SetActive(true);
    
    
     }

        void OnStay(ItemEditStateMachine itemEditor)
        {


                if(itemEditor.currentObjectEditing)
                {

                  itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().audioPath = itemEditor.audioSource_audioPath_InputField.text;
                  itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().pitch =float.Parse( itemEditor.audioSource_pitch_InputField.text);
                  itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().spatialBlend = itemEditor.audioSource_spatialBlend_Slider.value;
                  itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().minDistance = float.Parse(itemEditor.audioSource_minDistance_InputField.text);
                  itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().maxDistance = float.Parse(itemEditor.audioSource_maxDistance_InputField.text);
                  itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().repeat = itemEditor.audioSource_repeat_Toggle.isOn;

                  itemEditor.currentObjectEditing.GetComponent<AudioSourceObject>().SetValuesOnAudioSource();
             
                
                
                if(itemEditor.currentObjectEditing.GetComponent<AudioSource>().isPlaying)
                {
                    //show stop button
                    itemEditor.audioSource_Play_Button.gameObject.SetActive(false);
                    itemEditor.audioSource_Stop_Button.gameObject.SetActive(true);
                }
                else
                {
                    //show play button
                    itemEditor.audioSource_Play_Button.gameObject.SetActive(true);
                    itemEditor.audioSource_Stop_Button.gameObject.SetActive(false);
                }
             

                    //autio position/resize spheres (for visualizing)
                itemEditor.audioSource_minDistance_VisualizationGameObject.transform.position = itemEditor.currentObjectEditing.transform.position;
                itemEditor.audioSource_maxDistance_VisualizationGameObject.transform.position = itemEditor.currentObjectEditing.transform.position;

                
                float minScale = 2 * float.Parse(itemEditor.audioSource_minDistance_InputField.text);
                float maxScale = 2 * float.Parse(itemEditor.audioSource_maxDistance_InputField.text);

                itemEditor.audioSource_minDistance_VisualizationGameObject.transform.localScale = new Vector3(minScale,minScale,minScale);
                itemEditor.audioSource_maxDistance_VisualizationGameObject.transform.localScale = new Vector3(maxScale,maxScale,maxScale);
             

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
                itemEditor.audioSourceEditor_CANVAS.SetActive(false);

                itemEditor.audioSource_minDistance_VisualizationGameObject.SetActive(false);
                itemEditor.audioSource_maxDistance_VisualizationGameObject.SetActive(false);
        }



}
