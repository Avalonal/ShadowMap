using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ShadowMap;
using Ronin.Utils;

public class CascadeShadowMap : MonoBehaviour
{

    public GameObject sceneAABB;
    public float[] relativeSplitArray;
    public Shader depthCaptureShader;


    // Use this for initialization
    void Start()
    {
        List<Vector3> sceneBoundVertexs = BoundVertexsDetector.GetSceneBoundVertexs(sceneAABB);

        Camera lightCamera   = CreateCamera.Execute(gameObject, null);
        CaptureCSM depthCapturer = lightCamera.gameObject.AddComponent<CaptureCSM>();

        depthCapturer.Init(sceneBoundVertexs, depthCaptureShader, relativeSplitArray, gameObject);
    }

}
