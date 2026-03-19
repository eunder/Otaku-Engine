    Shader "Unlit/HighLightObjectShader" {
  Properties {
        _Color2 ("Main Color", Color) = (1,1,1,1)
        _MainTex2 ("Base (RGB) Trans (A)", 2D) = "white" {}
        _CutTex ("Cutout (A)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "Queue" = "Transparent+500" }
        Pass {
            ZWrite Off // don't write to depth buffer 
            ZTest Always

            Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
            CGPROGRAM
                #include "UnityCG.cginc"
                #pragma vertex vert
                #pragma fragment frag

                uniform float4 _Color2; // define shader property for shaders
                uniform sampler2D _MainTex2;
                uniform sampler2D _CutTex;
                uniform float _Cutoff;

                struct vertexInput {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0; // Use float2 for texture coordinates
                };

                struct vertexOutput {
                    float4 pos : SV_POSITION;
                    float2 tex : TEXCOORD0; // Use float2 for texture coordinates
                };

                vertexOutput vert(vertexInput input) {
                    vertexOutput output;
                    output.pos = UnityObjectToClipPos(input.vertex);
                    output.tex = input.texcoord; // Pass the texture coordinates directly
                    return output;
                }

                float4 frag(vertexOutput input) : COLOR {
                    float2 textureCoordinate = input.tex;

                    float2 textureCoordinatecuteoff = input.tex;

                    fixed4 col = tex2D(_MainTex2, textureCoordinate);
                    col.rgb *= _Color2.rgb; // Apply the color tint

                    // Use the alpha channel from _Color for transparency
                    col.a *= _Color2.a;

                    return col;
                }
            ENDCG
        }
    }
}