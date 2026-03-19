Shader "Retro/Realtime"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GeoRes("Geometric Resolution", Float) = 22   

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        //note: make sure to "addshadow" so that ambient occlusion glitch dosnt happen with the custom "vert" function
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert

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
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
