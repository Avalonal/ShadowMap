using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets._01_Basic_ShadowMap.Helper;
using Assets.ShadowCSharp;
using ShadowMap;
using UnityEngine;

class Controller : MonoBehaviour
{
    public bool createTreeToggle;
    public Material ShadowMaterial;
    public Texture tex;
    public GameObject sceneAABB;
    public Light sceneLight;
    public FrustumType frustumType;

    // For Render
    public RenderTexture depthShadowMap;
    public Shader depthCaptureShader;

    public float bias;
    public float pixelWidth;
    public float pixelHeight;

    public AABBManager aabbManager;
    public Mesh sphere;
    private List<Vector3> list;

    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    private void Init()
    {
        InitVariable();
        if (sceneLight != null)
        {
            var lightController = sceneLight.GetComponent<BasicShadowMap>();
            lightController.Init();

            Execute();
        }
        InitSystem();
        aabbManager.DoActionForEachPoint(CreateMesh, 1f);
    }

    private void Execute()
    {

    }

    private void InitVariable()
    {
        CommonValues.bias = bias;
        CommonValues.pixelHeight = pixelHeight;
        CommonValues.pixelWidth = pixelWidth;
    }

    private void InitSystem()
    {
        aabbManager = new AABBManager(sceneAABB);
        list = new List<Vector3>();
    }

    private void CreateMesh(Vector3 pos)
    {
        list.Add(pos);
    }

    void OnDrawGizmos()
    {
        if(list==null) return;
        foreach (var pos in list)
        {
            if(pos.y > 1) continue;
            var strength = CommonValues.GetShadowState(pos);
            if(strength>0.8) continue;
            Gizmos.DrawSphere(pos, 0.05f);
        }
    }
}

