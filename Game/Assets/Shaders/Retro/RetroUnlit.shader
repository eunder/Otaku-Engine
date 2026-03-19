Shader "Retro/Unlit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 
        





    _GeoRes("Geometric Resolution", Float) = 40   
    float _GeoRes;

    v2f vert (appdata v) 
     {                
    v2f o;                 
    float4 wp = mul(UNITY_MATRIX_MV, v.vertex);   
    wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;   
    float4 sp = mul(UNITY_MATRIX_P, wp);        
    o.vertex = sp;         
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);  
    UNITY_TRANSFER_FOG(o,o.vertex);    
    return o;
    }





        Pass {
            Color [_Color]
            SetTexture [_MainTex] { combine texture * primary } 
        }

        
    }
}