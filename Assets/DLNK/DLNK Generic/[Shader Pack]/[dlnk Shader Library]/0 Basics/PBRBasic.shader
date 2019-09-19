// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:Standard,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:32719,y:32712,varname:node_2865,prsc:2|diff-6343-OUT,spec-688-OUT,gloss-328-OUT,normal-7874-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32114,y:32712,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31921,y:32865,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31921,y:32620,ptovrint:True,ptlb:Albedo,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7427-OUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32336,y:32791,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-7427-OUT;n:type:ShaderForge.SFN_Slider,id:358,x:32500,y:32222,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:_Metallic,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32165,y:32212,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Multiply,id:328,x:32411,y:32437,varname:node_328,prsc:2|A-8958-G,B-1813-OUT;n:type:ShaderForge.SFN_Multiply,id:5514,x:31714,y:32282,varname:node_5514,prsc:2|A-6665-A,B-501-OUT;n:type:ShaderForge.SFN_ValueProperty,id:501,x:31552,y:32370,ptovrint:False,ptlb:Alpha Metal Mult,ptin:_AlphaMetalMult,varname:_AlphaMetalMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:688,x:32623,y:32422,varname:node_688,prsc:2|A-8958-R,B-358-OUT;n:type:ShaderForge.SFN_Vector3,id:8143,x:32336,y:32946,varname:node_8143,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:7874,x:32505,y:32791,varname:node_7874,prsc:2|A-5964-RGB,B-8143-OUT,T-287-OUT;n:type:ShaderForge.SFN_ValueProperty,id:287,x:32336,y:33046,ptovrint:False,ptlb:Normal Smooth,ptin:_NormalSmooth,varname:_NormalSmooth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:9982,x:31739,y:32578,varname:node_9982,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7427,x:31739,y:32725,varname:node_7427,prsc:2|A-9982-UVOUT,B-6981-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6981,x:31739,y:32865,ptovrint:False,ptlb:UV Master Tiling,ptin:_UVMasterTiling,varname:_UVMasterTiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:1039,x:31971,y:32273,varname:node_1039,prsc:2|A-3698-OUT,B-789-OUT,T-3628-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9070,x:32176,y:32335,ptovrint:False,ptlb:Use Metal Maps,ptin:_UseMetalMaps,varname:_UseMetalMaps,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9051-OUT,B-1039-OUT;n:type:ShaderForge.SFN_Blend,id:428,x:31552,y:32151,varname:node_428,prsc:2,blmd:10,clmp:True|SRC-7736-A,DST-5514-OUT;n:type:ShaderForge.SFN_Tex2d,id:6795,x:31901,y:32035,ptovrint:True,ptlb:Metalness,ptin:_MetallicGlossMap,varname:_MetallicGlossMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7427-OUT;n:type:ShaderForge.SFN_Tex2d,id:4693,x:32181,y:31969,ptovrint:False,ptlb:Cust Metal,ptin:_CustMetal,varname:_CustMetal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7427-OUT;n:type:ShaderForge.SFN_Tex2d,id:7530,x:32362,y:31969,ptovrint:False,ptlb:Cust Gloss,ptin:_CustGloss,varname:_CustGloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7427-OUT;n:type:ShaderForge.SFN_Append,id:789,x:32527,y:31948,varname:node_789,prsc:2|A-4693-R,B-7530-R;n:type:ShaderForge.SFN_Append,id:3698,x:32032,y:32144,varname:node_3698,prsc:2|A-6795-R,B-6795-A;n:type:ShaderForge.SFN_Slider,id:3628,x:31842,y:32412,ptovrint:False,ptlb:Custom Maps,ptin:_CustomMaps,varname:_CustomMaps,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Append,id:9051,x:31703,y:32151,varname:node_9051,prsc:2|A-428-OUT,B-428-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8958,x:32103,y:32495,varname:node_8958,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9070-OUT;proporder:6665-7736-6981-5964-287-358-1813-501-9070-6795-3628-4693-7530;pass:END;sub:END;*/

