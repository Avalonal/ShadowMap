using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Ronin.Utils;

public static class BoundVertexsDetector
{

    /// <summary>
    /// 向列表中添加一个以（0,0,z）为中心的正方形的四个顶点
    /// </summary>
    public static void AddOriginCenteredSquare(List<Vector3> list, Matrix4x4 trans, float xSize, float ySize, float z)
    {
        list.Add(trans.MultiplyPoint(new Vector3(-xSize / 2, -ySize / 2, z)));
        list.Add(trans.MultiplyPoint(new Vector3(-xSize / 2,  ySize / 2, z)));
        list.Add(trans.MultiplyPoint(new Vector3( xSize / 2, -ySize / 2, z)));
        list.Add(trans.MultiplyPoint(new Vector3( xSize / 2,  ySize / 2, z)));
    }

    public static List<Vector3> GetSceneBoundVertexs(GameObject sceneAABB)
    {
        List<Vector3> vertexs = new List<Vector3>();

        Matrix4x4 trans = sceneAABB.transform.localToWorldMatrix;
        AddOriginCenteredSquare(vertexs, trans, 1, 1, -0.5f);
        AddOriginCenteredSquare(vertexs, trans, 1, 1,  0.5f);
        return vertexs;
    }

    public static Vector3 GetFrustumSectionSize(Camera camera, float z)
    {
        Vector3 v;
        v.x = z * Mathf.Tan(camera.fieldOfView / 2 * Mathf.Deg2Rad);
        v.y = v.x * camera.aspect;
        v.z = z;
        return v;
    }


    public static List<Vector3> GetPerspectiveCameraFrustumVertexs(Camera camera)
    {
        List<Vector3> vertexs = new List<Vector3>();
        Vector3 nearSize = GetFrustumSectionSize(camera, camera.nearClipPlane);
        Vector3 farSize  = GetFrustumSectionSize(camera, camera.farClipPlane);

        Matrix4x4 trans = camera.transform.localToWorldMatrix;
        AddOriginCenteredSquare(vertexs, trans, nearSize.x, nearSize.y, nearSize.z);
        AddOriginCenteredSquare(vertexs, trans, farSize. x, farSize .y, farSize .z);

        return vertexs;
    }


}
