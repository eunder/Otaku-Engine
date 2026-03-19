    Shader "Overdraw_EventHiddenBlock" {
    Properties {
        _MainTex ("Base", 2D) = "white" {}
        _ColEventHidden ("Main Color", Color) = (0.15,0.0,0.0,0.0)
    }
     
    SubShader {
        Fog { Mode Off }
        ZWrite Off
        ZTest Always
        Blend One One // additive blending
        Cull Off
        Pass {
            SetTexture[_MainTex] {
                constantColor [_ColEventHidden]
                combine constant, texture
            }
        }
    }
    }
