Shader "Custom/VertexColor" {
	Properties{
		_Splat1("Base (RGB)", 2D) = "white" {}
		_BumpMap1("Bump1", 2D) = "bump" {}
		_Splat2("Base (RGB)", 2D) = "white" {}
		_BumpMap2("Bump2", 2D) = "bump" {}
		_Splat3("Base (RGB)", 2D) = "white" {}
		_BumpMap3("Bump3", 2D) = "bump" {}
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma target 3.0
#pragma surface surf BlinnPhong 

		sampler2D _Splat1;
		sampler2D _Splat2;
		sampler2D _Splat3;
		sampler2D _BumpMap1;
		sampler2D _BumpMap2;
		sampler2D _BumpMap3;

	struct Input {
		float2 uv_Splat1;
		float2 uv_Splat2;
		float2 uv_Splat3;
		float2 uv_BumpMap1;
		float2 uv_BumpMap2;
		float2 uv_BumpMap3;
		float4 color: Color; // Vertex color
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 splat1 = tex2D(_Splat1, IN.uv_Splat1);
		half4 splat2 = tex2D(_Splat2, IN.uv_Splat2);
		half4 splat3 = tex2D(_Splat3, IN.uv_Splat3);
		half4 normal1 = tex2D(_BumpMap1, IN.uv_BumpMap1);
		half4 normal2 = tex2D(_BumpMap2, IN.uv_BumpMap2);
		half4 normal3 = tex2D(_BumpMap3, IN.uv_BumpMap3);
		fixed3 albedo = lerp(splat1.rgb, splat2.rgb, IN.color.g);
		albedo = lerp(albedo, splat3.rgb, IN.color.b);
		fixed3 normal = lerp(normal1.rgb, normal2.rgb, IN.color.g);
		normal = lerp(normal, normal3.rgb, IN.color.b);
		o.Albedo = albedo;
		o.Normal = normal;

	}
	ENDCG
	}
		FallBack "Diffuse"
}