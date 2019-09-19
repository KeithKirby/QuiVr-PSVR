// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:DLNK Shaders/Advanced/PBRAdvOpaque,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:33456,y:32369,varname:node_2865,prsc:2|diff-4875-OUT,spec-9988-OUT,gloss-765-OUT,normal-1191-OUT,difocc-7575-OUT,spcocc-7575-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32138,y:32713,varname:node_6343,prsc:2|A-915-OUT,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31921,y:32872,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31921,y:32620,ptovrint:True,ptlb:Albedo,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32301,y:32791,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Slider,id:358,x:32327,y:32215,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:_Metallic,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:1813,x:32327,y:32338,ptovrint:True,ptlb:Gloss,ptin:_Glossiness,varname:_Glossiness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:2;n:type:ShaderForge.SFN_Multiply,id:328,x:32727,y:32604,varname:node_328,prsc:2|A-982-A,B-1813-OUT;n:type:ShaderForge.SFN_Multiply,id:688,x:32727,y:32471,varname:node_688,prsc:2|A-982-R,B-358-OUT;n:type:ShaderForge.SFN_Vector3,id:8143,x:32301,y:32993,varname:node_8143,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:7874,x:32520,y:32808,varname:node_7874,prsc:2|A-5964-RGB,B-8143-OUT,T-287-OUT;n:type:ShaderForge.SFN_ValueProperty,id:287,x:32301,y:33145,ptovrint:False,ptlb:Normal Smooth,ptin:_NormalSmooth,varname:_NormalSmooth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:9982,x:30543,y:32288,varname:node_9982,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7427,x:30393,y:32248,varname:node_7427,prsc:2|A-9982-UVOUT,B-6981-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6981,x:30222,y:32313,ptovrint:False,ptlb:UV Master Tiling,ptin:_UVMasterTiling,varname:_UVMasterTiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:982,x:32298,y:32507,ptovrint:True,ptlb:Metalness,ptin:_MetallicGlossMap,varname:_MetallicGlossMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:6982,x:31668,y:32233,ptovrint:True,ptlb:Occlusion,ptin:_OcclusionMap,varname:_OcclusionMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:6010,x:31842,y:32387,ptovrint:True,ptlb:Occlusion Mult,ptin:_OcclusionStrenght,varname:_OcclusionStrenght,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2405,x:30012,y:32746,ptovrint:True,ptlb:Depth,ptin:Depth,varname:Depth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Slider,id:2724,x:29855,y:32888,ptovrint:True,ptlb:Depth Map Smooth,ptin:_HgtMapSmooth,varname:_HgtMapSmooth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:5;n:type:ShaderForge.SFN_Tex2dAsset,id:1432,x:30237,y:32793,ptovrint:True,ptlb:Depth Map,ptin:_ParallaxMap,varname:_ParallaxMap,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4269,x:31727,y:32620,ptovrint:True,ptlb:Detail Albedo,ptin:_DetailAlbedoMap,varname:_DetailAlbedoMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1649,x:32136,y:33145,ptovrint:True,ptlb:Detail Bump,ptin:_DetailNormalMap,varname:_DetailNormalMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Lerp,id:2921,x:32520,y:32939,varname:node_2921,prsc:2|A-1649-RGB,B-8143-OUT,T-6074-OUT;n:type:ShaderForge.SFN_ValueProperty,id:590,x:32136,y:33383,ptovrint:True,ptlb:Detail Bump Smooth,ptin:_DetailNormalMapScale,varname:_DetailNormalMapScale,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:1950,x:32769,y:33297,ptovrint:False,ptlb:Detail Mask,ptin:_DetailMask,varname:_DetailMask,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Lerp,id:1487,x:32689,y:32808,varname:node_1487,prsc:2|A-7874-OUT,B-2921-OUT,T-8121-OUT;n:type:ShaderForge.SFN_Multiply,id:9828,x:32769,y:33140,varname:node_9828,prsc:2|A-1950-R,B-2573-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2573,x:32929,y:33329,ptovrint:False,ptlb:Detail Mask Mult,ptin:_DetailMaskMult,varname:_DetailMaskMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:915,x:32138,y:32842,varname:node_915,prsc:2|A-7736-RGB,B-4269-RGB,T-8121-OUT;n:type:ShaderForge.SFN_OneMinus,id:6074,x:32136,y:33490,varname:node_6074,prsc:2|IN-590-OUT;n:type:ShaderForge.SFN_Clamp01,id:8121,x:32929,y:33140,varname:node_8121,prsc:2|IN-9828-OUT;n:type:ShaderForge.SFN_Multiply,id:1224,x:31842,y:32233,varname:node_1224,prsc:2|A-6982-R,B-6010-OUT;n:type:ShaderForge.SFN_OneMinus,id:7361,x:32003,y:32387,varname:node_7361,prsc:2|IN-6010-OUT;n:type:ShaderForge.SFN_Clamp01,id:8159,x:32164,y:32338,varname:node_8159,prsc:2|IN-8758-OUT;n:type:ShaderForge.SFN_Add,id:8758,x:32003,y:32250,varname:node_8758,prsc:2|A-7361-OUT,B-1224-OUT;n:type:ShaderForge.SFN_Tex2d,id:3281,x:32270,y:31767,ptovrint:False,ptlb:Up Albedo,ptin:_UpAlbedo,varname:_UpAlbedo,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:320,x:32264,y:31452,ptovrint:False,ptlb:Up Normal,ptin:_UpNormal,varname:_UpNormal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Lerp,id:1760,x:32433,y:31452,varname:node_1760,prsc:2|A-320-RGB,B-7460-OUT,T-4876-OUT;n:type:ShaderForge.SFN_Vector3,id:7460,x:32278,y:31306,varname:node_7460,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_ValueProperty,id:4876,x:32433,y:31322,ptovrint:False,ptlb:UpNormal Smooth,ptin:_UpNormalSmooth,varname:_UpNormalSmooth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:8142,x:32461,y:31767,varname:node_8142,prsc:2|A-3281-RGB,B-616-RGB,C-7727-OUT;n:type:ShaderForge.SFN_Color,id:616,x:32426,y:31930,ptovrint:False,ptlb:Up Color,ptin:_UpColor,varname:_UpColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7727,x:32270,y:32006,ptovrint:False,ptlb:Up Alb Power,ptin:_UpAlbPower,varname:_UpAlbPower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Clamp01,id:8532,x:32622,y:31767,varname:node_8532,prsc:2|IN-8142-OUT;n:type:ShaderForge.SFN_Multiply,id:7342,x:32700,y:31954,varname:node_7342,prsc:2|A-7032-A,B-568-OUT;n:type:ShaderForge.SFN_ValueProperty,id:568,x:32700,y:32099,ptovrint:False,ptlb:Up Gloss Smooth,ptin:_UpGlossSmooth,varname:_UpGlossSmooth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1875,x:33037,y:31953,varname:node_1875,prsc:2|A-4786-OUT,B-8815-OUT;n:type:ShaderForge.SFN_Multiply,id:8766,x:33037,y:31693,varname:node_8766,prsc:2|A-7032-R,B-6038-OUT;n:type:ShaderForge.SFN_Clamp01,id:4786,x:32867,y:31966,varname:node_4786,prsc:2|IN-7342-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8815,x:33037,y:32109,ptovrint:False,ptlb:Up Gloss,ptin:_UpGloss,varname:_UpGloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_NormalVector,id:3195,x:30844,y:31729,prsc:2,pt:False;n:type:ShaderForge.SFN_ComponentMask,id:3433,x:31032,y:31880,varname:node_3433,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-1997-OUT;n:type:ShaderForge.SFN_Multiply,id:3322,x:31032,y:32082,varname:node_3322,prsc:2|A-4781-OUT,B-7787-OUT;n:type:ShaderForge.SFN_Clamp01,id:7715,x:31206,y:32082,varname:node_7715,prsc:2|IN-3322-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6807,x:30829,y:32055,ptovrint:False,ptlb:Top Bottom,ptin:_TopBottom,varname:_TopBottom,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:218,x:31168,y:32404,ptovrint:False,ptlb:Top Bottom Smooth,ptin:_TopBottomSmooth,varname:_TopBottomSmooth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:5074,x:31643,y:31931,varname:node_5074,prsc:2|A-4556-OUT,B-7331-OUT;n:type:ShaderForge.SFN_Lerp,id:9988,x:32909,y:32471,varname:node_9988,prsc:2|A-688-OUT,B-8766-OUT,T-5074-OUT;n:type:ShaderForge.SFN_Tex2d,id:7032,x:32867,y:31734,ptovrint:False,ptlb:Up Metalness,ptin:_UpMetalness,varname:_UpMetalness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:6038,x:33037,y:31835,ptovrint:False,ptlb:Up Metallic,ptin:_UpMetallic,varname:_UpMetallic,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:765,x:32909,y:32604,varname:node_765,prsc:2|A-328-OUT,B-1875-OUT,T-5074-OUT;n:type:ShaderForge.SFN_Lerp,id:1191,x:32909,y:32733,varname:node_1191,prsc:2|A-1487-OUT,B-1760-OUT,T-5074-OUT;n:type:ShaderForge.SFN_Lerp,id:4875,x:32909,y:32338,varname:node_4875,prsc:2|A-6343-OUT,B-8532-OUT,T-5074-OUT;n:type:ShaderForge.SFN_Lerp,id:7575,x:32909,y:32854,varname:node_7575,prsc:2|A-8159-OUT,B-2256-OUT,T-5074-OUT;n:type:ShaderForge.SFN_Tex2d,id:9860,x:33665,y:31952,ptovrint:False,ptlb:Up AmbientOc,ptin:_UpAmbientOc,varname:_UpAmbientOc,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9763-UVOUT;n:type:ShaderForge.SFN_Multiply,id:6167,x:33665,y:32156,varname:node_6167,prsc:2|A-9860-R,B-1211-OUT;n:type:ShaderForge.SFN_Clamp01,id:2256,x:33976,y:32156,varname:node_2256,prsc:2|IN-3655-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1211,x:33665,y:32292,ptovrint:False,ptlb:UpAO Mult,ptin:_UpAOMult,varname:_UpAOMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_OneMinus,id:7907,x:33819,y:32292,varname:node_7907,prsc:2|IN-1211-OUT;n:type:ShaderForge.SFN_Add,id:3655,x:33819,y:32156,varname:node_3655,prsc:2|A-6167-OUT,B-7907-OUT;n:type:ShaderForge.SFN_Multiply,id:6189,x:31168,y:32253,varname:node_6189,prsc:2|A-6465-R,B-218-OUT;n:type:ShaderForge.SFN_Tex2d,id:6465,x:30829,y:32345,varname:_node_6465,prsc:2,ntxv:0,isnm:False|MIP-2724-OUT,TEX-1432-TEX;n:type:ShaderForge.SFN_Clamp01,id:3694,x:31319,y:32253,varname:node_3694,prsc:2|IN-6189-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7331,x:31506,y:32051,ptovrint:False,ptlb:Invert Depth,ptin:_InvertDepth,varname:_InvertDepth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3694-OUT,B-6624-OUT;n:type:ShaderForge.SFN_RemapRange,id:6624,x:31506,y:32235,varname:node_6624,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0|IN-3694-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:1887,x:30674,y:31880,varname:node_1887,prsc:2;n:type:ShaderForge.SFN_Transform,id:7068,x:30844,y:31880,varname:node_7068,prsc:2,tffrom:0,tfto:1|IN-1887-XYZ;n:type:ShaderForge.SFN_Add,id:4781,x:31206,y:31903,varname:node_4781,prsc:2|A-3433-OUT,B-6807-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7787,x:30829,y:32173,ptovrint:False,ptlb:Top Bottom Mult,ptin:_TopBottomMult,varname:_TopBottomMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_SwitchProperty,id:1997,x:31032,y:31698,ptovrint:False,ptlb:Use Local Normals,ptin:_UseLocalNormals,varname:_UseLocalNormals,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7068-XYZ,B-3195-OUT;n:type:ShaderForge.SFN_OneMinus,id:4545,x:31367,y:31825,varname:node_4545,prsc:2|IN-7715-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:4556,x:31521,y:31762,ptovrint:False,ptlb:Invert Axis,ptin:_InvertAxis,varname:_InvertAxis,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7715-OUT,B-4545-OUT;n:type:ShaderForge.SFN_Parallax,id:9763,x:30659,y:32700,varname:node_9763,prsc:2|HEI-6465-R,DEP-2405-OUT,REF-4716-OUT;n:type:ShaderForge.SFN_Vector1,id:4716,x:30237,y:32997,varname:node_4716,prsc:2,v1:-1;proporder:6665-7736-5964-287-1432-2724-2405-358-1813-6981-982-1950-2573-4269-1649-590-6982-6010-6807-4556-1997-7787-218-7331-616-3281-7727-320-4876-7032-8815-568-6038-9860-1211;pass:END;sub:END;*/

