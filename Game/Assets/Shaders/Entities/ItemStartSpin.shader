// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

    Shader "Unlit/ItemStartSpin" {
   Properties {
    _Color ("Color Tint", Color) = (1,1,1,1)
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 }
 
 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 100
     
     ZTest Off
     ZWrite Off
     Blend SrcAlpha OneMinusSrcAlpha 
     
     // BlendOp LogicalCopy
    //AlphaToMask On

     Pass {  
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_fog
             
             #include "UnityCG.cginc"
 
             struct appdata_t {
                 float4 vertex : POSITION;
                 float2 texcoord : TEXCOORD0;
             };
 
             struct v2f {
                 float4 vertex : SV_POSITION;
                 half2 texcoord : TEXCOORD0;
                 UNITY_FOG_COORDS(1)
             };
 
             sampler2D _MainTex;
             float4 _MainTex_ST;
             
             v2f vert (appdata_t v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                 UNITY_TRANSFER_FOG(o,o.vertex);
                 return o;
             }
             
                         fixed4 _Color;

             fixed4 frag (v2f i) : SV_Target
             {
                 fixed4 col = tex2D(_MainTex, i.texcoord);
                 UNITY_APPLY_FOG(i.fogCoord, col);
                return col * _Color;
             }
         ENDCG
     }
 }
 
 }