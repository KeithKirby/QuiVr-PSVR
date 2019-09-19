// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:6535,x:32719,y:32712,varname:node_6535,prsc:2|diff-7250-OUT,normal-5212-OUT,emission-7250-OUT;n:type:ShaderForge.SFN_Slider,id:3489,x:31186,y:32523,ptovrint:False,ptlb:Reflection Edges,ptin:_ReflectionEdges,varname:_ReflectionGlow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Vector1,id:8462,x:31322,y:32293,varname:node_8462,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:4617,x:31322,y:32221,varname:node_4617,prsc:2,v1:10;n:type:ShaderForge.SFN_Lerp,id:7528,x:31554,y:32321,varname:node_7528,prsc:2|A-4617-OUT,B-8462-OUT,T-3489-OUT;n:type:ShaderForge.SFN_Fresnel,id:5849,x:31749,y:32321,varname:node_5849,prsc:2|EXP-7528-OUT;n:type:ShaderForge.SFN_Color,id:8393,x:31993,y:31969,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4589,x:32217,y:32124,varname:node_4589,prsc:2|A-8393-RGB,B-5849-OUT,C-5849-OUT,D-5849-OUT;n:type:ShaderForge.SFN_Tex2d,id:5613,x:31632,y:33016,ptovrint:False,ptlb:Normal (Animated),ptin:_NormalAnimated,varname:node_5613,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-9256-OUT;n:type:ShaderForge.SFN_Vector3,id:5238,x:31648,y:32913,varname:node_5238,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:5650,x:32023,y:32874,varname:node_5650,prsc:2|A-5238-OUT,B-5613-RGB,T-8425-OUT;n:type:ShaderForge.SFN_Slider,id:8425,x:31511,y:33221,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_8425,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:6503,x:31869,y:32584,ptovrint:False,ptlb:Emissive Intensity,ptin:_EmissiveIntensity,varname:node_6503,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:1;n:type:ShaderForge.SFN_Multiply,id:7250,x:32463,y:32277,varname:node_7250,prsc:2|A-4589-OUT,B-3773-OUT;n:type:ShaderForge.SFN_Lerp,id:7943,x:32214,y:32789,varname:node_7943,prsc:2|A-179-UVOUT,B-5650-OUT,T-2758-OUT;n:type:ShaderForge.SFN_TexCoord,id:179,x:31588,y:32732,varname:node_179,prsc:2,uv:2;n:type:ShaderForge.SFN_Slider,id:2758,x:31901,y:33130,ptovrint:False,ptlb:Normal Density,ptin:_NormalDensity,varname:node_2758,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_Panner,id:5898,x:31070,y:32870,varname:node_5898,prsc:2,spu:1,spv:1|UVIN-179-UVOUT,DIST-7384-OUT;n:type:ShaderForge.SFN_Time,id:9568,x:30601,y:32755,varname:node_9568,prsc:2;n:type:ShaderForge.SFN_Lerp,id:7384,x:30849,y:32907,varname:node_7384,prsc:2|A-9568-TSL,B-9568-T,T-8816-OUT;n:type:ShaderForge.SFN_Slider,id:8816,x:30500,y:33016,ptovrint:False,ptlb:Animation Speed,ptin:_AnimationSpeed,varname:node_8816,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_RemapRange,id:6566,x:32365,y:33099,varname:node_6566,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7943-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:5212,x:32484,y:32872,ptovrint:False,ptlb:Invert Effect,ptin:_InvertEffect,varname:node_5212,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-7943-OUT,B-6566-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9256,x:31300,y:32942,ptovrint:False,ptlb:Switch Animation Flow,ptin:_SwitchAnimationFlow,varname:node_9034,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5898-UVOUT,B-1006-UVOUT;n:type:ShaderForge.SFN_Panner,id:1006,x:31070,y:33004,varname:node_1006,prsc:2,spu:-1,spv:-1|UVIN-179-UVOUT,DIST-7384-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:3773,x:32299,y:32463,varname:node_3773,prsc:2,a:0,b:2|IN-6503-OUT;proporder:8393-3489-6503-2758-5613-8425-8816-5212-9256;pass:END;sub:END;*/

