using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class SetProjectionMatrix
{
    public static void Execute(Camera lightCamera, string name = "_LightProjection")
    {
        Matrix4x4 posToUV = new Matrix4x4();
        posToUV.SetRow(0, new Vector4(0.5f,    0,    0, 0.5f));
        posToUV.SetRow(1, new Vector4(   0, 0.5f,    0, 0.5f));
        posToUV.SetRow(2, new Vector4(   0,    0,    1,    0));
        posToUV.SetRow(3, new Vector4(   0,    0,    0,    1));


        Matrix4x4 worldToView = lightCamera.worldToCameraMatrix;
        Matrix4x4 projection  = GL.GetGPUProjectionMatrix(lightCamera.projectionMatrix, false);

        Matrix4x4 m = posToUV * projection * worldToView;
        Shader.SetGlobalMatrix(name, m);
    }


}
