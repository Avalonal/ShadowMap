using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets._01_Basic_ShadowMap.Helper
{
    public static class CommonValues
    {
        public static Shader depthCaptureShader;
        public static Matrix4x4 lightProjection;
        public static RenderTexture shadowDepthMap;
        public static Texture2D shadowDepthTexture2D;

        public static float pixelWidth;
        public static float pixelHeight;
        public static float bias;

        public static float GetShadowState(Vector3 pos)
        {
            Vector3 posInLight = new Vector3();
            float depth = GetDepth(pos, lightProjection, shadowDepthMap, out posInLight);
            float strength = GetShadowAttenuate(posInLight, shadowDepthMap, bias, pixelWidth, pixelHeight);
            //Debug.LogFormat("{0}=>{1}", posInLight.z,depth);
            //return posInLight.z <= depth ? 0.0f : 1.0f;
            return strength;
        }

        public static void Init()
        {
            if (shadowDepthTexture2D == null)
                shadowDepthTexture2D = toTexture2D(shadowDepthMap);
            Shader.SetGlobalTexture("_ShadowDepthMap", shadowDepthTexture2D);
        }

        private static float GetNearDepth(Vector3 pos, float bias, RenderTexture depthMap, float offsetX, float offsetY, float fator)
        {
            if (shadowDepthTexture2D == null)
                shadowDepthTexture2D = toTexture2D(depthMap);
            Color enc = shadowDepthTexture2D.GetPixelBilinear(pos.x + offsetX, pos.y + offsetY);
            return (pos.z - bias > DecodeFloatRGBA(enc)) ? fator : 0;
        }

        private static float GetShadowAttenuate(Vector3 pos, RenderTexture depthMap, float bias, float pixelWidth, float pixelHeight)
        {
            float atten = 0;
            for (int i = -2; i <= 2; i++)
            for (int j = -2; j <= 2; j++)
                atten += GetNearDepth(pos, bias, depthMap, i * pixelWidth, j * pixelHeight, 1);
            atten = atten / 25;
            return atten;
        }

        private static float GetDepth(Vector3 worldPos, Matrix4x4 lightTrans, RenderTexture depthMap, out Vector3 posInLight)
        {
            posInLight = (lightTrans * (new Vector4(worldPos.x, worldPos.y, worldPos.z, 1)));
            //Debug.LogFormat("{0} <=> {1}",worldPos,posInLight);
            if (shadowDepthTexture2D == null)
                shadowDepthTexture2D = toTexture2D(depthMap);
            Color enc = shadowDepthTexture2D.GetPixelBilinear(posInLight.x, posInLight.y);
            //Debug.Log(enc);
            return DecodeFloatRGBA(enc);
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

        private static float DecodeFloatRGBA(Vector4 enc)
        {
            Vector4 kDecodeDot = new Vector4(1.0f, 1.0f / 255.0f, 1.0f / 65025.0f, 1.0f / 16581375.0f);
            return dot(enc, kDecodeDot);
        }

        private static float dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        private static Texture2D toTexture2D(RenderTexture renderTexture)
        {
            Texture2D texDepth = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, true);
            RenderTexture.active = renderTexture;
            texDepth.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texDepth.Apply();
            RenderTexture.active = null;
            var png = texDepth.EncodeToPNG();
            string path = AssetDatabase.GetAssetPath(renderTexture) + ".png";
            System.IO.File.WriteAllBytes(path, png);
            AssetDatabase.ImportAsset(path);
            Debug.Log("Saved to " + path);
            //Texture2D texDepth2 = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, true);
            //texDepth2.LoadImage(png);
            return texDepth;
        }
    }
}
