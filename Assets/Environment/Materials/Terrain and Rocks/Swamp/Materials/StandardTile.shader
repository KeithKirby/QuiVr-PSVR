Shader "Custom/StandardTile" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_CutOff("Cut off", Range(0,1)) = 0.1
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpAmt("Normal Strength", float) = 1.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Spec("Specular Map", 2D) = "white" {}
	}
		SubShader{
		Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _Spec;
	float _CutOff;
	float _BumpAmt;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float2 uv_Spec;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed3 normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpAmt).rgb;
		o.Normal = normal;
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		fixed4 spec = tex2D(_Spec, IN.uv_Spec);
		o.Metallic = _Metallic * spec.r;
		o.Smoothness = _Glossiness * spec.a;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}