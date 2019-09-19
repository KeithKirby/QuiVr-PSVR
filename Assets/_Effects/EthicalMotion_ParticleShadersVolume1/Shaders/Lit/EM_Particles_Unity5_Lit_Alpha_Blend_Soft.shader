﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Ethical Motion/Particles/Unity5/Lit Alpha Blend Soft Particle" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_Thickness ("Thickness Factor", Range(0.0, 0.2)) = 0.05
		_Cutoff ("Alpha cutoff", Range(0,1) ) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		Cull Back
		Zwrite Off
 		Lighting Off
		
		CGPROGRAM
		#pragma surface surf Smoke vertex:vert  fullforwardshadows nodynlightmap nodirlightmap alpha:fade 
		#pragma target 3.0
		#include "EMLighting.cginc"

		sampler2D _MainTex;
		sampler2D _CameraDepthTexture;
		fixed4 _TintColor;
		fixed _InvFade;
	
		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
			float4 projPos : TEXCOORD1;
		};
		
		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float4 pos = UnityObjectToClipPos (v.vertex);
			o.projPos = ComputeScreenPos(pos);
			COMPUTE_EYEDEPTH(o.projPos.z);
		}

		void surf (Input i, inout SurfaceOutputSmoke o) {
			float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
			float partZ = i.projPos.z;
			float fade = saturate (_InvFade * (sceneZ-partZ));
			i.color.a *= fade;
			
			fixed4 c = tex2D(_MainTex, i.uv_MainTex) * _TintColor * i.color;
			o.Albedo = 2 * c.rgb;
			o.Alpha = 2 * c.a;
		}
		ENDCG
	} 
	Fallback "Hidden/Ethical Motion/Particles/Lit Alpha Blend Shadow Fallback"
}
