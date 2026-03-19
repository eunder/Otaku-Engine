using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement_SliderStep : MonoBehaviour
{
        public float stepAmount = 0.01f;
        public Slider mySlider = null;
     
        float numberOfSteps = 0;
     
        // Start is called before the first frame update
        void Start()
        {
            mySlider = GetComponent<Slider>();
            numberOfSteps =  mySlider.maxValue / stepAmount;
        }
     
        public void UpdateStep(float value)
        {
            float steppedValue = Mathf.Round(value / stepAmount) * stepAmount;
            mySlider.value = steppedValue;
        }

}
