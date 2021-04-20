Shader "Custom/ShadowOutput" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		#include "Assets/ShadowMap Shaders/Octree.cginc"
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _Octree;
		
		fixed4 _Color;
		int _TreeDepth;
		float3 _AABBMin;
		int _TexWidth;
		int _TexHeight;

		float GetShadowAtten(float3 worldPos) {
			float3 aabbPos = GetAABBPostion(worldPos, _AABBMin);
			return GetShadow(aabbPos, _Octree, _TreeDepth,_TexWidth,_TexHeight);
		}


		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color * GetShadowAtten(IN.worldPos);
			o.Alpha = 1;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
