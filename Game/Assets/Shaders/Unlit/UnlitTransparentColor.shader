Shader "Unlit/Transparent Colored Texture" {
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _DisableVertexSnapping ("Disable VertexSnapping", Range(0, 1)) = 0
    }
    SubShader
    {
 
       Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
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
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Blend;
            float _EnableVertexSnapping;
            float _DisableVertexSnapping;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

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
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
               
                col = tex2D(_MainTex, i.uv) * _Color;
              //  return col;
                                                           
 
               
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
         
           
            ENDCG
        }

       
        // Pass to render object as a shadow caster
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            LOD 80
            Cull [_Culling]
            Offset [_Offset], [_Offset]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
           
            CGPROGRAM
            #pragma target 2.0
            #pragma multi_compile_shadowcaster
            ENDCG
        }

    }
}
 