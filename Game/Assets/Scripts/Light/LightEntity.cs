using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEntity : MonoBehaviour
{
    public string lightType = "point";

    public float range = 10.0f;
    public float strength = 1.0f;
    public Color color = Color.white;
    public float spotAngle = 30.0f;
    public string shadowType = "none";


    //Dynamic color
    public Renderer rend;
    private MaterialPropertyBlock propBlock;


    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        UpdateColorOfWidget();
    }

    public void UpdateParameters()
    {
        //update normal light properties
        if(gameObject.GetComponent<Light>())
        {
           if(lightType == "point")
           gameObject.GetComponent<Light>().type = LightType.Point;
           if(lightType == "spot")
           gameObject.GetComponent<Light>().type = LightType.Spot;
           if(lightType == "directional")
           gameObject.GetComponent<Light>().type = LightType.Directional;



           gameObject.GetComponent<Light>().range = range;
           gameObject.GetComponent<Light>().intensity = strength;
           gameObject.GetComponent<Light>().color = color;
           gameObject.GetComponent<Light>().spotAngle = spotAngle;

           if(shadowType == "none")
            gameObject.GetComponent<Light>().shadows = LightShadows.None;
           if(shadowType == "hard")
            gameObject.GetComponent<Light>().shadows = LightShadows.Soft;

        }
                }



    //Dynamic color
    public void UpdateColorOfWidget()
    {
        if(propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        propBlock.SetColor("_Color", color);
        rend.SetPropertyBlock(propBlock);
    }





}
