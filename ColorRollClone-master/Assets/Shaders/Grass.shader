Shader "Unlit/Grass"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BottomColor("_BottomColor", Color) = (1,1,1,1)
		_TopColor("_TopColor", Color) = (1,1,1,1)
		_Factor("_Factor", Float) = 1.0
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Autolight.cginc"

	float _Factor;
	float4 _BottomColor;
	float4 _TopColor;

	struct VertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float2 uv : TEXCOORD0;
	};

	struct GeometryInput{

		float4 pos: SV_POSITION;
		float3 normal: NORMAL;
		float4 tangent : TANGENT;
		float2 uv: TEXCOORD0;
	};

	struct GeometryOutput {
		float4 pos: SV_POSITION;
		float2 uv: TEXCOORD0;
	};

	// Construct a rotation matrix that rotates around the provided axis, sourced from:
	// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
	float3x3 AngleAxis3x3(float angle, float3 axis)
	{
		float c, s;
		sincos(angle, s, c);

		float t = 1 - c;
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;

		return float3x3(
			t * x * x + c, t * x * y - s * z, t * x * z + s * y,
			t * x * y + s * z, t * y * y + c, t * y * z - s * x,
			t * x * z - s * y, t * y * z + s * x, t * z * z + c
			);
	}


	GeometryOutput geometryOutput;
	GeometryOutput GeometryVertex(float3 pos, float2 uv) {
		geometryOutput.pos = UnityObjectToClipPos(pos);
		geometryOutput.uv = uv;
		return geometryOutput;
	}

	GeometryInput vert(VertexInput i)
	{
		GeometryInput o;
	
		o.pos = i.vertex;
		o.normal = i.normal;
		o.tangent = i.tangent;
		o.uv = i.uv;

		return o;
	}

	[maxvertexcount(12)]
	void geo(triangle GeometryInput input[3], inout TriangleStream<GeometryOutput> triStream) {//float4 input[3]: SV_POSITION
		
		float3 faceNormal = normalize(cross(input[1].pos - input[0].pos, input[2].pos - input[0].pos));

		//determine whic lateral side is the longest
		float edge0 = distance(input[1].pos, input[2].pos);
		float edge1 = distance(input[2].pos, input[0].pos);
		float edge2 = distance(input[1].pos, input[0].pos);

		float4 centralPos = (input[1].pos + input[2].pos) / 2;
		float2 centralUv = (input[1].uv + input[2].uv) / 2;
		
		if (step(edge1, edge2) * step(edge0, edge2) == 1) {
			centralPos = (input[1].pos + input[0].pos) / 2;
			centralUv = (input[1].uv + input[0].uv) / 2;
		}
		else if (step(edge0, edge1) * step(edge2, edge1) == 1) {
			centralPos = (input[2].pos + input[0].pos) / 2;
			centralUv = (input[2].uv + input[0].uv) / 2;
		}

		centralPos += float4(input[0].normal, 0) * _Factor;

		GeometryOutput o;

		for (int i = 0; i < 3; i++) {
			int nexti = (i + 1) % 3;



			triStream.Append(GeometryVertex(input[i].pos, float2(0, 0)));
			triStream.Append(GeometryVertex(centralPos, float2(0.5, 1)));
			triStream.Append(GeometryVertex(input[nexti].pos, float2(1, 0)));

			triStream.RestartStrip();
		}

	}

	ENDCG


		SubShader
	{
		Cull Off
		Pass
		{
			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo

			#include "Lighting.cginc"

			float4 frag(GeometryOutput i, fixed facing : VFACE) : SV_Target//float4 vertex : SV_POSITION
			{
				return lerp(_BottomColor, _TopColor, i.uv.y);// float4(1, 1, 1, 1);
			}
			ENDCG
		}
	}
}
