Shader "Custom/FakeLighting_Darken"
{
    Properties
    {
        [HDR]_Color("Color", Color) = (1, 1, 1, 0.5)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        CGINCLUDE
        #include "UnityCG.cginc"

        float4 _Color;

        struct appdata
        {
            float4 vertex : POSITION;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
                        float2 uv : TEXCOORD0;
        };

        v2f vert(appdata v)
        {
                 v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                //way to prevent z fighting... 
                o.vertex = UnityObjectToClipPos(v.vertex * 1.0005);
            
                 return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            fixed4 col =  _Color * _Color;
            return col;    
                }

        ENDCG
        Pass
        {
            Name "Mask"
            Ztest Greater
            Zwrite Off
            Cull Front
            Colormask 0

            Stencil
            {
                Ref 1
                Comp Greater
                Pass Replace
            }

            //this part is needed or else exanding the block will yield no results
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

        Pass
        {
            Name "Darken Outside"
            Zwrite Off
            Ztest Lequal
            Cull Back
            Blend SrcColor OneMinusSrcAlpha  // Blend mode for darkening

            Stencil
            {
                Comp Equal
                Ref 1
                Pass Zero
                Fail Zero
                Zfail Zero
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }

        Pass
        {
            Name "Darken Inside"
            ZTest Off
            ZWrite Off
            Cull Front
            Blend SrcColor OneMinusSrcAlpha  // Blend mode for darkening

            Stencil
            {
                Ref 1
                Comp Equal
                Pass Zero
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }

 
    }
}