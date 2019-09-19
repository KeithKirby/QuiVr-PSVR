Shader "Custom/CubeRefl" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Cube("Cubemap", CUBE) = "" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
#pragma surface surf Lambert
		struct Input {
		float3 worldRefl;
	};
	samplerCUBE _Cube;
	float4 _Color;
	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = texCUBE(_Cube, IN.worldRefl).rgb;
		o.Albedo *= _Color;
		o.Emission = texCUBE(_Cube, IN.worldRefl).rgb;
	}
	ENDCG
	}
		Fallback "Diffuse"
}