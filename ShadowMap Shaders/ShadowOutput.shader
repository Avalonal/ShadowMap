Shader "Custom/ShadowOutput" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#include "Assets/ShadowMap Shaders/Octree.cginc"

		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		struct Node
		{
			int4 val;
		};
		#ifdef SHADER_API_D3D11
			StructuredBuffer<Node> _OctreeBuffer;
		#endif

		struct Input {
			float3 worldPos;
		};

		//sampler2D _Octree;
		
		fixed4 _Color;
		int _TreeDepth;
		float4 _AABBMin;
		//int _TexWidth;
		//int _TexHeight;
		float _AABBCell;

		float GetShadow(float3 pos, int depth, float cell) {

			int tmpIp = 0;
			uint x = 0, y = 0, z = 0;
			GetNearByPosInAABB(pos, cell, x, y, z);

			//[unroll(10)]
			for (int i = depth - 1; i >= 0; --i) {
				int id = (((x >> i) & 1) << 2) + (((y >> i) & 1) << 1) + ((z >> i) & 1);

				int type = 0;
				int4 value = _OctreeBuffer[tmpIp].value;
				int val = DecodeNode(value,type);

				if (type == 0) {
					//return 0;
					return val == 0 ? 0f : 1f;
				}
				else if (type == 1) {
					return ((val >> id) & 1) == 0 ? 0f : 1f;
				}
				else if (type == 2) {
					tmpIp = val+id;
					//return 0;
					continue;
				}
				else return 0;
			}
			int type = 0;
			int4 value = _OctreeBuffer[tmpIp].value;
			int val = DecodeNode(value, type);
			return val == 0 ? 0f : 1f;
		}

		float GetShadowAtten(float3 worldPos) {
			float3 aabbPos = GetAABBPostion(worldPos, _AABBMin.xyz);
			return GetShadow(aabbPos, _TreeDepth, _AABBCell);
		}


		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color * GetShadowAtten(IN.worldPos);
			o.Alpha = 1;
		}

		ENDCG
	}
}
