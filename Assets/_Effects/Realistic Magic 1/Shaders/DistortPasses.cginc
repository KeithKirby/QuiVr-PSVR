﻿
	sampler2D _GrabTexture;
	sampler2D _NormalTex;
	float4 _NormalTex_ST;
	half4 _MainColor;
	half _Distortion;
	half _RefractiveStrength;
	half _InvFade;

	sampler2D _HeightTex;
	float4 _HeightTex_ST;
	half4 _HeightUVScrollDistort;
	half _Height;

	half4 _FresnelColor;
	half _FresnelPow;
	half _FresnelR0;
	half _FresnelDistort;

	sampler2D _CutoutTex;
	float4 _CutoutTex_ST;
	half _Cutout;
	half4 _CutoutColor;
	half _CutoutThreshold;
	half _AlphaClip;

	sampler2D _CameraDepthTexture;

	struct appdata
	{
		float4 vertex : POSITION;
#if defined (USE_HEIGHT) || defined (USE_REFRACTIVE) || defined (USE_FRESNEL) 
		float3 normal : NORMAL;
#endif
#ifdef USE_REFRACTIVE
		float4 tangent : TANGENT;
#endif
		half4 color : COLOR;
#ifdef USE_BLENDING
	#if UNITY_VERSION == 600
		float4 uv : TEXCOORD0;
		float texcoordBlend : TEXCOORD1;
	#else
		float2 uv : TEXCOORD0;
		float4 texcoordBlendFrame : TEXCOORD1;
	#endif
#else
		float2 uv : TEXCOORD0;
#endif
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		half4 color : COLOR;
#ifdef USE_BLENDING
		float4 uv : TEXCOORD0;
		fixed blend : TEXCOORD1;
#else
		float2 uv : TEXCOORD0;
#endif
		half4 uvgrab : TEXCOORD2;
		UNITY_FOG_COORDS(3)
#ifdef USE_CUTOUT
		float2 uvCutout : TEXCOORD4;	
#endif
#if defined (USE_SOFT_PARTICLES)  && defined (SOFTPARTICLES_ON)
		float4 projPos : TEXCOORD5;
#endif
#ifdef USE_FRESNEL
		#if  defined (USE_HEIGHT)
			half4 localPos : TEXCOORD6;
			half3 viewDir : TEXCOORD7;
		#else
			half fresnel : TEXCOORD6;
		#endif
#endif

	};
			
	v2f vert (appdata v)
	{
		v2f o;
		float2 offset = 0;
#ifdef USE_HEIGHT
		offset = _Time.xx * _HeightUVScrollDistort.xy;
#endif

#ifdef USE_BLENDING
	#if UNITY_VERSION == 600
		o.uv.xy = TRANSFORM_TEX(v.uv.xy, _NormalTex) + offset;
		o.uv.zw = TRANSFORM_TEX(v.uv.zw, _NormalTex) + offset;
		o.blend = v.texcoordBlend;
	#else
		o.uv.xy = TRANSFORM_TEX(v.uv, _NormalTex) + offset;
		o.uv.zw = TRANSFORM_TEX(v.texcoordBlendFrame.xy, _NormalTex) + offset;
		o.blend = v.texcoordBlendFrame.z;
	#endif
#else
	o.uv.xy = TRANSFORM_TEX(v.uv, _NormalTex) + offset;
#endif

#ifdef USE_HEIGHT
		float4 uv2 = float4(TRANSFORM_TEX(v.uv, _HeightTex) + offset, 0, 0);
		float4 tex = tex2Dlod (_HeightTex,  uv2);
		v.vertex.xyz += v.normal * _Height * tex - v.normal * _Height / 2;
#endif

#ifdef USE_CUTOUT
		//o.uvCutout = TRANSFORM_TEX(v.uv, _CutoutTex);
		o.uvCutout = (v.vertex.xz - 0.5) * _CutoutTex_ST.xy + _CutoutTex_ST.zw;
#endif

		o.vertex = UnityObjectToClipPos(v.vertex);
		o.color = v.color;

		/////////////////////////////////////// GRABPASS ////////////////////////////////////////
		
		o.uvgrab.xy = GrabScreenPosXY(o.vertex);
		
#ifdef USE_REFRACTIVE
		float3 binormal = cross(v.normal, v.tangent.xyz) * v.tangent.w;
		float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal);
		o.uvgrab.xy += refract(normalize(mul(rotation, ObjSpaceViewDir(v.vertex))), 0, _RefractiveStrength) * v.color.a * v.color.a;
