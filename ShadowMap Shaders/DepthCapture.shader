// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DepthCapture" {

    SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass {
            CGPROGRAM

            #pragma vertex   vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct v2f {
                float4 position : SV_POSITION;
                float2 depth    : TEXCOORD0;
            };
            
            v2f vert(appdata_base v) {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.depth    = o.position.zw;
                return o;
            }
            
            float4 frag(v2f i) : COLOR {
                return EncodeFloatRGBA(i.depth.x / i.depth.y);
            }

            ENDCG
        }

    }
}
