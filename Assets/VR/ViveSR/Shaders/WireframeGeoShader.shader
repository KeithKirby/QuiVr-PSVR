Shader "ViveSR/Wireframe"
{
	Properties
    {
        _Thickness ("Wire Thickness", RANGE(0, 800)) = 100
        _Color ("Wire Color", Color) = (0.0, 1.0, 0.0, 1.0)
    }

    SubShader
    {
        //Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		//Blend SrcAlpha OneMinusSrcAlpha
		//
		//Pass
        //{
		//	ZWrite On
		//	ColorMask 0
		//}

		Tags { "Queue" = "Geometry" "RenderType"="Opaque" }

		GrabPass{ "_SeeThroughBGTex" }

        Pass
        {
			// http://developer.download.nvidia.com/SDK/10/direct3d/Source/SolidWireframe/Doc/SolidWireframe.pdf
            CGPROGRAM
			#pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float _Thickness;
            uniform float4 _Color; 
			sampler2D _SeeThroughBGTex;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2g
            {
                float4 cPos : SV_POSITION;
            };

            struct g2f
            {
                float4 cPos : SV_POSITION;
				float2 screenPos : TEXCOORD0;
                float4 dist : TEXCOORD1;
            };
            
            v2g vert (appdata v)
            {
                v2g o;
                o.cPos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triangleStream)
            {
                float2 p0 = i[0].cPos.xy / i[0].cPos.w;
                float2 p1 = i[1].cPos.xy / i[1].cPos.w;
                float2 p2 = i[2].cPos.xy / i[2].cPos.w;

                float2 edge0 = p2 - p1;
                float2 edge1 = p2 - p0;
                float2 edge2 = p1 - p0;

                // To find the distance to the opposite edge, we take the
                // formula for finding the area of a triangle Area = Base/2 * Height, 
                // and solve for the Height = (Area * 2)/Base.
                // We can get the area of a triangle by taking its cross product
                // divided by 2.  However we can avoid dividing our area/base by 2
                // since our cross product will already be double our area.
                float area = abs(edge1.x * edge2.y - edge1.y * edge2.x);
                float wireThickness = 800 - _Thickness;

                g2f o;

                o.cPos = i[0].cPos;
				o.screenPos = o.cPos.xy / o.cPos.w;
				o.screenPos.x = 0.5 + o.screenPos.x * 0.5;
				o.screenPos.y = 0.5 - o.screenPos.y * 0.5;
                o.dist.xyz = float3( (area / length(edge0)), 0.0, 0.0) * o.cPos.w * wireThickness;
                o.dist.w = 1.0 / o.cPos.w;
                triangleStream.Append(o);

                o.cPos = i[1].cPos;
				o.screenPos = o.cPos.xy / o.cPos.w;
				o.screenPos.x = 0.5 + o.screenPos.x * 0.5;
				o.screenPos.y = 0.5 - o.screenPos.y * 0.5;
                o.dist.xyz = float3(0.0, (area / length(edge1)), 0.0) * o.cPos.w * wireThickness;
                o.dist.w = 1.0 / o.cPos.w;
                triangleStream.Append(o);

                o.cPos = i[2].cPos;
				o.screenPos = o.cPos.xy / o.cPos.w;
				o.screenPos.x = 0.5 + o.screenPos.x * 0.5;
				o.screenPos.y = 0.5 - o.screenPos.y * 0.5;
                o.dist.xyz = float3(0.0, 0.0, (area / length(edge2))) * o.cPos.w * wireThickness;
                o.dist.w = 1.0 / o.cPos.w;
                triangleStream.Append(o);
            }

            float4 frag (g2f i) : SV_Target
            {
                float minDistanceToEdge = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.dist[3];
				float4 bgColor = tex2D(_SeeThroughBGTex, i.screenPos);

				// not on a line
				if (minDistanceToEdge > 0.9)
					return bgColor;

				// Smooth our line out
                float t = exp2(-2 * minDistanceToEdge * minDistanceToEdge);
                fixed4 finalColor = lerp(bgColor, _Color, t);
                return finalColor;
            }
			ENDCG
        }
    }
}
