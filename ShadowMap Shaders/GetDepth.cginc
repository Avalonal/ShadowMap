// Get Depth Info From Shadow Depth Map


inline float SelfDot(float4 a,float4 b){
    return a.x*b.x + a.y*b.y+a.z*b.z+a.w*b.w;
}

inline float SelfDecode( float4 enc )
{
    float4 kDecodeDot = float4(1.0, 1/255.0, 1/65025.0, 1/16581375.0);
    return SelfDot( enc, kDecodeDot );
}

inline float GetDepth(float3 worldPos, float4x4 lightTrans, sampler2D depthMap, out float3 posInLight) {
    posInLight = mul(lightTrans, float4(worldPos, 1)).xyz;
    return DecodeFloatRGBA(tex2D(depthMap, posInLight.xy));
}