using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Ronin.Utils;


public static class SetLightCameraFrustum
{

    private static RoninBound3D ConvertToBound(Matrix4x4 trans, List<Vector3> sceneBoundVertexs)
    {
        RoninBound3D bound = new RoninBound3D();
        sceneBoundVertexs.ForEach(vertex => bound.Update(trans.MultiplyPoint(vertex)));
        return bound;
    }


    public static void SetFitToScene(Camera lightCamera, GameObject light, List<Vector3> sceneBoundVertexs)
    {
        RoninBound3D bound = ConvertToBound(light.transform.worldToLocalMatrix, sceneBoundVertexs);

        lightCamera.transform.localPosition = new Vector3(bound.xCenter, bound.yCenter, bound.zMin <= 0.1f ? bound.zMin - 0.1f : 0);
        lightCamera.orthographicSize = Mathf.Max(bound.xSize / 2, bound.ySize / 2);
        lightCamera.nearClipPlane = bound.zMin - lightCamera.transform.localPosition.z;
        lightCamera.farClipPlane  = bound.zMax - lightCamera.transform.localPosition.z;
    }



    public static void SetFitToView(Camera lightCamera, GameObject light, List<Vector3> sceneBoundVertexs, List<Vector3> cameraFrustumVertexs)
    {
        RoninBound3D sceneBound = ConvertToBound(light.transform.worldToLocalMatrix, sceneBoundVertexs);
        RoninBound3D viewBound  = ConvertToBound(light.transform.worldToLocalMatrix, cameraFrustumVertexs);

        lightCamera.transform.localPosition = new Vector3(viewBound.xCenter, viewBound.yCenter, 0);
        lightCamera.orthographicSize = Mathf.Max(viewBound.xSize / 2, viewBound.ySize / 2);
        lightCamera.nearClipPlane = sceneBound.zMin;
        lightCamera.farClipPlane  = sceneBound.zMax;
    }

}
