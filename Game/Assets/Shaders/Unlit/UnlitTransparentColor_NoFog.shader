Shader "Unlit/Transparent Colored Texture Z Pass No Fog" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    ZTest Always
    Blend One OneMinusSrcAlpha
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off
    Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 

        Pass {
            Color [_Color]
            SetTexture [_MainTex] { combine texture * primary } 
        }

        
    }
}