#endif
		o.uvgrab.zw = o.vertex.w;
#if UNITY_SINGLE_PASS_STEREO
		o.uvgrab.xy = TransformStereoScreenSpaceTex(o.uvgrab.xy, o.uvgrab.w);
#endif
		o.uvgrab.z /= distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));

#if defined (USE_SOFT_PARTICLES)  && defined (SOFTPARTICLES_ON)
		o.projPos = ComputeScreenPos (o.vertex);
		COMPUTE_EYEDEPTH(o.projPos.z);
#endif
		////////////////////////////////////////////////////////////////////////////////////////////

#ifdef USE_FRESNEL
	#if  defined (USE_HEIGHT)
		o.localPos = v.vertex;
		o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
	#else
		o.fresnel = (1 - abs(dot(normalize(v.normal), normalize(ObjSpaceViewDir(v.vertex)))));
		o.fresnel = pow(o.fresnel, _FresnelPow);
		o.fresnel = saturate(_FresnelR0 + (1.0 - _FresnelR0) * o.fresnel);
	#endif
#endif


		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}
			
	half4 frag (v2f i) : SV_Target
	{
#if DISTORT_OFF
		return 0;
#endif

#if defined (USE_SOFT_PARTICLES)  && defined (SOFTPARTICLES_ON)
		float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
		float partZ = i.projPos.z;
		float fade = saturate (_InvFade * (sceneZ-partZ));
		float fadeStep = step(0.001, _InvFade);
		i.color.a *= lerp(1, fade, step(0.001, _InvFade));
#endif

#ifdef USE_BLENDING
		half4 dist1 = tex2D(_NormalTex, i.uv.xy);
		half4 dist2 = tex2D(_NormalTex, i.uv.zw);
		half3 dist = UnpackNormal(lerp(dist1, dist2, i.blend));
#else
		half3 dist = UnpackNormal(tex2D(_NormalTex, i.uv));
#endif
		
#ifdef USE_ALPHA_CLIPING
		half alphaBump = saturate((abs(dist.r + dist.g) - 0.01) * _AlphaClip);
		clip(step(0.5, alphaBump) - 0.1);

#endif

		half2 texelSize = GetGrabTexelSize();
		half2 offset = dist.rg * _Distortion * texelSize;
		
		half3 fresnelCol = 0;
#ifdef USE_FRESNEL
		#if  defined (USE_HEIGHT)
			half3 n = -normalize(cross(ddx(i.localPos.xyz), -ddy(i.localPos.xyz) * _ProjectionParams.x ));
			half fresnel = (1 - dot(normalize(n), i.viewDir));
			fresnel = pow(fresnel, _FresnelPow);
			fresnel = saturate(_FresnelR0 + (1.0 - _FresnelR0) * fresnel);
			offset += fresnel * texelSize * _FresnelDistort * dist.rg ;
			fresnelCol = _FresnelColor * fresnel * abs(dist.r + dist.g) * 2  * i.color.rgb * i.color.a;
		#else
			offset += i.fresnel * texelSize * _FresnelDistort * dist.rg;
			fresnelCol = _FresnelColor *  i.fresnel * abs(dist.r + dist.g) * 2  * i.color.rgb * i.color.a;
		#endif
#endif

		half4 cutoutCol = 0;
		cutoutCol.a = 1;
#ifdef USE_CUTOUT
		half cutoutAlpha = tex2D(_CutoutTex, i.uvCutout).r - (dist.r + dist.g) / 10;
		half alpha = step(0, (_Cutout - cutoutAlpha));
		half alpha2 = step(0, (_Cutout - cutoutAlpha + _CutoutThreshold));
		cutoutCol.rgb = _CutoutColor * saturate(alpha2 - alpha);
		cutoutCol.a = alpha2 * pow(_Cutout, 0.2);
#endif
		
#ifdef USE_ALPHA_CLIPING
		offset *= alphaBump;
#endif
		i.uvgrab.xy = offset * i.uvgrab.z * i.color.a + i.uvgrab.xy;
		half4 grabColor = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));;
		
		half4 result;
		result.rgb = grabColor * lerp((half4)1, _MainColor,  i.color.a) + fresnelCol * grabColor + cutoutCol.rgb;
		result.a = _MainColor.a * cutoutCol.a;
#ifdef USE_ALPHA_CLIPING
		result.a *= alphaBump;
#endif
		
		//UNITY_APPLY_FOG(i.fogCoord, result);
		result.a = saturate(result.a);
		return result;
	}