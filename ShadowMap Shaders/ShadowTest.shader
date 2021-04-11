Shader "Custom/ShadowTest"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #include "Assets/ShadowMap Shaders/GetDepth.cginc"

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input {
            float3 worldPos;
        };

        // For Shadow
        sampler2D _ShadowDepthMap;
        float4x4  _LightProjection;
        float4 _MousePos;


        float GetShadowAtten(float3 worldPos) {
            float3 posInLight;
            float shadowDepth = GetDepth(worldPos, _LightProjection, _ShadowDepthMap, posInLight);
            return posInLight.z <= shadowDepth ? 0.f : 1.f;
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = GetShadowAtten(_MousePos.rgb);
            o.Alpha  = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
