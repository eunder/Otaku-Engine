using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineRepeater : MonoBehaviour
{
    public float timePassed = 0.0f;
    public float timeEnd = 5.0f;

    public GameObject[] gameObjectsToLoop;

    public RectTransform TimelineHUDElement;
    public RectTransform TimelineTickerHUDElement;

    // Start is called before the first frame update
    void Start()
    {
            foreach (GameObject test1 in gameObjectsToLoop)
            {
                test1.GetComponent<Animator>().Play("Loop", -1, 0f);
            }
    }

    // Update is called once per frame
    void Update()
    {
        float timelineHUDElement_length = TimelineHUDElement.sizeDelta.x;
        
        TimelineTickerHUDElement.anchoredPosition = new Vector3((timelineHUDElement_length*(timePassed/timeEnd)) - TimelineTickerHUDElement.sizeDelta.x,0.0f,0.0f);

        timePassed += Time.deltaTime;
        if(timePassed > timeEnd)
        {
            foreach (GameObject test1 in gameObjectsToLoop)
            {
                test1.GetComponent<Animator>().Play("Loop", -1, 0f);
            }

            timePassed = 0.0f;
        }
    }
}
