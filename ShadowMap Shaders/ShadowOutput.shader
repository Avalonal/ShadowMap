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

		#pragma target 3.5

		struct Node
		{
			uint value;
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


		inline float GetShadow(int3 nearpos, int depth)
		{

			uint tmpIp = 0;

			uint length = (1 << depth);
			if (nearpos.x < 0 || nearpos.x >= length ||
						nearpos.y < 0 || nearpos.y >= length ||
						nearpos.z < 0 || nearpos.z >= length)
				return 0;
			
					//return	float3( (float)nearpos.x/(float)(length-1),
					//				(float)nearpos.y/(float)(length-1),
					//				(float)nearpos.z/(float)(length-1));
					//找所在网格正确


					//[unroll(10)]
			for (uint i = depth - 1; i >= 0; --i)
			{
				uint id = (((nearpos.x >> i) & 1) << 2) + (((nearpos.y >> i) & 1) << 1) + ((nearpos.z >> i) & 1);
				uint type = 3;
		#ifdef SHADER_API_D3D11
				uint value = _OctreeBuffer[tmpIp].value;
				uint val = DecodeNode(value,type);
		#else
				uint val = -1;
		#endif

						//switch(id){
						//		case 0:
						//		return float3(0,0,0);
						//		case 1:
						//		return float3(0,0,1);
						//		case 2:
						//		return float3(0,1,0);
						//		case 3:
						//		return float3(0,1,1);
						//		case 4:
						//		return float3(1,0,0);
						//		case 5:
						//		return float3(1,0,1);
						//		case 6:
						//		return float3(1,1,0);
						//		case 7:
						//		return float3(1,1,1);
						//}
						//return float3(0,0.5f,0);
						//找id索引正确
				

						//if(type==0) return float3(0,1,0);
						//else if(type==2) return float3(0,0,1);
						//else if(type==1){
						//	if(val == 3)
						//		return float3(1,1,0);
						//	else
						//		return float3(1,0,1);
						//}
						//else return float3(1,0,0);
						//buffer内数据正常


				if (type == 0)
				{
					if (val == 0) 
						return 1;
					else
						return 0;
				}
				else if (type == 1)
				{
					uint inShadow = ((val >> id) & 1);
					if (inShadow == 0)
						return 1;
					else
						return 0;
				}
				else if (type == 2)
				{
					tmpIp = val + id;
				}
				else
					return 0.5;
			}

			return 0;
		}


		inline float GetShadowWithPCF(int3 pos,int depth,int size){
			float tot = GetShadow(pos, depth) * 1;
			int num = 1;

			for (int i = -size; i <= size; ++i)
			for (int j = -size; j <= size; ++j)
			for (int k = -size; k <= size; ++k){
				if (i == 0 && j == 0 && k == 0) continue;
				++num;
				int3 newpos = int3(pos.x+i,pos.y+j,pos.z+k);
				tot += GetShadow(newpos,depth);
			}
			tot /= (float)num;
			return tot;
		}


		void surf(Input IN, inout SurfaceOutputStandard o) {
			float3 aabbPos = GetAABBPostion(IN.worldPos, _AABBMin.xyz);
			int3 nearpos = GetNearByPosInAABB(aabbPos, _AABBCell);
			o.Albedo = _Color * GetShadowWithPCF(nearpos, _TreeDepth,1);
			o.Alpha = 1;
		}

		ENDCG
	}
}
