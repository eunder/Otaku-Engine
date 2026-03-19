// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//NOTE! this is an impressive shader i found online, it allows for cutout transperent objects to cast shadows AND receive light


Shader "Poster/LitCutoutCharacter" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Cutoff  ("Alpha cutoff", Range(0,1)) = 0.5

    _DisableVertexSnapping ("Disable VertexSnapping", Range(0, 1)) = 0
    }
    SubShader {
        Tags {"Queue" = "AlphaTest" "RenderType" = "TransparentCutout"}
 
        Pass {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #pragma multi_compile_fwdbase
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma alphatest:_Cutoff
               
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                #include "Lighting.cginc"
               
                struct v2f
                {
                    float4  pos         : SV_POSITION;
                    float2  uv          : TEXCOORD0;
                    UNITY_LIGHTING_COORDS(1,2)
                    UNITY_FOG_COORDS(3)
                };
 
             float _EnableVertexSnapping;
             float _DisableVertexSnapping;


                v2f vert (appdata_tan v)
                {
                    v2f o;
                   
                    o.pos = UnityObjectToClipPos( v.vertex);
                    o.uv = v.texcoord.xy;
                    TRANSFER_VERTEX_TO_FRAGMENT(o);



                    if(_EnableVertexSnapping > 0 && _DisableVertexSnapping < 1)
                    {
                    float4 wp = mul(UNITY_MATRIX_MV, v.vertex);   
                    wp.xyz = floor(wp.xyz * 22) / 22;   
                    float4 sp = mul(UNITY_MATRIX_P, wp);        
                    o.pos = sp;     
                    }



                    UNITY_TRANSFER_FOG(o, o.pos); 


                    return o;
                }
 
                sampler2D _MainTex;
                float _Cutoff;
                fixed4 _Color;
               
                fixed4 frag(v2f i) : COLOR
                {
                    fixed4 color = tex2D(_MainTex, i.uv);
                    clip(color.a - _Cutoff);
                    fixed atten = LIGHT_ATTENUATION(i); // Light attenuation + shadows.

                    UNITY_APPLY_FOG(i.fogCoord, color); 

                    return color * atten * _Color * _LightColor0 ;
                }
            ENDCG
        }
 
        //pass that adds lighting to the shadows of the cutout

        Pass {
            Tags {"LightMode" = "ForwardAdd"}
            Blend One One
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #pragma multi_compile_fwdadd_fullshadows
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest

                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                #include "Lighting.cginc"

                struct v2f
                {
                    float4  pos         : SV_POSITION;
                    float2  uv          : TEXCOORD0;
                    LIGHTING_COORDS(1,2)
                    UNITY_FOG_COORDS(3)

                };
              float _EnableVertexSnapping;
              float _DisableVertexSnapping;

                v2f vert (appdata_tan v)
                {
                    v2f o;
                   
                    o.pos = UnityObjectToClipPos( v.vertex);
                    o.uv = v.texcoord.xy;
                    TRANSFER_VERTEX_TO_FRAGMENT(o);

                    if(_EnableVertexSnapping > 0 && _DisableVertexSnapping < 1)
                    {
                    float4 wp = mul(UNITY_MATRIX_MV, v.vertex);   
                    wp.xyz = floor(wp.xyz * 22) / 22;   
                    float4 sp = mul(UNITY_MATRIX_P, wp);        
                    o.pos = sp;     
                    }

                    UNITY_TRANSFER_FOG(o, o.pos); 


                    return o;
                }
 
                sampler2D _MainTex;
                float _Cutoff;
                fixed4 _Color;
               
                fixed4 frag(v2f i) : COLOR
                {
                    fixed4 color = tex2D(_MainTex, i.uv);
                    clip(color.a - _Cutoff);   
                                   
                    fixed atten = LIGHT_ATTENUATION(i); // Light attenuation + shadows.

                    
                    fixed4 result =tex2D(_MainTex, i.uv) * atten * _Color * _LightColor0 * (UNITY_LIGHTMODEL_AMBIENT * 9 + 0.15f);




            /*
                 // Calculate fog color and density here
                half fogDensity = 0.2; // Example fog density
                half3 fogColor = unity_FogColor; // Example fog color
                
                half depth = i.pos.z / i.pos.w; // Linear depth
                
                half fogFactor = saturate((unity_FogParams.w - depth) / (unity_FogParams.w - unity_FogParams.z));
                half3 finalColor = lerp(fogColor, result.rgb, fogFactor);
                
                return half4(finalColor, result.a);
*/


                    return result;


                }
            ENDCG
        }

    //fog shader pass... its not perfect... but it should work, might not be possible to actually have the perfect cutout shader with lighting and shadows...
Pass {
            Tags {"LightMode" = "ForwardBase"}
            Blend One One
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #pragma multi_compile_fwdadd_fullshadows
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest

                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                #include "Lighting.cginc"

                struct v2f
                {
                    float4  pos         : SV_POSITION;
                    float2  uv          : TEXCOORD0;
                    LIGHTING_COORDS(1,2)
                    UNITY_FOG_COORDS(3)

                };
 
              float _EnableVertexSnapping;
              float _DisableVertexSnapping;

                v2f vert (appdata_tan v)
                {
                    v2f o;
                   
                    o.pos = UnityObjectToClipPos( v.vertex);
                    o.uv = v.texcoord.xy;
                    TRANSFER_VERTEX_TO_FRAGMENT(o);

                    if(_EnableVertexSnapping > 0 && _DisableVertexSnapping < 1)
                    {
                    float4 wp = mul(UNITY_MATRIX_MV, v.vertex);   
                    wp.xyz = floor(wp.xyz * 22) / 22;   
                    float4 sp = mul(UNITY_MATRIX_P, wp);        
                    o.pos = sp;     
                    }

                    UNITY_TRANSFER_FOG(o, o.pos); 


                    return o;
                }
 

 
                sampler2D _MainTex;
                float _Cutoff;
                fixed4 _Color;
               
                fixed4 frag(v2f i) : COLOR
                {
                    fixed4 color = tex2D(_MainTex, i.uv);
                    clip(color.a - _Cutoff);   
                                   
                    fixed atten = LIGHT_ATTENUATION(i); // Light attenuation + shadows.

                                                        //SHADER FUCKERY, for some reason, if you dont add the attenuation... the result is UNLIT!?!?!
                    fixed4 result =tex2D(_MainTex, i.uv) * (atten* .01f) ;



                    UNITY_APPLY_FOG(i.fogCoord, result); 

                    return result;


                }
            ENDCG
        }
    }
    Fallback "Transparent/Cutout/VertexLit"
}