Shader "DLNK Shaders/Basics/PBRBasic" {
    Properties {
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _MainTex ("Albedo", 2D) = "white" {}
        _UVMasterTiling ("UV Master Tiling", Float ) = 1
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _NormalSmooth ("Normal Smooth", Float ) = 0
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _AlphaMetalMult ("Alpha Metal Mult", Float ) = 1
        [MaterialToggle] _UseMetalMaps ("Use Metal Maps", Float ) = 1
        _MetallicGlossMap ("Metalness", 2D) = "white" {}
        _CustomMaps ("Custom Maps", Range(0, 1)) = 0
        _CustMetal ("Cust Metal", 2D) = "white" {}
        _CustGloss ("Cust Gloss", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _AlphaMetalMult;
            uniform float _NormalSmooth;
            uniform float _UVMasterTiling;
            uniform fixed _UseMetalMaps;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _CustMetal; uniform float4 _CustMetal_ST;
            uniform sampler2D _CustGloss; uniform float4 _CustGloss_ST;
            uniform float _CustomMaps;
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
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
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
                #elif UNITY_SHOULD_SAMPLE_SH
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
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_7427 = (i.uv0*_UVMasterTiling);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_7427, _BumpMap)));
                float3 normalLocal = lerp(_BumpMap_var.rgb,float3(0,0,1),_NormalSmooth);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_7427, _MainTex));
                float node_428 = saturate(( (_Color.a*_AlphaMetalMult) > 0.5 ? (1.0-(1.0-2.0*((_Color.a*_AlphaMetalMult)-0.5))*(1.0-_MainTex_var.a)) : (2.0*(_Color.a*_AlphaMetalMult)*_MainTex_var.a) ));
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_7427, _MetallicGlossMap));
                float4 _CustMetal_var = tex2D(_CustMetal,TRANSFORM_TEX(node_7427, _CustMetal));
                float4 _CustGloss_var = tex2D(_CustGloss,TRANSFORM_TEX(node_7427, _CustGloss));
                float2 node_8958 = lerp( float2(node_428,node_428), lerp(float2(_MetallicGlossMap_var.r,_MetallicGlossMap_var.a),float2(_CustMetal_var.r,_CustGloss_var.r),_CustomMaps), _UseMetalMaps ).rg;
                float gloss = (node_8958.g*_Gloss);
                float specPow = exp2( gloss * 10.0+1.0);
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
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 diffuseColor = (_MainTex_var.rgb*_Color.rgb); // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, (node_8958.r*_Metallic), specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _AlphaMetalMult;
            uniform float _NormalSmooth;
            uniform float _UVMasterTiling;
            uniform fixed _UseMetalMaps;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _CustMetal; uniform float4 _CustMetal_ST;
            uniform sampler2D _CustGloss; uniform float4 _CustGloss_ST;
            uniform float _CustomMaps;
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
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
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
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_7427 = (i.uv0*_UVMasterTiling);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_7427, _BumpMap)));
                float3 normalLocal = lerp(_BumpMap_var.rgb,float3(0,0,1),_NormalSmooth);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_7427, _MainTex));
                float node_428 = saturate(( (_Color.a*_AlphaMetalMult) > 0.5 ? (1.0-(1.0-2.0*((_Color.a*_AlphaMetalMult)-0.5))*(1.0-_MainTex_var.a)) : (2.0*(_Color.a*_AlphaMetalMult)*_MainTex_var.a) ));
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_7427, _MetallicGlossMap));
                float4 _CustMetal_var = tex2D(_CustMetal,TRANSFORM_TEX(node_7427, _CustMetal));
                float4 _CustGloss_var = tex2D(_CustGloss,TRANSFORM_TEX(node_7427, _CustGloss));
                float2 node_8958 = lerp( float2(node_428,node_428), lerp(float2(_MetallicGlossMap_var.r,_MetallicGlossMap_var.a),float2(_CustMetal_var.r,_CustGloss_var.r),_CustomMaps), _UseMetalMaps ).rg;
                float gloss = (node_8958.g*_Gloss);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 diffuseColor = (_MainTex_var.rgb*_Color.rgb); // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, (node_8958.r*_Metallic), specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _AlphaMetalMult;
            uniform float _UVMasterTiling;
            uniform fixed _UseMetalMaps;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _CustMetal; uniform float4 _CustMetal_ST;
            uniform sampler2D _CustGloss; uniform float4 _CustGloss_ST;
            uniform float _CustomMaps;
            struct VertexInput {
                float4 vertex : POSITION;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float2 node_7427 = (i.uv0*_UVMasterTiling);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_7427, _MainTex));
                float3 diffColor = (_MainTex_var.rgb*_Color.rgb);
                float specularMonochrome;
                float3 specColor;
                float node_428 = saturate(( (_Color.a*_AlphaMetalMult) > 0.5 ? (1.0-(1.0-2.0*((_Color.a*_AlphaMetalMult)-0.5))*(1.0-_MainTex_var.a)) : (2.0*(_Color.a*_AlphaMetalMult)*_MainTex_var.a) ));
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_7427, _MetallicGlossMap));
                float4 _CustMetal_var = tex2D(_CustMetal,TRANSFORM_TEX(node_7427, _CustMetal));
                float4 _CustGloss_var = tex2D(_CustGloss,TRANSFORM_TEX(node_7427, _CustGloss));
                float2 node_8958 = lerp( float2(node_428,node_428), lerp(float2(_MetallicGlossMap_var.r,_MetallicGlossMap_var.a),float2(_CustMetal_var.r,_CustGloss_var.r),_CustomMaps), _UseMetalMaps ).rg;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, (node_8958.r*_Metallic), specColor, specularMonochrome );
                float roughness = 1.0 - (node_8958.g*_Gloss);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Standard"
    CustomEditor "ShaderForgeMaterialInspector"
}
