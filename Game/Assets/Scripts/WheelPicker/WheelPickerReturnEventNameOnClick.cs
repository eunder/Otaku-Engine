using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPickerReturnEventNameOnClick : MonoBehaviour
{
        //used to differenciate and tell the game the appropriate parameters to set
        public enum TypeOfEvent{On, Do};
        public TypeOfEvent eventType;


        public string eventCoroutineName;
        public string eventParameter;
        
        public string defaultValue = "0";

        public enum TypeOfParameter{none, _int, _float, _string, _object, _color, _vector3};
        
        //This is what you need to show in the inspector.
        public TypeOfParameter parameterType;
        public string parameterDescription; // for letting the player know what they are setting and how it will influence

        public void AssignEventName() 
        {
                Debug.Log("coroutine name:" + eventCoroutineName);
                //assign the name of the event
                if(IsOnParameter())
                {
                        PlayerObjectInteractionStateMachine.Instance.onEvent = eventCoroutineName;

                        //used to prevent the paramter from being cleard when this function happens(mostly used for making sure the state dosnt get cleared)
                        if(string.IsNullOrEmpty(PlayerObjectInteractionStateMachine.Instance.onParameter))
                        {
                                if(defaultValue != eventParameter)
                                {
                                        PlayerObjectInteractionStateMachine.Instance.onParameter = eventParameter;
                                }
                                else
                                {
                                        PlayerObjectInteractionStateMachine.Instance.onParameter = defaultValue;
                                }
                        }
                }
                else
                {
                        PlayerObjectInteractionStateMachine.Instance.doEvent = eventCoroutineName;   

                        //used to prevent the paramter from being cleard when this function happens(mostly used for making sure the state dosnt get cleared)
                        if(string.IsNullOrEmpty(PlayerObjectInteractionStateMachine.Instance.doParameter))
                        {
                                PlayerObjectInteractionStateMachine.Instance.doParameter = eventParameter;
                        }
                        else
                        {
                                PlayerObjectInteractionStateMachine.Instance.doParameter = defaultValue;
                        }
                }



               //close wheel
                WheelPickerHandler.Instance.CloseWheelPicker();

                //
                // HARDTHINKING MODE FOR THE CODE BELLOW: [required]
                //

                //if there is no parameter assigned ("none" enum), skip the parameter window and go straight to delay setting
                if(parameterType == TypeOfParameter.none)
                {
                        //if it is a "DO" event(last step)... then go to the delay window
                        if(eventType == TypeOfEvent.Do)
                        {
                                EventsUI_DelayCanvasSingleton.Instance.OpenDelayWindow();
                        }
                        else
                        {
                        //if the parameter type is none and we are on the ON event... then let the player proceed interacting with the next thing in the wiring state
                        PlayerObjectInteractionStateMachine.Instance.canPickNextEventObject = true;
                        }

                }
                else if (parameterType == TypeOfParameter._object)
                {
                EventsUI_Parameter_GameObjectPick_CanvasSingleton.Instance.isOnEventParameter = IsOnParameter();
                EventsUI_Parameter_GameObjectPick_CanvasSingleton.Instance.OpenParameter_ObjectPic_Window();
                EventsUI_Parameter_GameObjectPick_CanvasSingleton.Instance.parameterDesription_Text.text = parameterDescription;
                }
                else if (parameterType == TypeOfParameter._vector3)
                {
                EventsUI_Parameter_Vector3_CanvasSingleton.Instance.isOnEventParameter = IsOnParameter();
                EventsUI_Parameter_Vector3_CanvasSingleton.Instance.OpenParameterWindow();
                EventsUI_Parameter_Vector3_CanvasSingleton.Instance.parameterDesription_Text.text = parameterDescription;

                }
                else
                {
                EventsUI_ParameterCanvasSingleton.Instance.isOnEventParameter = IsOnParameter();
                EventsUI_ParameterCanvasSingleton.Instance.OpenParameterWindow(parameterType, defaultValue);
                EventsUI_ParameterCanvasSingleton.Instance.parameterDesription_Text.text = parameterDescription;
                }

        }

        //used to set the bool in the parameter singleton classes that dictcate whether the parameter being set is for ON or DO
        //it does this by checking if the parent name contains "ON" or "DO"
        public bool IsOnParameter()
        {
                if(eventType == TypeOfEvent.On)
                {
                 return true;
                }
                else
                {
                return false;
                }
        }
        
}
