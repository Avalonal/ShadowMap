inline float3 GetAABBPostion(float3 pos, float3 basePos) {
	return pos - basePos;
}

inline uint DecodeNode(uint val, out uint type)
{
    type = val >> 30;
    val -= type * 1073741824;
    return val;
}

inline int3 GetNearByPosInAABB(float3 pos, float cell)
{
    int3 ans;
    ans.x = floor(pos.x / cell);
    ans.y = floor(pos.y / cell);
    ans.z = floor(pos.z / cell);
    return ans;
}

