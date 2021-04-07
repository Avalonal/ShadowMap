Shader "Custom/ShadowMapWithPCF" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)

		_slopeScaleDepthBias("Bias Sloop Scale", Range(0, 1)) = 0
		_depthBias("depth bias", float) = 0.001

		_pixelWidth("pixel width", float) = 0.001
		_pixelHeight("pixel height", float) = 0.001
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#include "Assets/ShadowMap Shaders/GetDepth.cginc"
		#include "Assets/ShadowMap Shaders/GetShadowBias.cginc"
		#include "Assets/ShadowMap Shaders/ShadowPCF.cginc"

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

		// For PCF
		float _pixelWidth;
		float _pixelHeight;

		float GetShadowAtten(float3 worldPos, float3 worldNormal) {
			float3 posInLight;
			float shadowDepth = GetDepth(worldPos, _LightProjection, _ShadowDepthMap, posInLight);
			float bias = GetShadowBias(worldNormal, _LightProjection, _slopeScaleDepthBias, _depthBias / 100000);

			float strength = GetShadowAttenuate(posInLight, _ShadowDepthMap, bias, _pixelWidth, _pixelHeight);
			//if (shadowDepth < posInLight.z) {
			//	_Color = fixed4(1, 0, 0, 1);
			//}
			//else if (shadowDepth == posInLight.z) {
			//	_Color = fixed4(0, 1, 0, 1);
			//}
			//else {
			//	_Color = fixed4(0, 0, 1, 1);
			//}
			/*if (strength <= 0) {
				_Color = fixed4(1, 0, 0, 1);
			}
			else if (strength < 0.5) {
				_Color = fixed4(0, 1, 0, 1);
			}
			else {
				_Color = fixed4(0, 0, 1, 1);
			}*/
			return strength;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color * GetShadowAtten(IN.worldPos, IN.worldNormal);
			//o.Albedo = _Color;
			o.Alpha = 1;
		}
		ENDCG
	}
}