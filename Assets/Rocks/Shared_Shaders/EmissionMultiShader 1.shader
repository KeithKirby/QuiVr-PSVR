Shader "Custom/EmissionMultiTexture" {
	Properties{
		[Header(Sub Material 0 (Base Sub Material))]
	_diff0("Diffuse", 2D) = "white" {}
	[NoScaleOffset][Normal] _norm0("Normal", 2D) = "" {}
	_np0("Normal Power", Range(0.01,1)) = 1.0

		[Header(Sub Material 1)]
	[NoScaleOffset]_mask1("Mask", 2D) = "black" {}
		 _diff1("Diffuse", 2D) = "white" {}
	[NoScaleOffset][Normal] _norm1("Normal", 2D) = "" {}
	_emiss1("Emission Map", 2D) = "black" {}
	[HDR] _emisscolor1("Emission Color", Color) = (0,0,0)
	_np1("Normal Power", Range(0,1)) = 1.0

	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
//#pragma surface surf Standard fullforwardshadows
#pragma surface surf Lambert noforwardadd nometa //fullforwardshadows --Added noforwardadd nometa
#pragma target 3.0

		// Material 0 (Base Material)
		sampler2D _diff0;
	sampler2D _norm0;
	float _np0;

	// Material 1
	sampler2D _mask1;
	sampler2D _diff1;
	sampler2D _norm1;
	sampler2D _emiss1;
	fixed4 _emisscolor1;
	float _np1;

	struct Input {
		float2 uv_diff0;
		float2 uv_diff1;
		float2 uv_mask1;
		float2 uv_emiss1;
	};

	// Surface Function
	void surf(Input IN, inout SurfaceOutput output) {

		float3 grayscalar = float3(0.3, 0.59, 0.11);

		//###################
		// TILING
		//###################
		float2 uv_final_mask1 = IN.uv_mask1;

		//###################
		// DIFFUSE
		//###################
		half4 d0 = tex2D(_diff0, IN.uv_diff0);
		half4 d1 = tex2D(_diff1, IN.uv_diff1);
		fixed3 diffuse = lerp(
			d0.rgb,
			d1.rgb,
			dot(tex2D(_mask1, uv_final_mask1).rgb, grayscalar) * tex2D(_diff1, IN.uv_mask1).a
		);

		//###################
		// NORMAL
		//###################
		fixed3 normal = lerp(
			UnpackScaleNormal(tex2D(_norm0, IN.uv_diff0), _np0).rgb,
			UnpackScaleNormal(tex2D(_norm1, IN.uv_diff1), _np1).rgb,
			dot(tex2D(_mask1, uv_final_mask1).rgb, grayscalar)
		);

		//###################
		// Metallic
		//###################
		half metal = 0;

		//###################
		// Gloss
		//###################
		half gloss = 0;

		fixed3 emissive = lerp(
			fixed3(0,0,0),
			(tex2D(_emiss1, IN.uv_emiss1)*_emisscolor1).rgb,
			dot(tex2D(_mask1, uv_final_mask1).rgb, grayscalar)
		);

		//###################
		// Output
		//###################
		output.Albedo = diffuse;
		output.Normal = normal;
		output.Specular = metal;
		output.Gloss = gloss;
		output.Emission = emissive;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