Shader "DLNK Shaders/Advanced/PBRTopBottom" {
    Properties {
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _MainTex ("Albedo", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _NormalSmooth ("Normal Smooth", Float ) = 0
        _ParallaxMap ("Depth Map", 2D) = "white" {}
        _HgtMapSmooth ("Depth Map Smooth", Range(0, 5)) = 5
        Depth ("Depth", Float ) = 0
        _Metallic ("Metallic", Range(0, 2)) = 1
        _Glossiness ("Gloss", Range(0, 2)) = 0.8
        _UVMasterTiling ("UV Master Tiling", Float ) = 1
        _MetallicGlossMap ("Metalness", 2D) = "white" {}
        _DetailMask ("Detail Mask", 2D) = "white" {}
        _DetailMaskMult ("Detail Mask Mult", Float ) = 0
        _DetailAlbedoMap ("Detail Albedo", 2D) = "white" {}
        _DetailNormalMap ("Detail Bump", 2D) = "bump" {}
        _DetailNormalMapScale ("Detail Bump Smooth", Float ) = 1
        _OcclusionMap ("Occlusion", 2D) = "white" {}
        _OcclusionStrenght ("Occlusion Mult", Float ) = 1
        _TopBottom ("Top Bottom", Float ) = 1
        [MaterialToggle] _InvertAxis ("Invert Axis", Float ) = 1
        [MaterialToggle] _UseLocalNormals ("Use Local Normals", Float ) = 0
        _TopBottomMult ("Top Bottom Mult", Float ) = 1
        _TopBottomSmooth ("Top Bottom Smooth", Float ) = 1
        [MaterialToggle] _InvertDepth ("Invert Depth", Float ) = 0
        _UpColor ("Up Color", Color) = (0.5,0.5,0.5,1)
        _UpAlbedo ("Up Albedo", 2D) = "white" {}
        _UpAlbPower ("Up Alb Power", Float ) = 1
        _UpNormal ("Up Normal", 2D) = "bump" {}
        _UpNormalSmooth ("UpNormal Smooth", Float ) = 0
        _UpMetalness ("Up Metalness", 2D) = "white" {}
        _UpGloss ("Up Gloss", Float ) = 1
        _UpGlossSmooth ("Up Gloss Smooth", Float ) = 1
        _UpMetallic ("Up Metallic", Float ) = 0
        _UpAmbientOc ("Up AmbientOc", 2D) = "white" {}
        _UpAOMult ("UpAO Mult", Float ) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform float _NormalSmooth;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _OcclusionMap; uniform float4 _OcclusionMap_ST;
            uniform float _OcclusionStrenght;
            uniform float Depth;
            uniform float _HgtMapSmooth;
            uniform sampler2D _ParallaxMap; uniform float4 _ParallaxMap_ST;
            uniform sampler2D _DetailAlbedoMap; uniform float4 _DetailAlbedoMap_ST;
            uniform sampler2D _DetailNormalMap; uniform float4 _DetailNormalMap_ST;
            uniform float _DetailNormalMapScale;
            uniform sampler2D _DetailMask; uniform float4 _DetailMask_ST;
            uniform float _DetailMaskMult;
            uniform sampler2D _UpAlbedo; uniform float4 _UpAlbedo_ST;
            uniform sampler2D _UpNormal; uniform float4 _UpNormal_ST;
            uniform float _UpNormalSmooth;
            uniform float4 _UpColor;
            uniform float _UpAlbPower;
            uniform float _UpGlossSmooth;
            uniform float _UpGloss;
            uniform float _TopBottom;
            uniform float _TopBottomSmooth;
            uniform sampler2D _UpMetalness; uniform float4 _UpMetalness_ST;
            uniform float _UpMetallic;
            uniform sampler2D _UpAmbientOc; uniform float4 _UpAmbientOc_ST;
            uniform float _UpAOMult;
            uniform fixed _InvertDepth;
            uniform float _TopBottomMult;
            uniform fixed _UseLocalNormals;
            uniform fixed _InvertAxis;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _node_6465 = tex2Dlod(_ParallaxMap,float4(TRANSFORM_TEX(i.uv0, _ParallaxMap),0.0,_HgtMapSmooth));
                float2 node_9763 = (Depth*(_node_6465.r - (-1.0))*mul(tangentTransform, viewDirection).xy + i.uv0);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_9763.rg, _BumpMap)));
                float3 node_8143 = float3(0,0,1);
                float3 _DetailNormalMap_var = UnpackNormal(tex2D(_DetailNormalMap,TRANSFORM_TEX(node_9763.rg, _DetailNormalMap)));
                float4 _DetailMask_var = tex2D(_DetailMask,TRANSFORM_TEX(node_9763.rg, _DetailMask));
                float node_8121 = saturate((_DetailMask_var.r*_DetailMaskMult));
                float3 _UpNormal_var = UnpackNormal(tex2D(_UpNormal,TRANSFORM_TEX(node_9763.rg, _UpNormal)));
                float node_7715 = saturate(((lerp( mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb, i.normalDir, _UseLocalNormals ).g+_TopBottom)*_TopBottomMult));
                float node_3694 = saturate((_node_6465.r*_TopBottomSmooth));
                float node_5074 = (lerp( node_7715, (1.0 - node_7715), _InvertAxis )*lerp( node_3694, (node_3694*-1.0+1.0), _InvertDepth ));
                float3 normalLocal = lerp(lerp(lerp(_BumpMap_var.rgb,node_8143,_NormalSmooth),lerp(_DetailNormalMap_var.rgb,node_8143,(1.0 - _DetailNormalMapScale)),node_8121),lerp(_UpNormal_var.rgb,float3(0,0,1),_UpNormalSmooth),node_5074);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_9763.rg, _MetallicGlossMap));
                float4 _UpMetalness_var = tex2D(_UpMetalness,TRANSFORM_TEX(node_9763.rg, _UpMetalness));
                float gloss = lerp((_MetallicGlossMap_var.a*_Glossiness),(saturate((_UpMetalness_var.a*_UpGlossSmooth))*_UpGloss),node_5074);
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _OcclusionMap_var = tex2D(_OcclusionMap,TRANSFORM_TEX(node_9763.rg, _OcclusionMap));
                float4 _UpAmbientOc_var = tex2D(_UpAmbientOc,TRANSFORM_TEX(node_9763.rg, _UpAmbientOc));
                float node_7575 = lerp(saturate(((1.0 - _OcclusionStrenght)+(_OcclusionMap_var.r*_OcclusionStrenght))),saturate(((_UpAmbientOc_var.r*_UpAOMult)+(1.0 - _UpAOMult))),node_5074);
                float3 specularAO = node_7575;
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_9763.rg, _MainTex));
                float4 _DetailAlbedoMap_var = tex2D(_DetailAlbedoMap,TRANSFORM_TEX(node_9763.rg, _DetailAlbedoMap));
                float4 _UpAlbedo_var = tex2D(_UpAlbedo,TRANSFORM_TEX(node_9763.rg, _UpAlbedo));
                float3 diffuseColor = lerp((lerp(_MainTex_var.rgb,_DetailAlbedoMap_var.rgb,node_8121)*_Color.rgb),saturate((_UpAlbedo_var.rgb*_UpColor.rgb*_UpAlbPower)),node_5074); // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, lerp((_MetallicGlossMap_var.r*_Metallic),(_UpMetalness_var.r*_UpMetallic),node_5074), specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular) * specularAO;
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                indirectDiffuse *= node_7575; // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform float _NormalSmooth;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform float Depth;
            uniform float _HgtMapSmooth;
            uniform sampler2D _ParallaxMap; uniform float4 _ParallaxMap_ST;
            uniform sampler2D _DetailAlbedoMap; uniform float4 _DetailAlbedoMap_ST;
            uniform sampler2D _DetailNormalMap; uniform float4 _DetailNormalMap_ST;
            uniform float _DetailNormalMapScale;
            uniform sampler2D _DetailMask; uniform float4 _DetailMask_ST;
            uniform float _DetailMaskMult;
            uniform sampler2D _UpAlbedo; uniform float4 _UpAlbedo_ST;
            uniform sampler2D _UpNormal; uniform float4 _UpNormal_ST;
            uniform float _UpNormalSmooth;
            uniform float4 _UpColor;
            uniform float _UpAlbPower;
            uniform float _UpGlossSmooth;
            uniform float _UpGloss;
            uniform float _TopBottom;
            uniform float _TopBottomSmooth;
            uniform sampler2D _UpMetalness; uniform float4 _UpMetalness_ST;
            uniform float _UpMetallic;
            uniform fixed _InvertDepth;
            uniform float _TopBottomMult;
            uniform fixed _UseLocalNormals;
            uniform fixed _InvertAxis;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _node_6465 = tex2Dlod(_ParallaxMap,float4(TRANSFORM_TEX(i.uv0, _ParallaxMap),0.0,_HgtMapSmooth));
                float2 node_9763 = (Depth*(_node_6465.r - (-1.0))*mul(tangentTransform, viewDirection).xy + i.uv0);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_9763.rg, _BumpMap)));
                float3 node_8143 = float3(0,0,1);
                float3 _DetailNormalMap_var = UnpackNormal(tex2D(_DetailNormalMap,TRANSFORM_TEX(node_9763.rg, _DetailNormalMap)));
                float4 _DetailMask_var = tex2D(_DetailMask,TRANSFORM_TEX(node_9763.rg, _DetailMask));
                float node_8121 = saturate((_DetailMask_var.r*_DetailMaskMult));
                float3 _UpNormal_var = UnpackNormal(tex2D(_UpNormal,TRANSFORM_TEX(node_9763.rg, _UpNormal)));
                float node_7715 = saturate(((lerp( mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb, i.normalDir, _UseLocalNormals ).g+_TopBottom)*_TopBottomMult));
                float node_3694 = saturate((_node_6465.r*_TopBottomSmooth));
                float node_5074 = (lerp( node_7715, (1.0 - node_7715), _InvertAxis )*lerp( node_3694, (node_3694*-1.0+1.0), _InvertDepth ));
                float3 normalLocal = lerp(lerp(lerp(_BumpMap_var.rgb,node_8143,_NormalSmooth),lerp(_DetailNormalMap_var.rgb,node_8143,(1.0 - _DetailNormalMapScale)),node_8121),lerp(_UpNormal_var.rgb,float3(0,0,1),_UpNormalSmooth),node_5074);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_9763.rg, _MetallicGlossMap));
                float4 _UpMetalness_var = tex2D(_UpMetalness,TRANSFORM_TEX(node_9763.rg, _UpMetalness));
                float gloss = lerp((_MetallicGlossMap_var.a*_Glossiness),(saturate((_UpMetalness_var.a*_UpGlossSmooth))*_UpGloss),node_5074);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_9763.rg, _MainTex));
                float4 _DetailAlbedoMap_var = tex2D(_DetailAlbedoMap,TRANSFORM_TEX(node_9763.rg, _DetailAlbedoMap));
                float4 _UpAlbedo_var = tex2D(_UpAlbedo,TRANSFORM_TEX(node_9763.rg, _UpAlbedo));
                float3 diffuseColor = lerp((lerp(_MainTex_var.rgb,_DetailAlbedoMap_var.rgb,node_8121)*_Color.rgb),saturate((_UpAlbedo_var.rgb*_UpColor.rgb*_UpAlbPower)),node_5074); // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, lerp((_MetallicGlossMap_var.r*_Metallic),(_UpMetalness_var.r*_UpMetallic),node_5074), specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform float Depth;
            uniform float _HgtMapSmooth;
            uniform sampler2D _ParallaxMap; uniform float4 _ParallaxMap_ST;
            uniform sampler2D _DetailAlbedoMap; uniform float4 _DetailAlbedoMap_ST;
            uniform sampler2D _DetailMask; uniform float4 _DetailMask_ST;
            uniform float _DetailMaskMult;
            uniform sampler2D _UpAlbedo; uniform float4 _UpAlbedo_ST;
            uniform float4 _UpColor;
            uniform float _UpAlbPower;
            uniform float _UpGlossSmooth;
            uniform float _UpGloss;
            uniform float _TopBottom;
            uniform float _TopBottomSmooth;
            uniform sampler2D _UpMetalness; uniform float4 _UpMetalness_ST;
            uniform float _UpMetallic;
            uniform fixed _InvertDepth;
            uniform float _TopBottomMult;
            uniform fixed _UseLocalNormals;
            uniform fixed _InvertAxis;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _node_6465 = tex2Dlod(_ParallaxMap,float4(TRANSFORM_TEX(i.uv0, _ParallaxMap),0.0,_HgtMapSmooth));
                float2 node_9763 = (Depth*(_node_6465.r - (-1.0))*mul(tangentTransform, viewDirection).xy + i.uv0);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_9763.rg, _MainTex));
                float4 _DetailAlbedoMap_var = tex2D(_DetailAlbedoMap,TRANSFORM_TEX(node_9763.rg, _DetailAlbedoMap));
                float4 _DetailMask_var = tex2D(_DetailMask,TRANSFORM_TEX(node_9763.rg, _DetailMask));
                float node_8121 = saturate((_DetailMask_var.r*_DetailMaskMult));
                float4 _UpAlbedo_var = tex2D(_UpAlbedo,TRANSFORM_TEX(node_9763.rg, _UpAlbedo));
                float node_7715 = saturate(((lerp( mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb, i.normalDir, _UseLocalNormals ).g+_TopBottom)*_TopBottomMult));
                float node_3694 = saturate((_node_6465.r*_TopBottomSmooth));
                float node_5074 = (lerp( node_7715, (1.0 - node_7715), _InvertAxis )*lerp( node_3694, (node_3694*-1.0+1.0), _InvertDepth ));
                float3 diffColor = lerp((lerp(_MainTex_var.rgb,_DetailAlbedoMap_var.rgb,node_8121)*_Color.rgb),saturate((_UpAlbedo_var.rgb*_UpColor.rgb*_UpAlbPower)),node_5074);
                float specularMonochrome;
                float3 specColor;
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(node_9763.rg, _MetallicGlossMap));
                float4 _UpMetalness_var = tex2D(_UpMetalness,TRANSFORM_TEX(node_9763.rg, _UpMetalness));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, lerp((_MetallicGlossMap_var.r*_Metallic),(_UpMetalness_var.r*_UpMetallic),node_5074), specColor, specularMonochrome );
                float roughness = 1.0 - lerp((_MetallicGlossMap_var.a*_Glossiness),(saturate((_UpMetalness_var.a*_UpGlossSmooth))*_UpGloss),node_5074);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "DLNK Shaders/Advanced/PBRAdvOpaque"
    CustomEditor "ShaderForgeMaterialInspector"
}
