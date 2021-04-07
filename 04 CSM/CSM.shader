Shader "Custom/CSM" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)

        _slopeScaleDepthBias("Bias Sloop Scale", Range(0, 1)) = 0
        _depthBias("depth bias", float) = 0.001

        _pixelWidth("pixel width", float) = 0.001
        _pixelHeight("pixel height", float) = 0.001
    }

    SubShader {
        Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        #include "Assets/ShadowMap Shaders/GetDepth.cginc"
        #include "Assets/ShadowMap Shaders/GetShadowBias.cginc"
        #include "Assets/ShadowMap Shaders/ShadowPCF.cginc"

        struct Input {
            float3 worldPos;
            float3 worldNormal;
        };

        fixed4 _Color;


        // For ShadowBias
        float _slopeScaleDepthBias;
        float _depthBias;

        // For PCF
        float _pixelWidth;
        float _pixelHeight;

        // For CSM
        sampler2D _shadowDepthMap0;
        sampler2D _shadowDepthMap1;

        float _shadowDepth0;
        float _shadowDepth1;

        float4x4 _LightProjection0;
        float4x4 _LightProjection1;



        int GetIndex(float3 worldPos) {
            float4 pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
            float depth = pos.z / pos.w;
            if (depth >= _shadowDepth1)
                return 1;
            else
                return 0;
        }


        float GetShadowAtten(float3 worldPos, float3 worldNormal) {
            int index = GetIndex(worldPos);
            if (index == 1) {
                float3 posInLight;
                float shadowDepth = GetDepth(worldPos, _LightProjection0, _shadowDepthMap0, posInLight);
                float bias = GetShadowBias(worldNormal, _LightProjection0, _slopeScaleDepthBias, _depthBias / 100000);

                float strength = GetShadowAttenuate(posInLight, _shadowDepthMap0, bias, _pixelWidth, _pixelHeight);
                return 1 - strength;
            }
            else {
                float3 posInLight;
                float shadowDepth = GetDepth(worldPos, _LightProjection1, _shadowDepthMap1, posInLight);
                float bias = GetShadowBias(worldNormal, _LightProjection1, _slopeScaleDepthBias, _depthBias / 100000);

                float strength = GetShadowAttenuate(posInLight, _shadowDepthMap1, bias, _pixelWidth, _pixelHeight);
                return 1 - strength;
            }
        }


        void surf(Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = _Color * GetShadowAtten(IN.worldPos, IN.worldNormal);
            o.Alpha = 1;
        }
        ENDCG
    }
}
