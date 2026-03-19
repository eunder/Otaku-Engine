using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//INCREMENTS SLIDERS AND INPUT FIELDS

//uses raycast all to make sure it increment ui elements as long as the player mouse hovers over... AND uses mouse wheel

public class MouseWheelIncrementElementField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{



    public float incrementAmount = 0.1f;

    // Update is called once per frame
    void Update()
    {

 
            
              if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                    // Raycast to check if any UI elements are under the cursor
                    PointerEventData pointerData = new PointerEventData(EventSystem.current);
                    pointerData.position = Input.mousePosition;

                    List<RaycastResult> results = new List<RaycastResult>();

                    EventSystem.current.RaycastAll(pointerData, results);

                    bool cursorOverUI = false;

                    foreach (RaycastResult result in results)
                    {                                                       // Check if any of the results belong to this UI element
                        if (result.gameObject.GetComponent<Slider>() && result.gameObject == gameObject)
                        {
                            GetComponent<Slider>().value += incrementAmount;
                            break;
                        }
                                                                                // Check if any of the results belong to this UI element
                        if(result.gameObject.GetComponent<TMP_InputField>() && result.gameObject == gameObject)
                        {
                            if(string.IsNullOrEmpty(GetComponent<TMP_InputField>().text))
                            {
                                GetComponent<TMP_InputField>().text = "0";
                            }
                            float currentValue = float.Parse(GetComponent<TMP_InputField>().text);
                            currentValue += incrementAmount;

                            GetComponent<TMP_InputField>().text = currentValue.ToString();

                            break;
                        }
                    }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                 // Raycast to check if any UI elements are under the cursor
                    PointerEventData pointerData = new PointerEventData(EventSystem.current);
                    pointerData.position = Input.mousePosition;

                    List<RaycastResult> results = new List<RaycastResult>();

                    EventSystem.current.RaycastAll(pointerData, results);

                    bool cursorOverUI = false;

                    foreach (RaycastResult result in results)
                    {                                                       // Check if any of the results belong to this UI element
                        if (result.gameObject.GetComponent<Slider>() && result.gameObject == gameObject)
                        {
                            GetComponent<Slider>().value -= incrementAmount;
                            break;
                        }
                                                                                // Check if any of the results belong to this UI element
                        if(result.gameObject.GetComponent<TMP_InputField>() && result.gameObject == gameObject)
                        {
                            if(string.IsNullOrEmpty(GetComponent<TMP_InputField>().text))
                            {
                                GetComponent<TMP_InputField>().text = "0";
                            }
                            float currentValue = float.Parse(GetComponent<TMP_InputField>().text);
                            currentValue -= incrementAmount;

                            GetComponent<TMP_InputField>().text = currentValue.ToString();

                            break;
                        }
                    }

            }
    
    }


    //CHECKING IF MOUSE IS OVER ELEMENT MECHANIC
    private bool mouseOver = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }




}
