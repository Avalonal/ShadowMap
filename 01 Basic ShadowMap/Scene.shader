Shader "Custom/Scene" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM

        #include "Assets/ShadowMap Shaders/GetDepth.cginc"

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input {
            float3 worldPos;
        };

        fixed4 _Color;

        // For Shadow
        sampler2D _ShadowDepthMap;
        float4x4  _LightProjection;


        float GetShadowAtten(float3 worldPos) {
            float3 posInLight;
            float shadowDepth = GetDepth(worldPos, _LightProjection, _ShadowDepthMap, posInLight);
            return posInLight.z <= shadowDepth ? 0.f : 1.f;
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = _Color * GetShadowAtten(IN.worldPos);
            o.Alpha  = 1;
        }
        ENDCG
    }
}
