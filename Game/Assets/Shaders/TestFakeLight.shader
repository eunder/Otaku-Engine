    Shader "Unlit/TestFakeLight" 
    {
        Properties 
        {
         _MainTex ("Texture", 2D) = "white" {}
         _MainTexBehindWall ("Texture Behind Wall", 2D) = "white" {}
         _Color ("Behind Wall Texture Color", Color) = (1,1,1,1)
         _ColorHidden ("Behind Wall Texture Color Hideen", Color) = (1,1,1,1)
         _IsHidden ("Is Hidden", Float) = 0.0
         _HideAlpha ("Hide Alpha", Float) = 0.1

        _MyColor ("Highlight Color", Color) = (1,1,1,1) 
        _MyFloat ("Is Highlighted", Float) = 0.0
                            _CutTex ("Cutout (A)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

        }
        SubShader 
        {
            Tags { "Queue" = "Geometry+1" }
            Pass // PASS FOR WHEN OBJECT IS BEHIND WALL 
            { 
                                ZTest Greater

          ZWrite Off // don't write to depth buffer 
               Cull Off
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
         CGPROGRAM
     
                 #include "UnityCG.cginc"

         #pragma vertex vert
         #pragma fragment frag
         uniform float4 _Color; // define shader property for shaders
         uniform float4 _ColorHidden; // define shader property for shaders

         uniform sampler2D _MainTex;
         uniform sampler2D _CutTex;
         uniform sampler2D _MainTexBehindWall;
            float _IsHidden;

         uniform float _Cutoff;
         struct vertexInput {
          float4 vertex : POSITION;
          float4 texcoord : TEXCOORD0;
         };
          float4 _MainTex_ST; 
          float4 _CutTex_ST; 
          float4 _MainTexBehindWall_ST;
         struct vertexOutput {
          float4 pos : SV_POSITION;
          float4 tex : TEXCOORD0;
         };
     
         vertexOutput vert(vertexInput input)
          {
          vertexOutput output;
     
    output.pos = UnityObjectToClipPos(input.vertex);
    output.tex = ComputeScreenPos(output.pos);
          return output;
         }
         float4 frag(vertexOutput input) : COLOR {
               float2 textureCoordinate = input.tex.xy / input.tex.w;
    float aspect = _ScreenParams.x / _ScreenParams.y;
    textureCoordinate.x = textureCoordinate.x * aspect;
    textureCoordinate = TRANSFORM_TEX(textureCoordinate, _MainTexBehindWall);


    float2 textureCoordinatecuteoff = input.tex.xy / input.tex.w;
    textureCoordinatecuteoff.x = textureCoordinatecuteoff.x * aspect;
    textureCoordinatecuteoff = TRANSFORM_TEX(textureCoordinatecuteoff, _CutTex);



    fixed4 col = tex2D(_MainTexBehindWall, textureCoordinate);



    if(_IsHidden == 0)
    {
    col *= _Color;
    }
    else
    {
    col *= _ColorHidden;
    }

    float newOpacity = tex2D(_CutTex, textureCoordinatecuteoff).a; //load cuttext
          if(newOpacity < _Cutoff) {
           newOpacity = 0.3; // change this for cool!
          }

          return float4(col.r, col.g, col.b, 0.2f);

         }
         ENDCG
            }
            Pass // PASS FOR WHEN OBJECT IS SEEN(NORMAL)
            { 
                                ZTest Less

         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

             CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            uniform float4 _Color; // define shader property for shaders
         uniform float4 _ColorHidden; // define shader property for shaders

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _MyColor;
            float _MyFloat;
            float _IsHidden;
            float _HideAlpha;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
                float a;
                // sample the texture
                if(_IsHidden == 0)
                {
                col = tex2D(_MainTex, i.uv);
                a = 1.0f;
                }
                else
                {
                 col = tex2D(_MainTex, i.uv);
                 col = _ColorHidden;  
                 a = _HideAlpha;            
                 }


                 if(_MyFloat == 0)
                {
                 col.rgb = tex2D(_MainTex, i.uv);
                }
                else
                {
                 col.rgb = tex2D(_MainTex, i.uv) + _MyColor;
                }







                                  UNITY_APPLY_FOG(i.fogCoord, col);
                return half4(col.rgb, 0.0f);


            }
            ENDCG
            }
        }
    }