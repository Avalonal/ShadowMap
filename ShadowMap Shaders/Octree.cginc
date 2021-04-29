inline float3 GetAABBPostion(float3 pos, float3 basePos) {
	return pos - basePos;
}

inline float4 GetPixel(int id, sampler2D tex, uint width, uint height) {
	float2 uv;
	uv.x = (id / width) / (float)width;
	uv.y = (id % width) / (float)height;
	return tex2D(tex, uv);
}

inline int DecodeIntRGBA(float4 col, out int type) {
	int x = (int)(col.r * 255.0f + 0.5f);
	int y = (int)(col.g * 255.0f + 0.5f);
	int z = (int)(col.b * 255.0f + 0.5f);
	int w = (int)(col.a * 255.0f + 0.5f);
	type = x >> 6;
	x -= type * 64;
    //int val = x * 16777216 + y * 65536 + z * 256 + w;
	return 0;
}

inline void GetNearByPosInAABB(float3 pos,float cell, out uint x, out uint y, out uint z) {
	x = floor(pos.x / cell );
	y = floor(pos.y / cell );
	z = floor(pos.z / cell );
}

inline float GetShadow(float3 pos,sampler2D octree,int depth,int width,int height,float cell) {
	
	int tmpIp = 0;
	uint x = 0, y = 0, z = 0;
	GetNearByPosInAABB(pos, cell, x, y, z);
	
	//[unroll(10)]
	for (int i = depth-1; i >= 0; --i) {
		int id = (((x >> i) & 1) << 2) + (((y >> i) & 1) << 1) + ((z >> i) & 1);

		int type = 0;
		float4 col = GetPixel(tmpIp, octree, width, height);
		return (float)id/7.0f;
		/*int val = DecodeIntRGBA(col,type);
		return val;*/

		//if (type == 0) {
		//	//return 0;
		//	return val == 0 ? 0f : 1f;
		//}
		//else if (type == 1) {
		//	return ((val >> id) & 1) == 0 ? 0f : 1f;
		//}
		//else if (type == 2) {
		//	tmpIp = val+id;
		//	//return 0;
		//	continue;
		//}
		//else return 0;
	}
	/*int type = 0;
	float4 col = GetPixel(tmpIp, octree, width, height);
	return DecodeIntRGBA(col , type) == 0 ? 0f : 1f;*/

	return float4(1,1,0,1);
}

