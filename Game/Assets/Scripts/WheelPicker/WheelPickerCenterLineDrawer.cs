using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WheelPickerCenterLineDrawer : Graphic
{
    public WheelPickerHandler UIElementsAround;
    public float www = 1.0f;
    Vector2 objPos;
    Vector2 B;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if(UIElementsAround.closestObj != null)
        {
          objPos = new Vector3(UIElementsAround.closestObj.GetComponent<RectTransform>().anchoredPosition.x, UIElementsAround.closestObj.GetComponent<RectTransform>().anchoredPosition.y);
        }
        if(UIElementsAround.closestObj == null)
        {
            objPos = new Vector2(0.0f,0.0f);
        }

        vh.Clear();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height ;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;


        if(UIElementsAround.closestObj != null)
        {
        B = UIElementsAround.closestObj.GetComponent<RectTransform>().anchoredPosition;
        }
        var A = new Vector2(0f,0f);
        var v = (B - A).normalized;
        var right2 = new Vector2(v.y, -v.x);

        vertex.position = new Vector2(0,0) + right2 * www;
        vh.AddVert(vertex);

        vertex.position = new Vector2(0,0) + -right2 * www;
        vh.AddVert(vertex);

        vertex.position = objPos / 2; //half way between center of screen and item pos
        vh.AddVert(vertex);

        //vertex.position = objPos + right2 * www;
        //vh.AddVert(vertex);

        vh.AddTriangle(0,1,2);

       
    }

    // Update is called once per frame
    void Update()
    {
            SetVerticesDirty();
    }
}
