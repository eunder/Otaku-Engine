
Shader "PosterStencil/read" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Color ("Main Color", Color) = (1,1,1,1)
	[IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
    _StencilComp ("StencilComp", Int) = 0 //3 = equal   0 = disabled

    [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual" = default  // Always = draw overeverything
   
    _ColorKey ("Color Key", Color) = (1,1,1,1)
    _Threshold ("Threshold", Range(0, 1)) = 0
    _TransparencyThreshold ("Transparency Threshold", Range(0, 1)) = 0
    _SpillCorrection ("Spill Correction Amount", Range(0, 2)) = 0
    [IntRange] _Fog ("Enable Fog", Range(0,1)) = 1

    _DisableVertexSnapping ("Disable VertexSnapping", Range(0, 1)) = 0

}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

		ZTest [_ZTest]
    	Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Stencil
        {
            ref [_StencilRef]
            comp [_StencilComp]
        }



    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;


            float4 _ColorKey;
            float _Threshold;
            float _TransparencyThreshold;
            float _SpillCorrection;
            fixed _Fog;
            float _EnableVertexSnapping;
            float _DisableVertexSnapping;


            
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                if(_EnableVertexSnapping > 0 && _DisableVertexSnapping < 1)
                {
                float4 wp = mul(UNITY_MATRIX_MV, v.vertex);   
                wp.xyz = floor(wp.xyz * 22) / 22;   
                float4 sp = mul(UNITY_MATRIX_P, wp);        
                o.vertex = sp;     
                }



                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 col = tex2D(_MainTex, i.texcoord);

                if(_Threshold > 0 || _TransparencyThreshold > 0)
                {
                    // Calculate the chroma key difference
                    fixed4 keyDiff = abs(col - _ColorKey);
                    float keyBlend = smoothstep(0, _Threshold, max(max(keyDiff.r, keyDiff.g), keyDiff.b));

                    // Calculate the transparency based on the color key difference
                    float transparency = smoothstep(0, _TransparencyThreshold, max(max(keyDiff.r, keyDiff.g), keyDiff.b));

                    // Remove color key from semi-transparent pixels
                    col.rgb -= _ColorKey.rgb * _SpillCorrection  * (1 - transparency);

                    // Apply transparency to the color
                    col.a *= transparency;

                    // Set alpha to 0 for fully transparent pixels
                    col.a = col.a > 0 ? col.a : 0;

                    // Discard pixels that match the chroma key color
                    clip(keyBlend - 0.5);
                }


                    if (_Fog > 0) {
                        UNITY_APPLY_FOG(i.fogCoord, col);
                    }

                    return col * _Color;

        }
        ENDCG
    }
}

}