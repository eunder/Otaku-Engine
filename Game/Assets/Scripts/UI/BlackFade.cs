using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackFade : MonoBehaviour
{
    public RawImage FadeImage;
    public int fadeSpeed = 1;
    public float alpha;
    // Start is called before the first frame update
    void Start()
    {
        alpha = 1.0f;
        FadeImage.color = new Color(0,0,0,alpha);
    }

    // Update is called once per frame
    void Update()
    {

        if(FadeImage.color.a > 0.0f)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            FadeImage.color = new Color(0,0,0,alpha);
        }

    }



    


}
