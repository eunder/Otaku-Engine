using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressClass : System.IProgress<float>
{
    float lastValue = 0;
    UploadingScreen waitPanel;

    public ProgressClass()
    {
        waitPanel = GameObject.FindObjectOfType<UploadingScreen>();
    }

    public void Report(float value)
    {
        if(lastValue >= value) return;


        lastValue = value;

        if(waitPanel != null)
        {
                if(value < 1f)
                {

                    int percent = Mathf.RoundToInt(value * 100);

                    waitPanel.TurnOn();
                waitPanel.UpdateMessage("Uploading: " + percent);

                }
                else
                {
                waitPanel.TurnOff();
                }
        }

        Debug.Log("Uploading: " + value);


    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
