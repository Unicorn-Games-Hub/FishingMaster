// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Ciconia Studio/Effects/Water And Caustic/Water/Fast (Real Reflection)" {
    Properties {
        [Space(15)][Header(Main Maps)]
		[Space(15)]_Color ("Water Color", Color) = (0.09019607,0.3098039,0.4156863,1)
        [Space(10)]_ShoreColor ("Shore Color", Color) = (0,0.7529411,1,1)
        _ShoreDepth ("Shore Depth", Float ) = 1
        [Space(20)]_TideColor ("Tide Color", Color) = (1,1,1,1)
        _TideDepth ("Tide Depth", Range(0, 10)) = 1
        _TideSharpen ("Sharpen", Range(0, 1)) = 0.1
        [Space(35)]_SpecColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularIntensity ("Specular Intensity", Range(0, 2)) = 0.2
        _Glossiness ("Glossiness", Range(0, 2)) = 0.5

        [HideInInspector]_ReflectionTex ("ReflectionTex", 2D) = "bump" {}
        [Space(35)][Header(Reflection Properties)]
        [Space(10)][MaterialToggle] _InvertRealReflectionDeferredCamera ("Invert Real Reflection (Deferred Camera)", Float ) = 0
        _RealReflectionIntensity ("Real Reflection Intensity", Range(0, 1)) = 1
        _CubemapColor ("Color", Color) = (0,0,0,1)
        _Cubemap ("Cubemap ", Cube) = "_Skybox" {}
        _SpecularHighlight ("Specular Highlight", Float ) = 0
        _Blur ("Blur", Range(0, 2)) = 0
        _FresnelStrength ("Fresnel Strength", Range(-2, 8)) = 0

        [Space(35)][Header(Wave Properties)]
        [Space(10)][MaterialToggle] _EnableNormalWaves ("Enable Normal Waves", Float ) = 0
        [Space(10)]_GeneralTiling ("General Tiling", Float ) = 1
        [Space(10)]_Normalwave ("Normal wave", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity1", Range(0, 2)) = 0.2
        _AnimationSpeed1 ("Animation Speed1", Range(0, 1)) = 0.05
        _Angle1 ("Angle1", Float ) = 180
        [Space(10)]_NormalIntensity2 ("Normal Intensity", Range(0, 2)) = 0.2
        _AnimationSpeed2 ("Animation Speed2", Range(0, 1)) = 0.05
        _Angle2 ("Angle2", Float ) = -180
        [Space(10)]_Refraction ("Refraction", Range(0, 2)) = 0.1

        [Space(35)][Header(Transparency Properties)]
        [Space(10)]_ShoreTransparencyDepth ("Shore Transparency Depth", Float ) = 0
        _GeneralTransparency ("General Transparency", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float _SpecularIntensity;
            uniform float4 _Color;
            uniform float _FresnelStrength;
            uniform float _Glossiness;
            uniform samplerCUBE _Cubemap;
            uniform float4 _CubemapColor;
            uniform float _SpecularHighlight;
            uniform float _AnimationSpeed2;
            uniform float _AnimationSpeed1;
            uniform float _NormalIntensity;
            uniform float _Angle1;
            uniform float _Angle2;
            uniform float4 _ShoreColor;
            uniform float _GeneralTiling;
            uniform float _NormalIntensity2;
            uniform float _Blur;
            uniform float _ShoreTransparencyDepth;
            uniform float _Refraction;
            uniform float _ShoreDepth;
            uniform float4 _TideColor;
            uniform float _TideDepth;
            uniform float _TideSharpen;
            uniform float _GeneralTransparency;
            uniform sampler2D _Normalwave; uniform float4 _Normalwave_ST;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform fixed _InvertRealReflectionDeferredCamera;
            uniform float _RealReflectionIntensity;
            uniform fixed _EnableNormalWaves;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 screenPos : TEXCOORD7;
                float4 projPos : TEXCOORD8;
                LIGHTING_COORDS(9,10)
                UNITY_FOG_COORDS(11)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD12;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_5314 = float3(0,0,1);
                float node_4947 = ((_Angle1*3.141592654)/180.0);
                float node_9651_ang = node_4947;
                float node_9651_spd = 1.0;
                float node_9651_cos = cos(node_9651_spd*node_9651_ang);
                float node_9651_sin = sin(node_9651_spd*node_9651_ang);
                float2 node_9651_piv = float2(0.5,0.5);
                float2 node_1815 = (i.uv0*_GeneralTiling);
                float2 node_9651 = (mul(node_1815-node_9651_piv,float2x2( node_9651_cos, -node_9651_sin, node_9651_sin, node_9651_cos))+node_9651_piv);
                float4 node_426 = _Time + _TimeEditor;
                float node_1885 = (node_426.g*_AnimationSpeed1);
                float2 node_143 = (i.uv0/4.0);
                float2 node_1192 = (node_9651+(node_143+node_1885*float2(0,0.6)));
                float3 _node_1398 = UnpackNormal(tex2D(_Normalwave,TRANSFORM_TEX(node_1192, _Normalwave)));
                float node_8725_ang = ((_Angle2*3.141592654)/180.0);
                float node_8725_spd = 1.0;
                float node_8725_cos = cos(node_8725_spd*node_8725_ang);
                float node_8725_sin = sin(node_8725_spd*node_8725_ang);
                float2 node_8725_piv = float2(0.5,0.5);
                float2 GeneralTiling = node_1815;
                float2 node_8725 = (mul(GeneralTiling-node_8725_piv,float2x2( node_8725_cos, -node_8725_sin, node_8725_sin, node_8725_cos))+node_8725_piv);
                float4 node_1399 = _Time + _TimeEditor;
                float2 node_2310 = (node_8725+(1.0 - (node_143+(node_1399.g*_AnimationSpeed2)*float2(0,0.6))));
                float3 _node_3492 = UnpackNormal(tex2D(_Normalwave,TRANSFORM_TEX(node_2310, _Normalwave)));
                float3 node_5280_nrm_base = lerp(node_5314,_node_1398.rgb,_NormalIntensity) + float3(0,0,1);
                float3 node_5280_nrm_detail = lerp(float3(0,0,1),_node_3492.rgb,_NormalIntensity2) * float3(-1,-1,1);
                float3 node_5280_nrm_combined = node_5280_nrm_base*dot(node_5280_nrm_base, node_5280_nrm_detail)/node_5280_nrm_base.z - node_5280_nrm_detail;
                float3 node_5280 = node_5280_nrm_combined;
                float3 Normalmap = lerp( node_5314, node_5280, _EnableNormalWaves );
                float3 normalLocal = Normalmap;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float2 Refraction = (node_5280.rg*_Refraction);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + Refraction;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float Glossiness = _Glossiness;
                float gloss = Glossiness;
                float perceptualRoughness = 1.0 - Glossiness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float node_8072 = (((0.95*pow(1.0-max(0,dot(normalDirection, viewDirection)),1.0))+0.05)*_FresnelStrength);
                float4 _Cubemap_var = texCUBElod(_Cubemap,float4(viewReflectDirection,lerp(0,14,_Blur)));
                float3 CubemapSpec = (_CubemapColor.rgb*(node_8072+(_Cubemap_var.rgb*(_Cubemap_var.a*_SpecularHighlight))));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 Specular = (_SpecColor.rgb*_SpecularIntensity);
                float3 specularColor = Specular;
                float specularMonochrome;
                float2 _InvertRealReflectionDeferredCamera_var = lerp( float2(sceneUVs.r,(1.0 - sceneUVs.g)), sceneUVs.rg, _InvertRealReflectionDeferredCamera );
                float4 _ReflectionTex_var = tex2D(_ReflectionTex,TRANSFORM_TEX(_InvertRealReflectionDeferredCamera_var, _ReflectionTex));
                float3 BaseColor = lerp(saturate((1.0-(1.0-(_ReflectionTex_var.rgb*_RealReflectionIntensity))*(1.0-_Color.rgb))),saturate((1.0-(1.0-(_TideColor.rgb*(1.0 - saturate((sceneZ-partZ)/_TideDepth))*lerp(0,10,_TideSharpen)))*(1.0-_ShoreColor.rgb))),(1.0 - saturate(saturate((sceneZ-partZ)/_ShoreDepth))));
                float3 diffuseColor = BaseColor; // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular + CubemapSpec);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float Opacity = (saturate(saturate((sceneZ-partZ)/_ShoreTransparencyDepth))*(1.0 - _GeneralTransparency));
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,Opacity),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float _SpecularIntensity;
            uniform float4 _Color;
            uniform float _Glossiness;
            uniform float _AnimationSpeed2;
            uniform float _AnimationSpeed1;
            uniform float _NormalIntensity;
            uniform float _Angle1;
            uniform float _Angle2;
            uniform float4 _ShoreColor;
            uniform float _GeneralTiling;
            uniform float _NormalIntensity2;
            uniform float _ShoreTransparencyDepth;
            uniform float _Refraction;
            uniform float _ShoreDepth;
            uniform float4 _TideColor;
            uniform float _TideDepth;
            uniform float _TideSharpen;
            uniform float _GeneralTransparency;
            uniform sampler2D _Normalwave; uniform float4 _Normalwave_ST;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform fixed _InvertRealReflectionDeferredCamera;
            uniform float _RealReflectionIntensity;
            uniform fixed _EnableNormalWaves;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 screenPos : TEXCOORD7;
                float4 projPos : TEXCOORD8;
                LIGHTING_COORDS(9,10)
                UNITY_FOG_COORDS(11)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_5314 = float3(0,0,1);
                float node_4947 = ((_Angle1*3.141592654)/180.0);
                float node_9651_ang = node_4947;
                float node_9651_spd = 1.0;
                float node_9651_cos = cos(node_9651_spd*node_9651_ang);
                float node_9651_sin = sin(node_9651_spd*node_9651_ang);
                float2 node_9651_piv = float2(0.5,0.5);
                float2 node_1815 = (i.uv0*_GeneralTiling);
                float2 node_9651 = (mul(node_1815-node_9651_piv,float2x2( node_9651_cos, -node_9651_sin, node_9651_sin, node_9651_cos))+node_9651_piv);
                float4 node_426 = _Time + _TimeEditor;
                float node_1885 = (node_426.g*_AnimationSpeed1);
                float2 node_143 = (i.uv0/4.0);
                float2 node_1192 = (node_9651+(node_143+node_1885*float2(0,0.6)));
                float3 _node_1398 = UnpackNormal(tex2D(_Normalwave,TRANSFORM_TEX(node_1192, _Normalwave)));
                float node_8725_ang = ((_Angle2*3.141592654)/180.0);
                float node_8725_spd = 1.0;
                float node_8725_cos = cos(node_8725_spd*node_8725_ang);
                float node_8725_sin = sin(node_8725_spd*node_8725_ang);
                float2 node_8725_piv = float2(0.5,0.5);
                float2 GeneralTiling = node_1815;
                float2 node_8725 = (mul(GeneralTiling-node_8725_piv,float2x2( node_8725_cos, -node_8725_sin, node_8725_sin, node_8725_cos))+node_8725_piv);
                float4 node_1399 = _Time + _TimeEditor;
                float2 node_2310 = (node_8725+(1.0 - (node_143+(node_1399.g*_AnimationSpeed2)*float2(0,0.6))));
                float3 _node_3492 = UnpackNormal(tex2D(_Normalwave,TRANSFORM_TEX(node_2310, _Normalwave)));
                float3 node_5280_nrm_base = lerp(node_5314,_node_1398.rgb,_NormalIntensity) + float3(0,0,1);
                float3 node_5280_nrm_detail = lerp(float3(0,0,1),_node_3492.rgb,_NormalIntensity2) * float3(-1,-1,1);
                float3 node_5280_nrm_combined = node_5280_nrm_base*dot(node_5280_nrm_base, node_5280_nrm_detail)/node_5280_nrm_base.z - node_5280_nrm_detail;
                float3 node_5280 = node_5280_nrm_combined;
                float3 Normalmap = lerp( node_5314, node_5280, _EnableNormalWaves );
                float3 normalLocal = Normalmap;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float2 Refraction = (node_5280.rg*_Refraction);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + Refraction;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float Glossiness = _Glossiness;
                float gloss = Glossiness;
                float perceptualRoughness = 1.0 - Glossiness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 Specular = (_SpecColor.rgb*_SpecularIntensity);
                float3 specularColor = Specular;
                float specularMonochrome;
                float2 _InvertRealReflectionDeferredCamera_var = lerp( float2(sceneUVs.r,(1.0 - sceneUVs.g)), sceneUVs.rg, _InvertRealReflectionDeferredCamera );
                float4 _ReflectionTex_var = tex2D(_ReflectionTex,TRANSFORM_TEX(_InvertRealReflectionDeferredCamera_var, _ReflectionTex));
                float3 BaseColor = lerp(saturate((1.0-(1.0-(_ReflectionTex_var.rgb*_RealReflectionIntensity))*(1.0-_Color.rgb))),saturate((1.0-(1.0-(_TideColor.rgb*(1.0 - saturate((sceneZ-partZ)/_TideDepth))*lerp(0,10,_TideSharpen)))*(1.0-_ShoreColor.rgb))),(1.0 - saturate(saturate((sceneZ-partZ)/_ShoreDepth))));
                float3 diffuseColor = BaseColor; // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float Opacity = (saturate(saturate((sceneZ-partZ)/_ShoreTransparencyDepth))*(1.0 - _GeneralTransparency));
                fixed4 finalRGBA = fixed4(finalColor * Opacity,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float _SpecularIntensity;
            uniform float4 _Color;
            uniform float _Glossiness;
            uniform float4 _ShoreColor;
            uniform float _ShoreDepth;
            uniform float4 _TideColor;
            uniform float _TideDepth;
            uniform float _TideSharpen;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform fixed _InvertRealReflectionDeferredCamera;
            uniform float _RealReflectionIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float4 projPos : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : SV_Target {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float2 _InvertRealReflectionDeferredCamera_var = lerp( float2(sceneUVs.r,(1.0 - sceneUVs.g)), sceneUVs.rg, _InvertRealReflectionDeferredCamera );
                float4 _ReflectionTex_var = tex2D(_ReflectionTex,TRANSFORM_TEX(_InvertRealReflectionDeferredCamera_var, _ReflectionTex));
                float3 BaseColor = lerp(saturate((1.0-(1.0-(_ReflectionTex_var.rgb*_RealReflectionIntensity))*(1.0-_Color.rgb))),saturate((1.0-(1.0-(_TideColor.rgb*(1.0 - saturate((sceneZ-partZ)/_TideDepth))*lerp(0,10,_TideSharpen)))*(1.0-_ShoreColor.rgb))),(1.0 - saturate(saturate((sceneZ-partZ)/_ShoreDepth))));
                float3 diffColor = BaseColor;
                float3 Specular = (_SpecColor.rgb*_SpecularIntensity);
                float3 specColor = Specular;
                float specularMonochrome = max(max(specColor.r, specColor.g),specColor.b);
                diffColor *= (1.0-specularMonochrome);
                float Glossiness = _Glossiness;
                float roughness = 1.0 - Glossiness;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
