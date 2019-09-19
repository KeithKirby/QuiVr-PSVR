// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:6535,x:32701,y:32656,varname:node_6535,prsc:2|emission-7250-OUT,custl-679-RGB;n:type:ShaderForge.SFN_Slider,id:3489,x:30938,y:33152,ptovrint:False,ptlb:Fresnel Strength,ptin:_FresnelStrength,varname:_ReflectionGlow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Vector1,id:8462,x:31074,y:32922,varname:node_8462,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:4617,x:31074,y:32826,varname:node_4617,prsc:2,v1:10;n:type:ShaderForge.SFN_Lerp,id:7528,x:31306,y:32950,varname:node_7528,prsc:2|A-4617-OUT,B-8462-OUT,T-3489-OUT;n:type:ShaderForge.SFN_Fresnel,id:5849,x:31501,y:32950,varname:node_5849,prsc:2|EXP-7528-OUT;n:type:ShaderForge.SFN_Add,id:6253,x:31760,y:32825,varname:node_6253,prsc:2|A-5849-OUT,B-5849-OUT,C-5849-OUT;n:type:ShaderForge.SFN_Color,id:8393,x:31745,y:32598,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4589,x:31969,y:32753,varname:node_4589,prsc:2|A-8393-RGB,B-6253-OUT;n:type:ShaderForge.SFN_Slider,id:6503,x:31643,y:33218,ptovrint:False,ptlb:Emissive Intensity,ptin:_EmissiveIntensity,varname:node_6503,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_Multiply,id:7250,x:32133,y:32845,varname:node_7250,prsc:2|A-4589-OUT,B-5122-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:5122,x:31993,y:33024,varname:node_5122,prsc:2,a:0,b:2|IN-6503-OUT;n:type:ShaderForge.SFN_SceneColor,id:679,x:32036,y:32364,varname:node_679,prsc:2|UVIN-1956-UVOUT;n:type:ShaderForge.SFN_ScreenPos,id:1956,x:31752,y:32412,varname:node_1956,prsc:2,sctp:2;proporder:8393-3489-6503;pass:END;sub:END;*/

Shader "Ciconia Studio/Effects/Ghost/FastGhost" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _FresnelStrength ("Fresnel Strength", Range(0, 1)) = 0.5
        _EmissiveIntensity ("Emissive Intensity", Range(0, 1)) = 0.2
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float _FresnelStrength;
            uniform float4 _Color;
            uniform float _EmissiveIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float node_5849 = pow(1.0-max(0,dot(normalDirection, viewDirection)),lerp(10.0,1.0,_FresnelStrength));
                float3 emissive = ((_Color.rgb*(node_5849+node_5849+node_5849))*lerp(0,2,_EmissiveIntensity));
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
