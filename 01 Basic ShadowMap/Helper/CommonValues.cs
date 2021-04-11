using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._01_Basic_ShadowMap.Helper
{
    public static class CommonValues
    {
        public static Shader depthCaptureShader;
        public static Matrix4x4 lightProjection;
        public static RenderTexture shadowDepthMap;
        private static Texture2D shadowDepthTexture2D;

        public static bool GetShadowState(Vector3 pos)
        {
            Vector3 posInLight = new Vector3();
            float depth = GetDepth(pos, lightProjection, shadowDepthMap, out posInLight);
            return posInLight.z > depth;
        }

        //public static float GetDepthWithPcf(Vector3 worldPos, Matrix4x4 lightTrans, RenderTexture depthMap,
        //    out Vector3 posInLight)
        //{
            
        //}
        public static float GetDepth(Vector3 worldPos, Matrix4x4 lightTrans, RenderTexture depthMap, out Vector3 posInLight)
        {
            posInLight = (lightTrans * (new Vector4(worldPos.x, worldPos.y, worldPos.z, 1)));
            if (shadowDepthTexture2D == null)
                shadowDepthTexture2D = Rt2T(shadowDepthMap);
            Color enc = shadowDepthTexture2D.GetPixelBilinear(posInLight.x, posInLight.y);
            return DecodeFloatRGBA(new Vector4(enc.r,enc.g,enc.b,enc.a));
        }

        //public static Vector4 EncodeFloatRGBA(float v)
        //{
        //    Vector4 kEncodeMul = new Vector4(1.0f, 255.0f, 65025.0f, 160581375.0f);
        //    float kEncodeBit = 1.0f / 255.0f;
        //    Vector4 enc = kEncodeMul * v;
        //    enc = frac(enc);
        //    enc -= enc.yzww * kEncodeBit;
        //    return enc;
        //}

        public static float DecodeFloatRGBA(Vector4 enc)
        {
            Vector4 kDecodeDot = new Vector4(1.0f, 1 / 255.0f, 1 / 65025.0f, 1 / 160581375.0f);
            return dot(enc, kDecodeDot);
        }

        private static float dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        private static Texture2D Rt2T(RenderTexture rt)
        {
            Texture2D tex = new Texture2D(rt.width, rt.height);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            return tex;
        }

    }
}
