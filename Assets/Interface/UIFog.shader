// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/ClipArea"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "white" {}
		_Fill("Fill", Range(0.0, 1.0)) = 1.0
		_Color("Tint", Color) = (1,1,1,1)
		_MinX("MinX", Float) = 0
		_MaxX("MaxX", Float) = 1
	}

		SubShader
	{
		LOD 200

		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}

		Pass
	{
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		sampler2D _MainTex;
	float4 _MainTex_ST;
	float _MinX;
	float _MaxX;
	float _Fill;

	struct appdata_t
	{
		float4 vertex : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	fixed4 _Color;

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord;
		return o;
	}

	half4 frag(v2f IN) : COLOR
	{
		if ((IN.texcoord.x<_MinX) || (IN.texcoord.x>(_MinX + _Fill*(_MaxX - _MinX))))
		{
			half4 colorTransparent = half4(0,0,0,0);
			return  colorTransparent;
		}
		return tex2D(_MainTex, IN.texcoord) * _Color;
	}
		ENDCG
	}
	}
}