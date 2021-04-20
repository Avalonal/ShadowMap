inline float3 GetAABBPostion(float3 pos, float basePos) {
	return pos - basePos;
}

inline float4 GetPixel(int id, sampler2D tex, int width, int height) {
	float u = (id / width) / (float)width;
	float v = (id % width) / (float)width;
	return tex2D(tex, float2(u, v));
}

inline float GetShadow(float3 pos,sampler2D octree,int depth,int width,int height) {
	int tmpIp = 0;
	for (int i = depth-1; i >= 0; --i) {
		int id = (((pos.x >> i) & 1) << 2) + (((pos.y >> i) & 1) << 1) + (((pos.z >> i) & 1));
		if()
	}
}

