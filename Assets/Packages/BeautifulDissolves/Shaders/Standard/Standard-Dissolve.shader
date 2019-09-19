// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Beautiful Dissolves/Standard Dissolve"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
		_ParallaxMap ("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
		
		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

		_DissolveMap ("Dissolve Map", 2D) = "white" {}
		_TilingX("X", Float) = 1.0
		_TilingY("Y", Float) = 1.0
		_DissolveAmount ("Dissolve Amount", Range(-2.0, 2.0)) = 0.5
		_DirectionMap ("Direction Map", 2D) = "white" {}
		_SubTex ("Substitute Texture", 2D) = "black" {}
		[Toggle(_DISSOLVEGLOW_ON)] _DissolveGlow ("Dissolve Glow", Int) = 1
		_GlowColor ("Glow Color", Color) = (1,0.5,0,1)
		_GlowIntensity ("Glow Intensity", Float) = 7
		_OuterEdgeColor ("Outer Edge Color", Color) = (1,0,0,1)
		_InnerEdgeColor ("Inner Edge Color", Color) = (1,1,0,1)
		_OuterEdgeThickness ("Outer Edge Thickness", Range(0.0, 1.0)) = 0.02
		_InnerEdgeThickness ("Inner Edge Thickness", Range(0.0, 1.0)) = 0.04
		[Toggle(_EDGEGLOW_ON)] _EdgeGlow ("Edge Glow", Int) = 1
		[Toggle(_COLORBLENDING_ON)] _ColorBlending ("Color Blending", Int) = 1
		[Toggle(_GLOWFOLLOW_ON)] _GlowFollow ("Follow-Through", Int) = 0
		
		_EdgeColorRamp ("Edge Color Ramp", 2D) = "white" {}
		[Toggle(_EDGECOLORRAMP_USE)] _UseEdgeColorRamp("Use Edge Color Ramp", Int) = 0
		
		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	CGINCLUDE
		#define UNITY_SETUP_BRDF_INPUT MetallicSetup
	ENDCG

	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 300
	

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------
			
#pragma shader_feature _SUBMAP
#pragma shader_feature _DIRECTIONMAP
#pragma shader_feature _EDGEGLOW_ON
#pragma shader_feature _COLORBLENDING_ON
#pragma shader_feature _EDGECOLORRAMP_USE
#pragma shader_feature _DISSOLVEGLOW_ON
#pragma shader_feature _GLOWFOLLOW_ON
#pragma multi_compile __ _DISSOLVEMAP

#pragma shader_feature __ _NORMALMAP
#pragma shader_feature __ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature __ _EMISSION
#pragma shader_feature __ _METALLICGLOSSMAP
#pragma shader_feature __ _DETAIL_MULX2
#pragma shader_feature __ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature __ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature __ _GLOSSYREFLECTIONS_OFF
// Disabled by Shader Control: #pragma shader_feature __ _PARALLAXMAP

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#pragma vertex vertBase
			#pragma fragment fragBase
			#include "DissolveStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------

#pragma shader_feature __ _SUBMAP
#pragma shader_feature __ _DIRECTIONMAP
#pragma shader_feature __ _COLORBLENDING_ON
#pragma shader_feature __ _EDGECOLORRAMP_USE
#pragma multi_compile __ _DISSOLVEMAP

#pragma shader_feature __ _NORMALMAP
#pragma shader_feature __ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature __ _METALLICGLOSSMAP
#pragma shader_feature __ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature __ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature __ _DETAIL_MULX2
// Disabled by Shader Control: #pragma shader_feature __ _PARALLAXMAP

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog


			#pragma vertex vertAdd
			#pragma fragment fragAdd
			#include "DissolveStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------

#pragma shader_feature __ _SUBMAP
#pragma shader_feature __ _DIRECTIONMAP
#pragma multi_compile __ _DISSOLVEMAP

#pragma shader_feature __ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature __ _METALLICGLOSSMAP
// Disabled by Shader Control: #pragma shader_feature __ _PARALLAXMAP
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "DissolveStandardShadow.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Deferred pass
		Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }
			Cull Off

			CGPROGRAM
			#pragma target 3.0
			#pragma exclude_renderers nomrt


			// -------------------------------------

