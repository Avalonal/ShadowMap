using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets._01_Basic_ShadowMap.Helper;
using ShadowMap;
using UnityEditor;

public class BasicShadowMap : MonoBehaviour
{

    public GameObject _sceneAABB;

    public FrustumType _frustumType;

    // For Render
    public RenderTexture _depthShadowMap;
    public Shader _depthCaptureShader;

    // Use this for initialization
    void Start()
    {
    }

    public void Init()
    {
        Camera lightCamera = CameraExecute(gameObject, _depthShadowMap);
        Camera viewCamera = Camera.main;
        if (_frustumType == FrustumType.FIT_TO_SCENE)
        {
            SetFitToScene(lightCamera);
        }
        else
        {
            SetFitToView(lightCamera, viewCamera);
        }
        
        lightCamera.RenderWithShader(_depthCaptureShader,"RenderType");
        CommonValues.depthCaptureShader = _depthCaptureShader;

        SetProjectionMatrix.Execute(lightCamera);
    }

    private void SetFitToScene(Camera lightCamera)
    {
        List<Vector3> sceneBoundVertexs = BoundVertexsDetector.GetSceneBoundVertexs(_sceneAABB);
        list = sceneBoundVertexs;
        SetLightCameraFrustum.SetFitToScene(lightCamera, gameObject, sceneBoundVertexs);
    }

    private void SetFitToView(Camera lightCamera, Camera viewCamera)
    {
        List<Vector3> sceneBoundVertexs = BoundVertexsDetector.GetSceneBoundVertexs(_sceneAABB);
        List<Vector3> viewBoundVertexs = BoundVertexsDetector.GetPerspectiveCameraFrustumVertexs(viewCamera);
        SetLightCameraFrustum.SetFitToView(lightCamera, gameObject, sceneBoundVertexs, viewBoundVertexs);
    }

    private List<Vector3> list;

    private void OnDrawGizmos()
    {
        if(list == null) return;
        foreach (var pos in list)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pos,2.0f);
        }
    }

    public Camera CameraExecute(GameObject parentLight, RenderTexture rt)
    {
        GameObject cameraObject = new GameObject();
        cameraObject.name = "Light Camera";
        cameraObject.transform.SetParent(parentLight.transform);
        cameraObject.transform.localPosition = Vector3.zero;
        cameraObject.transform.localRotation = Quaternion.identity;

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.targetTexture = rt;
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = Color.black;
        camera.orthographic = true;
        camera.nearClipPlane = 0;
        camera.farClipPlane = 10000;

        camera.enabled = false;


        Shader.SetGlobalTexture("_ShadowDepthMap", rt);
        CommonValues.shadowDepthMap = rt;
        return camera;
    }
}
