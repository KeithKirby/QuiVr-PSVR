// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:6535,x:32719,y:32712,varname:node_6535,prsc:2|diff-7250-OUT,normal-7429-OUT,emission-7250-OUT;n:type:ShaderForge.SFN_Slider,id:3489,x:31186,y:32523,ptovrint:False,ptlb:Reflection Edges,ptin:_ReflectionEdges,varname:_ReflectionGlow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Vector1,id:8462,x:31322,y:32293,varname:node_8462,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:4617,x:31322,y:32221,varname:node_4617,prsc:2,v1:10;n:type:ShaderForge.SFN_Lerp,id:7528,x:31554,y:32321,varname:node_7528,prsc:2|A-4617-OUT,B-8462-OUT,T-3489-OUT;n:type:ShaderForge.SFN_Fresnel,id:5849,x:31749,y:32321,varname:node_5849,prsc:2|EXP-7528-OUT;n:type:ShaderForge.SFN_Color,id:8393,x:31993,y:31969,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4589,x:32217,y:32124,varname:node_4589,prsc:2|A-8393-RGB,B-5849-OUT,C-5849-OUT,D-5849-OUT;n:type:ShaderForge.SFN_Tex2d,id:5613,x:30933,y:33091,ptovrint:False,ptlb:Animated Normal Details,ptin:_AnimatedNormalDetails,varname:node_5613,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-6012-OUT;n:type:ShaderForge.SFN_Vector3,id:5238,x:30933,y:32991,varname:node_5238,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:5650,x:31217,y:32976,varname:node_5650,prsc:2|A-5238-OUT,B-5613-RGB,T-8425-OUT;n:type:ShaderForge.SFN_Slider,id:8425,x:30796,y:33299,ptovrint:False,ptlb:Normal Anim Intensity,ptin:_NormalAnimIntensity,varname:node_8425,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:6503,x:31883,y:32532,ptovrint:False,ptlb:Emissive Intensity,ptin:_EmissiveIntensity,varname:node_6503,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:1;n:type:ShaderForge.SFN_Multiply,id:7250,x:32463,y:32277,varname:node_7250,prsc:2|A-4589-OUT,B-6036-OUT;n:type:ShaderForge.SFN_Lerp,id:7943,x:31589,y:32822,varname:node_7943,prsc:2|A-179-UVOUT,B-5650-OUT,T-2758-OUT;n:type:ShaderForge.SFN_TexCoord,id:179,x:30935,y:32821,varname:node_179,prsc:2,uv:2;n:type:ShaderForge.SFN_Slider,id:2758,x:31381,y:33035,ptovrint:False,ptlb:Normal Density,ptin:_NormalDensity,varname:node_2758,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_Panner,id:5898,x:30366,y:33000,varname:node_5898,prsc:2,spu:1,spv:1|UVIN-179-UVOUT,DIST-7384-OUT;n:type:ShaderForge.SFN_Time,id:9568,x:29815,y:32773,varname:node_9568,prsc:2;n:type:ShaderForge.SFN_Lerp,id:7384,x:30063,y:32925,varname:node_7384,prsc:2|A-9568-TSL,B-9568-T,T-8816-OUT;n:type:ShaderForge.SFN_Slider,id:8816,x:29714,y:33034,ptovrint:False,ptlb:Animation Speed,ptin:_AnimationSpeed,varname:node_8816,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_Add,id:7429,x:32282,y:33042,varname:node_7429,prsc:2|A-954-OUT,B-4418-OUT;n:type:ShaderForge.SFN_Tex2d,id:8338,x:31327,y:33378,ptovrint:False,ptlb:Normal ,ptin:_Normal,varname:_Normal_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Vector3,id:6568,x:31327,y:33278,varname:node_6568,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:8321,x:31190,y:33586,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:_NormalIntensity_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:4418,x:31611,y:33263,varname:node_4418,prsc:2|A-6568-OUT,B-8338-RGB,T-8321-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:954,x:32141,y:32765,ptovrint:False,ptlb:Invert Effect,ptin:_InvertEffect,varname:node_5212,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7943-OUT,B-518-OUT;n:type:ShaderForge.SFN_RemapRange,id:518,x:31824,y:32953,varname:node_518,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7943-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:6012,x:30608,y:33121,ptovrint:False,ptlb:Switch Animation Flow,ptin:_SwitchAnimationFlow,varname:_SwitchAnimation_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5898-UVOUT,B-9892-UVOUT;n:type:ShaderForge.SFN_Panner,id:9892,x:30366,y:33138,varname:node_9892,prsc:2,spu:-1,spv:-1|UVIN-179-UVOUT,DIST-7384-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:6036,x:32286,y:32424,varname:node_6036,prsc:2,a:0,b:2|IN-6503-OUT;proporder:8393-3489-6503-8338-8321-2758-5613-8425-8816-954-6012;pass:END;sub:END;*/

Shader "Ciconia Studio/Effects/Ghost/Old version(1.2)/Ghost Animated Details" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _ReflectionEdges ("Reflection Edges", Range(0, 1)) = 0.5
        _EmissiveIntensity ("Emissive Intensity", Range(0, 1)) = 0.25
        _Normal ("Normal ", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 1
        _NormalDensity ("Normal Density", Range(0, 1)) = 0.2
        _AnimatedNormalDetails ("Animated Normal Details", 2D) = "bump" {}
        _NormalAnimIntensity ("Normal Anim Intensity", Range(0, 1)) = 1
        _AnimationSpeed ("Animation Speed", Range(0, 1)) = 0.2
        [MaterialToggle] _InvertEffect ("Invert Effect", Float ) = 0
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
            uniform sampler2D _AnimatedNormalDetails; uniform float4 _AnimatedNormalDetails_ST;
            uniform float _NormalAnimIntensity;
            uniform float _EmissiveIntensity;
            uniform float _NormalDensity;
            uniform float _AnimationSpeed;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _NormalIntensity;
            uniform fixed _InvertEffect;
            uniform fixed _SwitchAnimationFlow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                float3 _AnimatedNormalDetails_var = UnpackNormal(tex2D(_AnimatedNormalDetails,TRANSFORM_TEX(_SwitchAnimationFlow_var, _AnimatedNormalDetails)));
                float3 node_7943 = lerp(float3(i.uv2,0.0),lerp(float3(0,0,1),_AnimatedNormalDetails_var.rgb,_NormalAnimIntensity),_NormalDensity);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = (lerp( node_7943, (node_7943*2.0+-1.0), _InvertEffect )+lerp(float3(0,0,1),_Normal_var.rgb,_NormalIntensity));
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
            uniform sampler2D _AnimatedNormalDetails; uniform float4 _AnimatedNormalDetails_ST;
            uniform float _NormalAnimIntensity;
            uniform float _EmissiveIntensity;
            uniform float _NormalDensity;
            uniform float _AnimationSpeed;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _NormalIntensity;
            uniform fixed _InvertEffect;
            uniform fixed _SwitchAnimationFlow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                float3 _AnimatedNormalDetails_var = UnpackNormal(tex2D(_AnimatedNormalDetails,TRANSFORM_TEX(_SwitchAnimationFlow_var, _AnimatedNormalDetails)));
                float3 node_7943 = lerp(float3(i.uv2,0.0),lerp(float3(0,0,1),_AnimatedNormalDetails_var.rgb,_NormalAnimIntensity),_NormalDensity);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = (lerp( node_7943, (node_7943*2.0+-1.0), _InvertEffect )+lerp(float3(0,0,1),_Normal_var.rgb,_NormalIntensity));
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
