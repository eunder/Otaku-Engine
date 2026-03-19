
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterCanvasUpdater : MonoBehaviour
{
    public Counter counter;
    public TextMeshProUGUI noteText;

    void CheckForCounter()
    {
        if(transform.parent.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<Counter>())
        {
            counter = transform.parent.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<Counter>();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(counter)
        {
        noteText.text = counter.currentCount.ToString();
        }
        else
        {
            CheckForCounter();
        }
    }
}
