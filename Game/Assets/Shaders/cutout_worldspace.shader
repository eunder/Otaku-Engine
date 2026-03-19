// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

      Shader "Experimental/worldspace" {
       Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _CutTex ("Cutout (A)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

        _DisableVertexSnapping ("Disable VertexSnapping", Range(0, 1)) = 0
       }
       SubShader {
        Tags { "Queue" = "Transparent+500" }
        Pass {
         ZWrite Off // don't write to depth buffer 
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
         CGPROGRAM
     
                 #include "UnityCG.cginc"

         #pragma vertex vert
         #pragma fragment frag
         uniform float4 _Color; // define shader property for shaders
         uniform sampler2D _MainTex;
         uniform sampler2D _CutTex;
         uniform float _Cutoff;
         struct vertexInput {
          float4 vertex : POSITION;
          float4 texcoord : TEXCOORD0;
         };
          float4 _MainTex_ST; 
          float4 _CutTex_ST; 

         struct vertexOutput {
          float4 pos : SV_POSITION;
          float4 tex : TEXCOORD0;
         };


            float _EnableVertexSnapping;
            float _DisableVertexSnapping;

         vertexOutput vert(vertexInput input)
          {
    vertexOutput output;

    // Original vertex position transformation
    output.pos = UnityObjectToClipPos(input.vertex);

      if(_EnableVertexSnapping > 0 && _DisableVertexSnapping < 1)
      {
            // Additional code snippet integration
            float4 wp = mul(UNITY_MATRIX_MV, input.vertex);
            wp.xyz = floor(wp.xyz * 22) / 22;
            float4 sp = mul(UNITY_MATRIX_P, wp);
            output.pos = sp;
      }


    // Additional texture coordinate transformation if needed
    output.tex = ComputeScreenPos(output.pos);

    // Return the output
    return output;
         }
         float4 frag(vertexOutput input) : COLOR {
               float2 textureCoordinate = input.tex.xy / input.tex.w;
    float aspect = _ScreenParams.x / _ScreenParams.y;
    textureCoordinate.x = textureCoordinate.x * aspect;
    textureCoordinate = TRANSFORM_TEX(textureCoordinate, _MainTex);


    float2 textureCoordinatecuteoff = input.tex.xy / input.tex.w;
    textureCoordinatecuteoff.x = textureCoordinatecuteoff.x * aspect;
    textureCoordinatecuteoff = TRANSFORM_TEX(textureCoordinatecuteoff, _CutTex);



       fixed4 col = tex2D(_MainTex, textureCoordinate);

    col.rgb *= _Color.rgb; // Apply the color tint

    // Use the alpha channel from _Color for transparency
    col.a *= _Color.a;

    return col;
         }
         ENDCG
        }
       }
      }