Shader "Ciconia Studio/Effects/Ghost/Old version(1.2)/Ghost Animated" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _ReflectionEdges ("Reflection Edges", Range(0, 1)) = 0.5
        _EmissiveIntensity ("Emissive Intensity", Range(0, 1)) = 0.25
        _NormalDensity ("Normal Density", Range(0, 1)) = 0.2
        _NormalAnimated ("Normal (Animated)", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 1
        _AnimationSpeed ("Animation Speed", Range(0, 1)) = 0.2
        [MaterialToggle] _InvertEffect ("Invert Effect", Float ) = -1
        [MaterialToggle] _SwitchAnimationFlow ("Switch Animation Flow", Float ) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcColor
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float _ReflectionEdges;
            uniform float4 _Color;
            uniform sampler2D _NormalAnimated; uniform float4 _NormalAnimated_ST;
            uniform float _NormalIntensity;
            uniform float _EmissiveIntensity;
            uniform float _NormalDensity;
            uniform float _AnimationSpeed;
            uniform fixed _InvertEffect;
            uniform fixed _SwitchAnimationFlow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv2 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_9568 = _Time + _TimeEditor;
                float node_7384 = lerp(node_9568.r,node_9568.g,_AnimationSpeed);
                float2 _SwitchAnimationFlow_var = lerp( (i.uv2+node_7384*float2(1,1)), (i.uv2+node_7384*float2(-1,-1)), _SwitchAnimationFlow );
                float3 _NormalAnimated_var = UnpackNormal(tex2D(_NormalAnimated,TRANSFORM_TEX(_SwitchAnimationFlow_var, _NormalAnimated)));
                float3 node_7943 = lerp(float3(i.uv2,0.0),lerp(float3(0,0,1),_NormalAnimated_var.rgb,_NormalIntensity),_NormalDensity);
                float3 normalLocal = lerp( node_7943, (node_7943*2.0+-1.0), _InvertEffect );
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float node_5849 = pow(1.0-max(0,dot(normalDirection, viewDirection)),lerp(10.0,0.0,_ReflectionEdges));
                float3 node_7250 = ((_Color.rgb*node_5849*node_5849*node_5849)*lerp(0,2,_EmissiveIntensity));
                float3 diffuseColor = node_7250;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = node_7250;
/// Final Color:
                float3 finalColor = diffuse + emissive;
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float _ReflectionEdges;
            uniform float4 _Color;
            uniform sampler2D _NormalAnimated; uniform float4 _NormalAnimated_ST;
            uniform float _NormalIntensity;
            uniform float _EmissiveIntensity;
            uniform float _NormalDensity;
            uniform float _AnimationSpeed;
            uniform fixed _InvertEffect;
            uniform fixed _SwitchAnimationFlow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv2 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
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
                float4 node_9568 = _Time + _TimeEditor;
                float node_7384 = lerp(node_9568.r,node_9568.g,_AnimationSpeed);
                float2 _SwitchAnimationFlow_var = lerp( (i.uv2+node_7384*float2(1,1)), (i.uv2+node_7384*float2(-1,-1)), _SwitchAnimationFlow );
                float3 _NormalAnimated_var = UnpackNormal(tex2D(_NormalAnimated,TRANSFORM_TEX(_SwitchAnimationFlow_var, _NormalAnimated)));
                float3 node_7943 = lerp(float3(i.uv2,0.0),lerp(float3(0,0,1),_NormalAnimated_var.rgb,_NormalIntensity),_NormalDensity);
                float3 normalLocal = lerp( node_7943, (node_7943*2.0+-1.0), _InvertEffect );
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float node_5849 = pow(1.0-max(0,dot(normalDirection, viewDirection)),lerp(10.0,0.0,_ReflectionEdges));
                float3 node_7250 = ((_Color.rgb*node_5849*node_5849*node_5849)*lerp(0,2,_EmissiveIntensity));
                float3 diffuseColor = node_7250;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
