using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Ronin.Utils;

public static class SplitScene
{


    private static MinMax GetSceneDepthRange(Camera camera, List<Vector3> sceneBoundVertexs)
    {
        MinMax range = new MinMax();
        Matrix4x4 trans = camera.transform.worldToLocalMatrix;

        // 先获取场景的所有顶点在camera下的深度的范围
        foreach (Vector3 p in sceneBoundVertexs)
        {
            range.Update(trans.MultiplyPoint(p).z);
        }

        // 再和camera原有的深度范围比较
        // 对 near 来说，如果camera的near更大，则意味着一部分场景中的物体被near截断，所以不需要绘制，因此取大值即可。
        // far 同理
        range.min = Mathf.Max(camera.nearClipPlane, range.min);
        range.max = Mathf.Min(camera.farClipPlane,  range.max);
        return range;
    }


    private static MinMax[] GetSplitDepthRangeList(float [] relativeSplitArray, MinMax wholeRange)
    {
        MinMax[] splitDepthLit = new MinMax[relativeSplitArray.Length + 1];
        float rangeLength = wholeRange.max - wholeRange.min;
        for (int i = 0; i < splitDepthLit.Length; i ++)
        {
            splitDepthLit[i] = new MinMax(
                i <= 0 ?
                    wholeRange.min :
                    wholeRange.min + relativeSplitArray[i - 1] * rangeLength,
                i >= relativeSplitArray.Length ?
                    wholeRange.max :
                    wholeRange.min + relativeSplitArray[  i  ] * rangeLength);
        }
        return splitDepthLit;
    }




    private static List<Vector3> GetBoundVertexs(MinMax depthRange, Camera camera)
    {
        List<Vector3> vertexs = new List<Vector3>();
        Vector3 nearSize = BoundVertexsDetector.GetFrustumSectionSize(camera, depthRange.min);
        Vector3 farSize  = BoundVertexsDetector.GetFrustumSectionSize(camera, depthRange.max);

        Matrix4x4 trans = camera.transform.localToWorldMatrix;
        BoundVertexsDetector.AddOriginCenteredSquare(vertexs, trans, nearSize.x, nearSize.y, nearSize.z);
        BoundVertexsDetector.AddOriginCenteredSquare(vertexs, trans, farSize .x, farSize .y, farSize .z);

        return vertexs;
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneAABB"></param>
    /// <param name="viewCamera"></param>
    /// <param name="splitDepthArray">分割场景的方法，取值在0~1</param>
    /// <returns></returns>
    public static List<Vector3> [] Execute(List<Vector3> sceneBoundVertexs, Camera viewCamera, float [] relativeSplitArray, out MinMax[] depthRange)
    {
        MinMax sceneDepthRange = GetSceneDepthRange(viewCamera, sceneBoundVertexs);
        depthRange = GetSplitDepthRangeList(relativeSplitArray, sceneDepthRange);

        List<Vector3>[] bounds = new List<Vector3>[depthRange.Length];
        for (int i = 0; i < bounds.Length; i ++)
        {
            bounds[i] = GetBoundVertexs(depthRange[i], viewCamera);
        }

        return bounds;
    }


}
