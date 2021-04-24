inline float3 GetAABBPostion(float3 pos, float3 basePos) {
	return pos - basePos;
}

inline float4 GetPixel(int id, sampler2D tex, int width, int height) {
	float u = (id / width) / (float)width;
	float v = (id % width) / (float)width;
	return tex2D(tex, float2(u, v));
}

inline int DecodeIntRGBA(float4 col, out int type) {
	int x = (int)(col.x * 255 + 0.5);
	int y = (int)(col.y * 255 + 0.5);
	int z = (int)(col.z * 255 + 0.5);
	int w = (int)(col.w * 255 + 0.5);
	type = (x >> 6);
	x -= type * (1 << 7);
	int val = x * (1 << 24) + y * (1 << 16) + z * (1 << 8) + w;
	return val;
}

inline void GetNearByPosInAABB(float3 pos,float cell, out int x, out int y, out int z) {
	x = (int)(pos.x / cell + 0.5);
	y = (int)(pos.y / cell + 0.5);
	z = (int)(pos.z / cell + 0.5);
}

inline float GetShadow(float3 pos,sampler2D octree,int depth,int width,int height,float cell) {
	
	int tmpIp = 0;
	bool goOn = false;
	int x = 0, y = 0, z = 0;
	GetNearByPosInAABB(pos, cell, x, y, z);
	
	[unroll(10)]
	for (int i = depth-1; i >= 0; --i) {
		int id = (((x >> i) & 1) << 2) + (((y >> i) & 1) << 1) + ((z >> i) & 1);
		if (goOn) {
			return ((tmpIp >> id) & 1);
		}
		int type = 0;
		int val = DecodeIntRGBA(GetPixel(tmpIp, octree, width, height), type);
		if (type == 0) {
			//return 0;
			return val;
		}
		else if (type == 1) {
			goOn = true;
			tmpIp = val;
			//return 1;
			continue;
		}
		else if (type == 2) {
			tmpIp = val+id;
			//return 0;
			continue;
		}
	}
	int type = 0;
	return DecodeIntRGBA(GetPixel(tmpIp, octree, width, height), type);
}

