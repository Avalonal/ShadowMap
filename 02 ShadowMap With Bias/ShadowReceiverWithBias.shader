Shader "Custom/ShadowReceiverWithBias" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)

        _slopeScaleDepthBias("Bias Sloop Scale", Range(0, 1)) = 0
        _depthBias("depth bias", float) = 0.001
    }

    SubShader{
        Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM

        #include "Assets/ShadowMap Shaders/GetDepth.cginc"
        #include "Assets/ShadowMap Shaders/GetShadowBias.cginc"

        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        struct Input {
            float3 worldPos;
            float3 worldNormal;
        };

        fixed4 _Color;

        // For Shadow
        sampler2D _ShadowDepthMap;
        float4x4  _LightProjection;

        // For ShadowBias
        float _slopeScaleDepthBias;
        float _depthBias;

        float GetShadowAtten(float3 worldPos, float3 worldNormal) {
            float3 posInLight;
            float shadowDepth = GetDepth(worldPos, _LightProjection, _ShadowDepthMap, posInLight);
            float bias        = GetShadowBias(worldNormal, _LightProjection, _slopeScaleDepthBias, _depthBias / 100000);
            return posInLight.z - bias <= shadowDepth ? 1.0f : 0.1f;
        }


        void surf(Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = _Color * GetShadowAtten(IN.worldPos, IN.worldNormal);
            o.Alpha  = 1;
        }
        ENDCG
    }
}
