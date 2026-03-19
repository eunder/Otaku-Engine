using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorPickerLogic : MonoBehaviour
{
    private static UIColorPickerLogic _instance;
    public static UIColorPickerLogic Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

    }


        public Color color;
        public Color InitialColor;
        public RectTransform colorPicker;
         Vector2 pos;
         public Canvas colorPickerCanvas;

        public AudioClip openWheel_AudioClip;
        public AudioClip closeWheel_AudioClip;

    public void ActivateColorPicker()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(colorPickerCanvas.transform as RectTransform, Input.mousePosition, colorPickerCanvas.worldCamera, out pos);
         transform.position = new Vector2( colorPickerCanvas.transform.TransformPoint(pos).x + 10.0f,  colorPickerCanvas.transform.TransformPoint(pos).y - colorPickerCanvas.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y/2);

        //prevent from going to far down screen
         if(transform.position.y < 0)
         {
            transform.position = new Vector2(transform.position.x,0);
         }

        //prevent from going too far up screen
         if(transform.position.y + transform.GetComponent<RectTransform>().sizeDelta.y > Screen.height)
         {
            transform.position = new Vector2(transform.position.x,Screen.height - transform.GetComponent<RectTransform>().sizeDelta.y);
         }


        //prevent from going to far left
         if(transform.position.x < 0)
         {
            transform.position = new Vector2(0,transform.position.y);
         }

        //prevent from going to far right
         if(transform.position.x + transform.GetComponent<RectTransform>().sizeDelta.x > Screen.width)
         {
            transform.position = new Vector2(Screen.width - transform.GetComponent<RectTransform>().sizeDelta.x, transform.position.y);
         }

        colorPicker.gameObject.SetActive(true);
        ToolBarStatic_UISFXAudioSource.Instance.ToolBar_UISFX_Play(openWheel_AudioClip);
    }

    // Update is called once per frame
    void Update()
    {
        if(colorPicker.gameObject.activeSelf)
        {
        Vector2 delta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(colorPicker, Input.mousePosition, null, out delta);

        string debug = "mousePosition= " + Input.mousePosition;

        Texture2D colorPickerTexture = colorPicker.GetComponent<Image>().mainTexture as Texture2D;

        float width = colorPicker.rect.width;
        float height = colorPicker.rect.height; 

        float x = Mathf.Clamp(delta.x / width, 0, 1);
        float y = Mathf.Clamp(delta.y / height, 0, 1);

        int texX = Mathf.RoundToInt(x * colorPickerTexture.width);
        int texY = Mathf.RoundToInt(y * colorPickerTexture.height);

        Color pixelColor = colorPickerTexture.GetPixel(texX, texY);

                if(pixelColor.a < 0.9f) // if color goes a little transparent, give back the initial color 
                {
                    color = InitialColor;
                }
                else
                {
                    color = new Color(pixelColor.r, pixelColor.g, pixelColor.b, InitialColor.a); //maintains the og alpha
                }

        if(Input.GetMouseButtonUp(0))
        {
            if(colorPicker.gameObject.activeSelf)
            {
            colorPicker.gameObject.SetActive(false);
            ToolBarStatic_UISFXAudioSource.Instance.ToolBar_UISFX_Play(closeWheel_AudioClip);
            }
        }
    }
    }

}
