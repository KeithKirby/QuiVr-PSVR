// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/EmissiveMove" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_NormalPower("Normal Power", Range(0,2)) = 1.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Emission("Emission", 2D) = "black" {}
		[HDR]_EmColor("Emission Color", Color) = (0,0,0,0)
		_XScrollSpeed("X Scroll Speed", Float) = 1  
		_YScrollSpeed("Y Scroll Speed", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		#pragma shader_feature ANIMATE

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Normal;
		sampler2D _Emission;
		struct Input {
			float2 uv_MainTex;
			float2 uv_Emission;
			float2 uv_Normal;
		};

		half _Glossiness;
		half _Metallic;
		half _NormalPower;
		fixed4 _Color;
		fixed4 _EmColor;

		float _XScrollSpeed;
		float _YScrollSpeed;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			half x = -_NormalPower + 1;
			o.Normal = lerp(UnpackNormal(tex2D(_Normal, IN.uv_Normal)), fixed3(0, 0, 1), x);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			fixed xScrollValue = _XScrollSpeed * _Time.x;
			fixed yScrollValue = _YScrollSpeed * _Time.x;
			fixed2 scrollUV = IN.uv_Emission;
			scrollUV += fixed2(xScrollValue, yScrollValue);
			o.Emission = tex2D(_Emission, scrollUV)* _EmColor;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
