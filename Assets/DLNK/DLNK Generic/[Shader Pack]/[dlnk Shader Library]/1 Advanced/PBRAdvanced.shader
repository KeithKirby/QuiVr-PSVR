// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:DLNK Shaders/Advanced/PBRAdvOpaque,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:33128,y:32674,varname:node_2865,prsc:2|diff-5688-OUT,spec-688-OUT,gloss-328-OUT,normal-1487-OUT,emission-4297-OUT,lwrap-5158-OUT,difocc-1437-OUT,spcocc-1437-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32114,y:32712,varname:node_6343,prsc:2|A-915-OUT,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31932,y:32822,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31921,y:32620,ptovrint:True,ptlb:Albedo,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32301,y:32791,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_Slider,id:358,x:32500,y:32222,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:_Metallic,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:1813,x:32197,y:32191,ptovrint:True,ptlb:Gloss,ptin:_Glossiness,varname:_Glossiness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:2;n:type:ShaderForge.SFN_Multiply,id:328,x:32559,y:32531,varname:node_328,prsc:2|A-982-A,B-1813-OUT;n:type:ShaderForge.SFN_Multiply,id:688,x:32578,y:32382,varname:node_688,prsc:2|A-982-RGB,B-358-OUT;n:type:ShaderForge.SFN_Vector3,id:8143,x:32336,y:32946,varname:node_8143,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:7874,x:32520,y:32808,varname:node_7874,prsc:2|A-5964-RGB,B-8143-OUT,T-287-OUT;n:type:ShaderForge.SFN_ValueProperty,id:287,x:32336,y:33046,ptovrint:False,ptlb:Normal Smooth,ptin:_NormalSmooth,varname:_NormalSmooth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:9982,x:31452,y:32491,varname:node_9982,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7427,x:31452,y:32636,varname:node_7427,prsc:2|A-9982-UVOUT,B-6981-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6981,x:31258,y:32535,ptovrint:False,ptlb:UV Master Tiling,ptin:_UVMasterTiling,varname:_UVMasterTiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:982,x:32308,y:32362,ptovrint:True,ptlb:Metalness,ptin:_MetallicGlossMap,varname:_MetallicGlossMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_Multiply,id:4297,x:31932,y:33025,varname:node_4297,prsc:2|A-3007-RGB,B-5962-OUT;n:type:ShaderForge.SFN_Tex2d,id:3007,x:31765,y:33008,ptovrint:True,ptlb:Emission,ptin:_EmissionMap,varname:_EmissionMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:5962,x:31872,y:33190,ptovrint:False,ptlb:Emission LM,ptin:_EmissionLM,varname:_EmissionLM,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:6982,x:31759,y:32242,ptovrint:True,ptlb:Occlusion,ptin:_OcclusionMap,varname:_OcclusionMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:6010,x:31759,y:32468,ptovrint:True,ptlb:Occlusion Mult,ptin:_OcclusionStrenght,varname:_OcclusionStrenght,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:5158,x:32349,y:33179,varname:node_5158,prsc:2|A-2256-R,B-2042-OUT;n:type:ShaderForge.SFN_Tex2d,id:2256,x:32165,y:33179,ptovrint:False,ptlb:Translucency Map,ptin:_TranslucencyMap,varname:_TranslucencyMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7427-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2042,x:32148,y:33413,ptovrint:False,ptlb:Trans Mult,ptin:_TransMult,varname:_TransMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:1934,x:32089,y:31973,ptovrint:False,ptlb:Cavity,ptin:_Cavity,varname:_Cavity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_Color,id:8811,x:31918,y:31920,ptovrint:False,ptlb:Cavity Color,ptin:_CavityColor,varname:_CavityColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:5688,x:32301,y:32622,varname:node_5688,prsc:2|A-8811-RGB,B-6343-OUT,T-5100-OUT;n:type:ShaderForge.SFN_Multiply,id:4062,x:32258,y:31865,varname:node_4062,prsc:2|A-1934-R,B-3597-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3597,x:32380,y:31887,ptovrint:False,ptlb:Cavity Mult,ptin:_CavityMult,varname:_CavityMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:7259,x:31285,y:32786,varname:_HeightText,prsc:2,ntxv:0,isnm:False|UVIN-7427-OUT,MIP-2724-OUT,TEX-1432-TEX;n:type:ShaderForge.SFN_Parallax,id:9350,x:31545,y:32814,varname:node_9350,prsc:2|UVIN-7427-OUT,HEI-7259-R,DEP-2405-OUT,REF-3691-OUT;n:type:ShaderForge.SFN_Vector1,id:3691,x:31265,y:33106,varname:node_3691,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2405,x:31265,y:33011,ptovrint:True,ptlb:Depth,ptin:_Parallax,varname:_Parallax,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Blend,id:1437,x:32101,y:32287,varname:node_1437,prsc:2,blmd:10,clmp:True|SRC-1224-OUT,DST-6982-R;n:type:ShaderForge.SFN_Slider,id:2724,x:30941,y:32977,ptovrint:True,ptlb:Depth Map Smooth,ptin:_HgtMapSmooth,varname:_HgtMapSmooth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Tex2dAsset,id:1432,x:30961,y:32697,ptovrint:True,ptlb:Depth Map,ptin:_ParallaxMap,varname:_ParallaxMap,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4269,x:32724,y:33235,ptovrint:True,ptlb:Detail Albedo,ptin:_DetailAlbedoMap,varname:_DetailAlbedoMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1649,x:32724,y:33464,ptovrint:True,ptlb:Detail Bump,ptin:_DetailNormalMap,varname:_DetailNormalMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_Lerp,id:2921,x:32883,y:33464,varname:node_2921,prsc:2|A-1649-RGB,B-8143-OUT,T-6074-OUT;n:type:ShaderForge.SFN_ValueProperty,id:590,x:32724,y:33694,ptovrint:True,ptlb:Detail Bump Smooth,ptin:_DetailNormalMapScale,varname:_DetailNormalMapScale,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:1950,x:32655,y:32929,ptovrint:False,ptlb:Detail Mask,ptin:_DetailMask,varname:_DetailMask,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9350-UVOUT;n:type:ShaderForge.SFN_Lerp,id:1487,x:32913,y:32785,varname:node_1487,prsc:2|A-7874-OUT,B-2921-OUT,T-8121-OUT;n:type:ShaderForge.SFN_Multiply,id:9828,x:32802,y:32902,varname:node_9828,prsc:2|A-1950-R,B-2573-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2573,x:32818,y:33091,ptovrint:False,ptlb:Detail Mask Mult,ptin:_DetailMaskMult,varname:_DetailMaskMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:915,x:32138,y:32842,varname:node_915,prsc:2|A-7736-RGB,B-4269-RGB,T-8121-OUT;n:type:ShaderForge.SFN_Blend,id:5100,x:32258,y:32001,varname:node_5100,prsc:2,blmd:10,clmp:True|SRC-4062-OUT,DST-1934-R;n:type:ShaderForge.SFN_OneMinus,id:6074,x:32836,y:33596,varname:node_6074,prsc:2|IN-590-OUT;n:type:ShaderForge.SFN_Clamp01,id:8121,x:32930,y:32929,varname:node_8121,prsc:2|IN-9828-OUT;n:type:ShaderForge.SFN_Multiply,id:1224,x:31935,y:32259,varname:node_1224,prsc:2|A-6982-R,B-7361-OUT;n:type:ShaderForge.SFN_OneMinus,id:7361,x:31935,y:32377,varname:node_7361,prsc:2|IN-6010-OUT;proporder:6665-7736-5964-287-1432-2724-2405-358-1813-6981-982-1950-2573-4269-1649-590-3007-5962-6982-6010-1934-3597-8811-2256-2042;pass:END;sub:END;*/

