// Get Shadow Bias

inline float GetShadowBias(float3 worldNormal, float4x4 lightTrans, float slopeFactor, float depthBias) {
    float3 normalInLight = mul((float3x3)lightTrans, worldNormal);
    return (1 - normalInLight.z) * slopeFactor + depthBias;
}