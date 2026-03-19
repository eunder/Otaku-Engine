using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPicker : MonoBehaviour
{   public Image previewColor;
    public Image selectedColor;
    RectTransform Rect;
    Texture2D ColorTexture;
    // Start is called before the first frame update
    void Start()
    {
        Rect = GetComponent<RectTransform>();
        ColorTexture = GetComponent<Image>().mainTexture as Texture2D;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 delta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, Input.mousePosition, null, out delta);

        string debug = "mousePosition= " + Input.mousePosition;

        float width = Rect.rect.width;
        float height = Rect.rect.height;

        float x = Mathf.Clamp(delta.x / width, 0, 1);
        float y = Mathf.Clamp(delta.y / height, 0, 1);

        int texX = Mathf.RoundToInt(x * ColorTexture.width);
        int texY = Mathf.RoundToInt(y * ColorTexture.height);

        Color color = ColorTexture.GetPixel(texX, texY);

                if(color.a < 0.9f) // if color goes a little transparent, turn it black
                {
                    color = Color.black;
                }
         previewColor.color = color;

        if(Input.GetMouseButtonDown(0)) // click to set color
        {
            selectedColor.color = color;
        }
    }
}