Shader "DLNK Shaders/Advanced/PBRAdvanced" {
    Properties {
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _MainTex ("Albedo", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _NormalSmooth ("Normal Smooth", Float ) = 0
        _ParallaxMap ("Depth Map", 2D) = "white" {}
        _HgtMapSmooth ("Depth Map Smooth", Range(0, 5)) = 0
        _Parallax ("Depth", Float ) = 0
        _Metallic ("Metallic", Range(0, 2)) = 1
        _Glossiness ("Gloss", Range(0, 2)) = 0.8
        _UVMasterTiling ("UV Master Tiling", Float ) = 1
        _MetallicGlossMap ("Metalness", 2D) = "white" {}
        _DetailMask ("Detail Mask", 2D) = "white" {}
        _DetailMaskMult ("Detail Mask Mult", Float ) = 0
        _DetailAlbedoMap ("Detail Albedo", 2D) = "white" {}
        _DetailNormalMap ("Detail Bump", 2D) = "bump" {}
        _DetailNormalMapScale ("Detail Bump Smooth", Float ) = 1
        _EmissionMap ("Emission", 2D) = "white" {}
        _EmissionLM ("Emission LM", Float ) = 0
        _OcclusionMap ("Occlusion", 2D) = "white" {}
        _OcclusionStrenght ("Occlusion Mult", Float ) = 1
        _Cavity ("Cavity", 2D) = "white" {}
        _CavityMult ("Cavity Mult", Float ) = 1
        _CavityColor ("Cavity Color", Color) = (0.5,0.5,0.5,1)
        _TranslucencyMap ("Translucency Map", 2D) = "white" {}
        _TransMult ("Trans Mult", Float ) = 0
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
            #pragma glsl
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform float _NormalSmooth;
            uniform float _UVMasterTiling;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float _EmissionLM;
            uniform sampler2D _OcclusionMap; uniform float4 _OcclusionMap_ST;
            uniform float _OcclusionStrenght;
            uniform sampler2D _TranslucencyMap; uniform float4 _TranslucencyMap_ST;
            uniform float _TransMult;
            uniform sampler2D _Cavity; uniform float4 _Cavity_ST;
            uniform float4 _CavityColor;
            uniform float _CavityMult;
            uniform float _Parallax;
            uniform float _HgtMapSmooth;
            uniform sampler2D _ParallaxMap; uniform float4 _ParallaxMap_ST;
            uniform sampler2D _DetailAlbedoMap; uniform float4 _DetailAlbedoMap_ST;
            uniform sampler2D _DetailNormalMap; uniform float4 _DetailNormalMap_ST;
            uniform float _DetailNormalMapScale;
            uniform sampler2D _DetailMask; uniform float4 _DetailMask_ST;
            uniform float _DetailMaskMult;
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
                float4 _HeightText = tex2Dlod(_ParallaxMap,float4(TRANSFORM_TEX(node_7427, _ParallaxMap),0.0,_HgtMapSmooth));
                float2 node_9350 = (_Parallax*(_HeightText.r - 1.0)*mul(tangentTransform, viewDirection).xy + node_7427);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_9350.rg, _BumpMap)));
                float3 node_8143 = float3(0,0,1);
                float3 _DetailNormalMap_var = UnpackNormal(tex2D(_DetailNormalMap,TRANSFORM_TEX(node_9350.rg, _DetailNormalMap)));
                float4 _DetailMask_var = tex2D(_DetailMask,TRANSFORM_TEX(node_9350.rg, _DetailMask));
                float node_8121 = saturate((_DetailMask_var.r*_DetailMaskMult));
                float3 normalLocal = lerp(lerp(_BumpMap_var.rgb,node_8143,_NormalSmooth),lerp(_DetailNormalMap_var.rgb,node_8143,(1.0 - _DetailNormalMapScale)),node_8121);
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
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_9350.rg, _MetallicGlossMap));
                float gloss = (_MetallicGlossMap_var.a*_Glossiness);
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
                float4 _OcclusionMap_var = tex2D(_OcclusionMap,TRANSFORM_TEX(node_9350.rg, _OcclusionMap));
                float node_1437 = saturate(( _OcclusionMap_var.r > 0.5 ? (1.0-(1.0-2.0*(_OcclusionMap_var.r-0.5))*(1.0-(_OcclusionMap_var.r*(1.0 - _OcclusionStrenght)))) : (2.0*_OcclusionMap_var.r*(_OcclusionMap_var.r*(1.0 - _OcclusionStrenght))) ));
                float3 specularAO = node_1437;
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_9350.rg, _MainTex));
                float4 _DetailAlbedoMap_var = tex2D(_DetailAlbedoMap,TRANSFORM_TEX(node_9350.rg, _DetailAlbedoMap));
                float4 _Cavity_var = tex2D(_Cavity,TRANSFORM_TEX(node_9350.rg, _Cavity));
                float3 diffuseColor = lerp(_CavityColor.rgb,(lerp(_MainTex_var.rgb,_DetailAlbedoMap_var.rgb,node_8121)*_Color.rgb),saturate(( _Cavity_var.r > 0.5 ? (1.0-(1.0-2.0*(_Cavity_var.r-0.5))*(1.0-(_Cavity_var.r*_CavityMult))) : (2.0*_Cavity_var.r*(_Cavity_var.r*_CavityMult)) ))); // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, (_MetallicGlossMap_var.rgb*_Metallic).r, specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular) * specularAO;
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float4 _TranslucencyMap_var = tex2D(_TranslucencyMap,TRANSFORM_TEX(node_7427, _TranslucencyMap));
                float node_5158 = (_TranslucencyMap_var.r*_TransMult);
                float3 w = float3(node_5158,node_5158,node_5158)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                NdotLWrap = max(float3(0,0,0), NdotLWrap);
                float3 directDiffuse = (forwardLight + ((1 +(fd90 - 1)*pow((1.00001-NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL))*(0.5-max(w.r,max(w.g,w.b))*0.5) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                indirectDiffuse *= node_1437; // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(node_9350.rg, _EmissionMap));
                float3 emissive = (_EmissionMap_var.rgb*_EmissionLM);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
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
            #pragma glsl
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform float _NormalSmooth;
            uniform float _UVMasterTiling;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float _EmissionLM;
            uniform sampler2D _TranslucencyMap; uniform float4 _TranslucencyMap_ST;
            uniform float _TransMult;
            uniform sampler2D _Cavity; uniform float4 _Cavity_ST;
            uniform float4 _CavityColor;
            uniform float _CavityMult;
            uniform float _Parallax;
            uniform float _HgtMapSmooth;
            uniform sampler2D _ParallaxMap; uniform float4 _ParallaxMap_ST;
            uniform sampler2D _DetailAlbedoMap; uniform float4 _DetailAlbedoMap_ST;
            uniform sampler2D _DetailNormalMap; uniform float4 _DetailNormalMap_ST;
            uniform float _DetailNormalMapScale;
            uniform sampler2D _DetailMask; uniform float4 _DetailMask_ST;
            uniform float _DetailMaskMult;
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
                float4 _HeightText = tex2Dlod(_ParallaxMap,float4(TRANSFORM_TEX(node_7427, _ParallaxMap),0.0,_HgtMapSmooth));
                float2 node_9350 = (_Parallax*(_HeightText.r - 1.0)*mul(tangentTransform, viewDirection).xy + node_7427);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_9350.rg, _BumpMap)));
                float3 node_8143 = float3(0,0,1);
                float3 _DetailNormalMap_var = UnpackNormal(tex2D(_DetailNormalMap,TRANSFORM_TEX(node_9350.rg, _DetailNormalMap)));
                float4 _DetailMask_var = tex2D(_DetailMask,TRANSFORM_TEX(node_9350.rg, _DetailMask));
                float node_8121 = saturate((_DetailMask_var.r*_DetailMaskMult));
                float3 normalLocal = lerp(lerp(_BumpMap_var.rgb,node_8143,_NormalSmooth),lerp(_DetailNormalMap_var.rgb,node_8143,(1.0 - _DetailNormalMapScale)),node_8121);
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
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_9350.rg, _MetallicGlossMap));
                float gloss = (_MetallicGlossMap_var.a*_Glossiness);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_9350.rg, _MainTex));
                float4 _DetailAlbedoMap_var = tex2D(_DetailAlbedoMap,TRANSFORM_TEX(node_9350.rg, _DetailAlbedoMap));
                float4 _Cavity_var = tex2D(_Cavity,TRANSFORM_TEX(node_9350.rg, _Cavity));
                float3 diffuseColor = lerp(_CavityColor.rgb,(lerp(_MainTex_var.rgb,_DetailAlbedoMap_var.rgb,node_8121)*_Color.rgb),saturate(( _Cavity_var.r > 0.5 ? (1.0-(1.0-2.0*(_Cavity_var.r-0.5))*(1.0-(_Cavity_var.r*_CavityMult))) : (2.0*_Cavity_var.r*(_Cavity_var.r*_CavityMult)) ))); // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, (_MetallicGlossMap_var.rgb*_Metallic).r, specularColor, specularMonochrome );
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
                NdotL = dot( normalDirection, lightDirection );
                float4 _TranslucencyMap_var = tex2D(_TranslucencyMap,TRANSFORM_TEX(node_7427, _TranslucencyMap));
                float node_5158 = (_TranslucencyMap_var.r*_TransMult);
                float3 w = float3(node_5158,node_5158,node_5158)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                NdotLWrap = max(float3(0,0,0), NdotLWrap);
                float3 directDiffuse = (forwardLight + ((1 +(fd90 - 1)*pow((1.00001-NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL))*(0.5-max(w.r,max(w.g,w.b))*0.5) * attenColor;
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
            #pragma glsl
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform float _UVMasterTiling;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float _EmissionLM;
            uniform sampler2D _Cavity; uniform float4 _Cavity_ST;
            uniform float4 _CavityColor;
            uniform float _CavityMult;
            uniform float _Parallax;
            uniform float _HgtMapSmooth;
            uniform sampler2D _ParallaxMap; uniform float4 _ParallaxMap_ST;
            uniform sampler2D _DetailAlbedoMap; uniform float4 _DetailAlbedoMap_ST;
            uniform sampler2D _DetailMask; uniform float4 _DetailMask_ST;
            uniform float _DetailMaskMult;
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
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float2 node_7427 = (i.uv0*_UVMasterTiling);
                float4 _HeightText = tex2Dlod(_ParallaxMap,float4(TRANSFORM_TEX(node_7427, _ParallaxMap),0.0,_HgtMapSmooth));
                float2 node_9350 = (_Parallax*(_HeightText.r - 1.0)*mul(tangentTransform, viewDirection).xy + node_7427);
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(node_9350.rg, _EmissionMap));
                o.Emission = (_EmissionMap_var.rgb*_EmissionLM);
                
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_9350.rg, _MainTex));
                float4 _DetailAlbedoMap_var = tex2D(_DetailAlbedoMap,TRANSFORM_TEX(node_9350.rg, _DetailAlbedoMap));
                float4 _DetailMask_var = tex2D(_DetailMask,TRANSFORM_TEX(node_9350.rg, _DetailMask));
                float node_8121 = saturate((_DetailMask_var.r*_DetailMaskMult));
                float4 _Cavity_var = tex2D(_Cavity,TRANSFORM_TEX(node_9350.rg, _Cavity));
                float3 diffColor = lerp(_CavityColor.rgb,(lerp(_MainTex_var.rgb,_DetailAlbedoMap_var.rgb,node_8121)*_Color.rgb),saturate(( _Cavity_var.r > 0.5 ? (1.0-(1.0-2.0*(_Cavity_var.r-0.5))*(1.0-(_Cavity_var.r*_CavityMult))) : (2.0*_Cavity_var.r*(_Cavity_var.r*_CavityMult)) )));
                float specularMonochrome;
                float3 specColor;
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_9350.rg, _MetallicGlossMap));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, (_MetallicGlossMap_var.rgb*_Metallic).r, specColor, specularMonochrome );
                float roughness = 1.0 - (_MetallicGlossMap_var.a*_Glossiness);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "DLNK Shaders/Advanced/PBRAdvOpaque"
    CustomEditor "ShaderForgeMaterialInspector"
}
