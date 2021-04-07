using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CaptureDepth : MonoBehaviour
{

    private Shader mRenderShader;
    public void SetCaptureShader(Shader shader)
    {
        mRenderShader = shader;
    }


    private Camera mLightCamera = null;


    private void Start()
    {
        mLightCamera = GetComponent<Camera>();
        mLightCamera.RenderWithShader(mRenderShader, "RenderType");
    }
}
