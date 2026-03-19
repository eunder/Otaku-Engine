using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLineStylingManager : MonoBehaviour
{
    private static EventLineStylingManager _instance;
    public static EventLineStylingManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public Color eventLineColorStart_Standard;
    public Color eventLineColorEnd_Standard;
    public Color eventLineColorStart_Highlight;
    public Color eventLineColorEnd_Highlight;
    public Color eventLineColor_Hover;

    public float eventLineWidthStart_Standard; 
    public float eventLineWidthEnd_Standard; 
    public float eventLineWidthStart_Highlight; 
    public float eventLineWidthEnd_Highlight; 
    public float eventLineWidth_Hover; 

    public float globalLineEventTransparencyMod;
}
