Shader "Unlit/Particle_Opaque"
{
	Properties
	{
		[HDR]_EmissionColor("Emission Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Main Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd nometa
		#pragma target 3.0
		sampler2D _MainTex;
		half4 _EmissionColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			float4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Emission = _EmissionColor * tex;
			o.Albedo = o.Emission;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
