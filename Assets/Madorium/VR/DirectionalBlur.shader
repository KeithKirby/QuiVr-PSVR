Shader "Madorium/Screen/DirectionalBlur"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BlurAmount("BlurAmount", Float) = 0.05
	}

	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE

#include "UnityCG.cginc"

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 blurVector : TEXCOORD1;
	};

	sampler2D _MainTex;
	float _BlurAmount;

	half4 _MainTex_ST;

	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.blurVector = float2(_BlurAmount, 0);
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 color = half4(0,0,0,0);

		color += tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv.xy, _MainTex_ST)) * 0.390524f;
		i.uv.xy += i.blurVector;
		color += tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv.xy, _MainTex_ST)) * 0.276142f;
		i.uv.xy += i.blurVector;
		color += tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv.xy, _MainTex_ST)) * 0.195262f;
		i.uv.xy += i.blurVector;
		color += tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv.xy, _MainTex_ST)) * 0.138071f;
		return color;
	}

	ENDCG

	Subshader
	{
		Blend One Zero
			Pass{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag

			ENDCG
		} // Pass
	} // Subshader

	Fallback off

} // shader