// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:6535,x:32719,y:32712,varname:node_6535,prsc:2|diff-7250-OUT,normal-5212-OUT,emission-7250-OUT,custl-8367-RGB;n:type:ShaderForge.SFN_Slider,id:3489,x:31186,y:32523,ptovrint:False,ptlb:Fresnel Strength,ptin:_FresnelStrength,varname:_ReflectionGlow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Vector1,id:8462,x:31322,y:32293,varname:node_8462,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:4617,x:31322,y:32221,varname:node_4617,prsc:2,v1:10;n:type:ShaderForge.SFN_Lerp,id:7528,x:31554,y:32321,varname:node_7528,prsc:2|A-4617-OUT,B-8462-OUT,T-3489-OUT;n:type:ShaderForge.SFN_Fresnel,id:5849,x:31749,y:32321,varname:node_5849,prsc:2|EXP-7528-OUT;n:type:ShaderForge.SFN_Color,id:8393,x:31993,y:31969,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4589,x:32217,y:32124,varname:node_4589,prsc:2|A-8393-RGB,B-5849-OUT,C-5849-OUT,D-5849-OUT;n:type:ShaderForge.SFN_Tex2d,id:5613,x:31632,y:33016,ptovrint:False,ptlb:Normal map (Animated),ptin:_NormalmapAnimated,varname:node_5613,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-9256-OUT;n:type:ShaderForge.SFN_Vector3,id:5238,x:31648,y:32913,varname:node_5238,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:5650,x:32023,y:32874,varname:node_5650,prsc:2|A-5238-OUT,B-5613-RGB,T-8425-OUT;n:type:ShaderForge.SFN_Slider,id:8425,x:31511,y:33221,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_8425,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:6503,x:31869,y:32584,ptovrint:False,ptlb:Emissive Intensity,ptin:_EmissiveIntensity,varname:node_6503,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:1;n:type:ShaderForge.SFN_Multiply,id:7250,x:32463,y:32277,varname:node_7250,prsc:2|A-4589-OUT,B-3773-OUT;n:type:ShaderForge.SFN_TexCoord,id:179,x:30465,y:32478,varname:node_179,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:5898,x:31070,y:32870,varname:node_5898,prsc:2,spu:1,spv:1|UVIN-7154-UVOUT,DIST-7384-OUT;n:type:ShaderForge.SFN_Time,id:9568,x:30479,y:33020,varname:node_9568,prsc:2;n:type:ShaderForge.SFN_Lerp,id:7384,x:30747,y:33109,varname:node_7384,prsc:2|A-9568-TSL,B-9568-T,T-8816-OUT;n:type:ShaderForge.SFN_Slider,id:8816,x:30435,y:33316,ptovrint:False,ptlb:Animation Speed,ptin:_AnimationSpeed,varname:node_8816,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_RemapRange,id:6566,x:32365,y:33099,varname:node_6566,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-5650-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:5212,x:32484,y:32872,ptovrint:False,ptlb:Invert Effect,ptin:_InvertEffect,varname:node_5212,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-5650-OUT,B-6566-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9256,x:31300,y:32942,ptovrint:False,ptlb:Switch Animation Flow,ptin:_SwitchAnimationFlow,varname:node_9034,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5898-UVOUT,B-1006-UVOUT;n:type:ShaderForge.SFN_Panner,id:1006,x:31070,y:33004,varname:node_1006,prsc:2,spu:-1,spv:-1|UVIN-7154-UVOUT,DIST-7384-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:3773,x:32299,y:32463,varname:node_3773,prsc:2,a:0,b:2|IN-6503-OUT;n:type:ShaderForge.SFN_ScreenPos,id:8831,x:32101,y:33446,varname:node_8831,prsc:2,sctp:2;n:type:ShaderForge.SFN_SceneColor,id:8367,x:32385,y:33398,varname:node_8367,prsc:2|UVIN-8831-UVOUT;n:type:ShaderForge.SFN_Rotator,id:7154,x:30585,y:32743,varname:node_7154,prsc:2|UVIN-179-UVOUT,ANG-7231-OUT;n:type:ShaderForge.SFN_Slider,id:7231,x:30241,y:32885,ptovrint:False,ptlb:Rotation,ptin:_Rotation,varname:node_7231,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:6;proporder:8393-3489-6503-5613-8425-8816-5212-9256-7231;pass:END;sub:END;*/

Shader "Ciconia Studio/Effects/Ghost/Ghost Animated" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _FresnelStrength ("Fresnel Strength", Range(0, 1)) = 0.5
        _EmissiveIntensity ("Emissive Intensity", Range(0, 1)) = 0.25
        _NormalmapAnimated ("Normal map (Animated)", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 2)) = 1
        _AnimationSpeed ("Animation Speed", Range(0, 1)) = 0.2
        [MaterialToggle] _InvertEffect ("Invert Effect", Float ) = -1
        [MaterialToggle] _SwitchAnimationFlow ("Switch Animation Flow", Float ) = 0
        _Rotation ("Rotation", Range(0, 6)) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float _FresnelStrength;
            uniform float4 _Color;
            uniform sampler2D _NormalmapAnimated; uniform float4 _NormalmapAnimated_ST;
            uniform float _NormalIntensity;
            uniform float _EmissiveIntensity;
            uniform float _AnimationSpeed;
            uniform fixed _InvertEffect;
            uniform fixed _SwitchAnimationFlow;
            uniform float _Rotation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_9568 = _Time + _TimeEditor;
                float node_7384 = lerp(node_9568.r,node_9568.g,_AnimationSpeed);
                float node_7154_ang = _Rotation;
                float node_7154_spd = 1.0;
                float node_7154_cos = cos(node_7154_spd*node_7154_ang);
                float node_7154_sin = sin(node_7154_spd*node_7154_ang);
                float2 node_7154_piv = float2(0.5,0.5);
                float2 node_7154 = (mul(i.uv0-node_7154_piv,float2x2( node_7154_cos, -node_7154_sin, node_7154_sin, node_7154_cos))+node_7154_piv);
                float2 _SwitchAnimationFlow_var = lerp( (node_7154+node_7384*float2(1,1)), (node_7154+node_7384*float2(-1,-1)), _SwitchAnimationFlow );
                float3 _NormalmapAnimated_var = UnpackNormal(tex2D(_NormalmapAnimated,TRANSFORM_TEX(_SwitchAnimationFlow_var, _NormalmapAnimated)));
                float3 node_5650 = lerp(float3(0,0,1),_NormalmapAnimated_var.rgb,_NormalIntensity);
                float3 normalLocal = lerp( node_5650, (node_5650*2.0+-1.0), _InvertEffect );
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float node_5849 = pow(1.0-max(0,dot(normalDirection, viewDirection)),lerp(10.0,0.0,_FresnelStrength));
                float3 node_7250 = ((_Color.rgb*node_5849*node_5849*node_5849)*lerp(0,2,_EmissiveIntensity));
                float3 emissive = node_7250;
                float3 finalColor = emissive + tex2D( _GrabTexture, sceneUVs.rg).rgb;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
