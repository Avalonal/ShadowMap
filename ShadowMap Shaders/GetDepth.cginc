// Get Depth Info From Shadow Depth Map


inline float GetDepth(float3 worldPos, float4x4 lightTrans, sampler2D depthMap, out float3 posInLight) {
    posInLight = mul(lightTrans, float4(worldPos, 1)).xyz;
    return DecodeFloatRGBA(tex2D(depthMap, posInLight.xy));
}