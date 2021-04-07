using Ronin.Utils;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCSM : MonoBehaviour
{

    private Camera mLightCamera;

    private GameObject mLightObj;

    private List<Vector3> mSceneBoundVertexs;
    private Shader mCaptureShader;
    private float[] mRelativeSplitArray;

    public RenderTexture[] mDepthMaps;

    public void Init(List<Vector3> sceneBoundVertexs, Shader captureShader, float [] relativeSplitArray, GameObject lightObj)
    {
        mSceneBoundVertexs = sceneBoundVertexs;
        mCaptureShader = captureShader;
        mRelativeSplitArray = relativeSplitArray;
        mLightObj = lightObj;
    }


    void Start()
    {
        mLightCamera = GetComponent<Camera>();
        mDepthMaps = new RenderTexture[mRelativeSplitArray.Length + 1];
        for (int i = 0; i < mDepthMaps.Length; i ++)
        {
            mDepthMaps[i] = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            mDepthMaps[i].useMipMap = false;
        }
        Capture();
    }


    private float ToCameraShaderDepth(float depth, Camera camera)
    {
        return (depth - camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane);
    }


    private void Capture()
    {
        Camera viewCamera = Camera.main;
        MinMax[] depthRanges;
        List<Vector3>[] splitBounds = SplitScene.Execute(mSceneBoundVertexs, viewCamera, mRelativeSplitArray, out depthRanges);

        for (int i = 0; i < splitBounds.Length; i++)
        {
            mLightCamera.targetTexture = mDepthMaps[i];

            SetLightCameraFrustum.SetFitToView(mLightCamera, mLightObj, mSceneBoundVertexs, splitBounds[i]);
            mLightCamera.RenderWithShader(mCaptureShader, "RenderType");

            Shader.SetGlobalFloat("_shadowDepth" + i, ToCameraShaderDepth(depthRanges[i].min, viewCamera));
            Shader.SetGlobalTexture("_shadowDepthMap" + i, mDepthMaps[i]);
            SetProjectionMatrix.Execute(mLightCamera, "_LightProjection" + i);
        }

    }


}
