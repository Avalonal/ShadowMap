using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ShadowMap;

public class BasicShadowMap : MonoBehaviour
{

    public GameObject sceneAABB;

    public FrustumType frustumType;

    // For Render
    public RenderTexture depthShadowMap;
    public Shader depthCaptureShader;



    // Use this for initialization
    void Start()
    {
        Camera lightCamera = CreateCamera.Execute(gameObject, depthShadowMap);
        Camera viewCamera  = Camera.main;
        if (frustumType == FrustumType.FIT_TO_SCENE)
        {
            SetFitToScene(lightCamera);
        } else
        {
            SetFitToView(lightCamera, viewCamera);
        }

        CaptureDepth depthCapturer = lightCamera.gameObject.AddComponent<CaptureDepth>();
        depthCapturer.SetCaptureShader(depthCaptureShader);

        SetProjectionMatrix.Execute(lightCamera);
    }

    private void SetFitToScene(Camera lightCamera)
    {
        List<Vector3> sceneBoundVertexs = BoundVertexsDetector.GetSceneBoundVertexs(sceneAABB);
        SetLightCameraFrustum.SetFitToScene(lightCamera, gameObject, sceneBoundVertexs);
    }

    private void SetFitToView(Camera lightCamera, Camera viewCamera)
    {
        List<Vector3> sceneBoundVertexs = BoundVertexsDetector.GetSceneBoundVertexs(sceneAABB);
        List<Vector3> viewBoundVertexs = BoundVertexsDetector.GetPerspectiveCameraFrustumVertexs(viewCamera);
        SetLightCameraFrustum.SetFitToView(lightCamera, gameObject, sceneBoundVertexs, viewBoundVertexs);
    }


}
