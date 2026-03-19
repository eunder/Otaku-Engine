Shader "Retro/Transparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GeoRes("Geometric Resolution", Float) = 22   

        _ColorKey ("Color Key", Color) = (1,1,1,1)
        _Threshold ("Threshold", Range(0, 1)) = 0
        _TransparencyThreshold ("Transparency Threshold", Range(0, 1)) = 0
        _SpillCorrection ("Spill Correction Amount", Range(0, 2)) = 0

    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent"}
        LOD 200
                 ZWrite Off
                 Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4 _ColorKey;
        float _Threshold;
        float _TransparencyThreshold;
        float _SpillCorrection;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


                  float _GeoRes; 



        //VERTEX SNAPPING VERTEX FUNCTION. (remember, for surface shaders you have to return the vertex space in object space)
        void vert (inout appdata_full v) 
        {
            //from object to world position
			float4 worldPosition = mul(unity_ObjectToWorld, v.vertex); // get world space position of vertex

            //from world to view position
            float4 viewSpace = mul(UNITY_MATRIX_V, worldPosition);
            //floor the positions of the vertcices
            viewSpace.xyz = floor(viewSpace.xyz * _GeoRes) / _GeoRes;

            //view space to world space
            float4 worldSpace2 = mul(UNITY_MATRIX_I_V, viewSpace);


            //world space to object space
			v.vertex = mul(unity_WorldToObject, worldSpace2); // reproject position into object space
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 col = tex2D (_MainTex, IN.uv_MainTex) * _Color;



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

                //    if (_Fog > 0) {
                //        UNITY_APPLY_FOG(i.fogCoord, col);
                //    }
                }

                    o.Albedo = col.rgb;
                    o.Alpha = col.a;



        }
        ENDCG
    }
  // disable shadows  
  //  FallBack "Diffuse"
}
