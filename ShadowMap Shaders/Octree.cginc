inline float3 GetAABBPostion(float3 pos, float3 basePos) {
	return pos - basePos;
}

inline float4 GetPixel(int id, sampler2D tex, uint width, uint height) {
	float u = (id / width) / (float)width;
	float v = (id % width) / (float)width;
	return tex2D(tex, float2(u, v));
}

inline int DecodeIntRGBA(float4 col, out int type) {
	int x = (int)(col.x * 255 );
	int y = (int)(col.y * 255 );
	int z = (int)(col.z * 255 );
	int w = (int)(col.w * 255 );
	type = x / 64;
	x -= type * 64;
    int val = x * 16777216 + y * 65536 + z * 256 + w;
	return val;
}

inline void GetNearByPosInAABB(float3 pos,float cell, out int x, out int y, out int z) {
	x = (int)(pos.x / cell );
	y = (int)(pos.y / cell );
	z = (int)(pos.z / cell );
}

inline float GetShadow(float3 pos,sampler2D octree,int depth,int width,int height,float cell) {
	
	int tmpIp = 0;
	int x = 0, y = 0, z = 0;
	GetNearByPosInAABB(pos, cell,x,y,z);

	//[unroll(10)]
	for (int i = depth-1; i >= 0; i--) {
		int id = (((x >> i) & 1) << 2) + (((y >> i) & 1) << 1) + ((z >> i) & 1);

		int type = 0;
		float4 col = GetPixel(tmpIp, octree, width, height);
		int val = DecodeIntRGBA(col,type);

		if (type == 0) {
			//return 0;
			return val;
		}
		else if (type == 1) {
			return ((val >> id) & 1);
		}
		else if (type == 2) {
			tmpIp = val+id;
			//return 0;
			continue;
		}
	}
	int type = 0;
	float4 col = GetPixel(tmpIp, octree, width, height);
	return DecodeIntRGBA(col , type);
}

