using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class WheelPickerButtonEventHolder : MonoBehaviour
{
    public UnityEvent buttonEvent;
    // Start is called before the first frame update
 
    public void PlayButtonEvent()
    {
        buttonEvent.Invoke();
    }
}