#pragma shader_feature __ _SUBMAP
#pragma shader_feature __ _DIRECTIONMAP
#pragma shader_feature __ _EDGEGLOW_ON
#pragma shader_feature __ _COLORBLENDING_ON
#pragma shader_feature __ _EDGECOLORRAMP_USE
#pragma shader_feature __ _DISSOLVEGLOW_ON
#pragma shader_feature __ _GLOWFOLLOW_ON
#pragma multi_compile __ _DISSOLVEMAP

#pragma shader_feature __ _NORMALMAP
#pragma shader_feature __ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature __ _EMISSION
#pragma shader_feature __ _METALLICGLOSSMAP
#pragma shader_feature __ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature __ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature __ _DETAIL_MULX2
// Disabled by Shader Control: #pragma shader_feature __ _PARALLAXMAP

			#pragma multi_compile_prepassfinal
			#pragma multi_compile_instancing

			#pragma vertex vertDeferred
			#pragma fragment fragDeferredDissolve

			#include "DissolveStandardCore.cginc"

			ENDCG
		}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			Name "META" 
			Tags { "LightMode"="Meta" }
			Cull Off
			
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_meta
			#pragma fragment frag_meta

#pragma shader_feature __ _SUBMAP
#pragma shader_feature __ _DIRECTIONMAP
#pragma shader_feature __ _EDGEGLOW_ON
#pragma shader_feature __ _DISSOLVEGLOW_ON
#pragma shader_feature __ _GLOWFOLLOW_ON
#pragma multi_compile __ _DISSOLVEMAP

#pragma shader_feature __ _EMISSION
#pragma shader_feature __ _METALLICGLOSSMAP
#pragma shader_feature __ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature __ _DETAIL_MULX2
#pragma shader_feature __ EDITOR_VISUALIZATION

			#include "DissolveStandardMeta.cginc"
			ENDCG
		}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 150
		Cull Off

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM
			#pragma target 2.0

#pragma shader_feature __ _NORMALMAP
#pragma shader_feature __ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature __ _EMISSION
#pragma shader_feature __ _METALLICGLOSSMAP
#pragma shader_feature __ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature __ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature __ _GLOSSYREFLECTIONS_OFF
			// SM2.0: NOT SUPPORTED shader_feature ___ _DETAIL_MULX2
			// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP

			#pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			#pragma vertex vertBase
			#pragma fragment fragBase
			#include "DissolveStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual
			
			CGPROGRAM
			#pragma target 2.0

#pragma shader_feature __ _NORMALMAP
#pragma shader_feature __ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature __ _METALLICGLOSSMAP
#pragma shader_feature __ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature __ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature __ _DETAIL_MULX2
			// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP
			#pragma skip_variants SHADOWS_SOFT
			
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			
			#pragma vertex vertAdd
			#pragma fragment fragAdd
			#include "DissolveStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 2.0

#pragma shader_feature __ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature __ _METALLICGLOSSMAP
			#pragma skip_variants SHADOWS_SOFT
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "DissolveStandardShadow.cginc"

			ENDCG
		}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			Name "META" 
			Tags { "LightMode"="Meta" }

			Cull Off

			CGPROGRAM
			#pragma vertex vert_meta
			#pragma fragment frag_meta

#pragma shader_feature __ _EMISSION
#pragma shader_feature __ _METALLICGLOSSMAP
#pragma shader_feature __ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature __ _DETAIL_MULX2
#pragma shader_feature __ EDITOR_VISUALIZATION

			#include "DissolveStandardMeta.cginc"
			ENDCG
		}
	}


	FallBack "VertexLit"
	CustomEditor "StandardDissolveShaderGUI"
}
