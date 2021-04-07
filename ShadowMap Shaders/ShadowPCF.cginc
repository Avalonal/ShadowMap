
// Upgrade NOTE: excluded shader from DX11, Xbox360, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 xbox360 gles
inline float GetNearDepth(float3 pos, float bias, sampler2D depthMap, float offsetX, float offsetY, float fator) {
    return (pos.z - bias > DecodeFloatRGBA(tex2D(depthMap, float2(pos.x + offsetX, pos.y + offsetY)))) ? fator : 0;
}

inline float GetShadowAttenuate(float3 pos, sampler2D depthMap, float bias, float pixelWidth, float pixelHeight) {
    float atten = 0;
    int i = 0;
    int j = 0;

    //float[] factors = {0.1, 0.2, 0.4, 0.2, 0.1}; // 1.0 combined
    for (i = -2; i <= 2; i ++)
        for (j = -2; j <= 2; j ++)
            atten += GetNearDepth(pos, bias, depthMap, i * pixelWidth, j * pixelHeight, 1);// factors[i + 2] * factors[j + 2]);
    atten = atten / 25;
    return atten;
}
