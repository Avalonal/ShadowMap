Shader "Custom/ShadowOutput" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#include "Assets/ShadowMap Shaders/Octree.cginc"

		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};

		sampler2D _Octree;

		uniform float4 _MainTex_TexelSize;
		
		fixed4 _Color;
		int _TreeDepth;
		float4 _AABBMin;
		int _TexWidth;
		int _TexHeight;
		float _AABBCell;

		float GetShadowAtten(float3 worldPos) {
			float3 aabbPos = GetAABBPostion(worldPos, _AABBMin.xyz);
			return GetShadow(aabbPos, _Octree, _TreeDepth, _TexWidth, _TexHeight, _AABBCell);
		}


		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color * GetShadowAtten(IN.worldPos);
			o.Alpha = 1;
		}

		ENDCG
	}
}
