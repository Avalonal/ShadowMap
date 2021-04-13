using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets._01_Basic_ShadowMap.Helper;
using Assets.ShadowCSharp;
using ShadowMap;
using UnityEngine;

class Controller : MonoBehaviour
{
    public GameObject sceneAABB;
    public Light sceneLight;

    public float bias = 0;
    public float pixelWidth = 0;
    public float pixelHeight = 0;
    public float step = 1f;
    public int depth = 1;

    public AABBManager aabbManager;

    private OctreeManager octreeManager;
    private List<DebugShadowData> list;
    private Octree root;

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

        list = octreeManager.GetDebugShadowDatas();
        //aabbManager.DoActionForEachPoint(CreateMesh, step);
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
        octreeManager = new OctreeManager(aabbManager, depth);
        root = octreeManager.BuildTree();
    }

    //private void CreateMesh(Vector3 pos)
    //{
    //    list.Add(pos);
    //}

    void OnDrawGizmos()
    {
        if(list==null) return;
        foreach (var item in list)
        {
            var pos = item.pos;
            var size = item.size;
            var strength = CommonValues.GetShadowState(pos);
            if(strength>0.3) continue;
            //if(Physics.CheckSphere(pos,size*0.5f)) continue;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pos, size * Vector3.one);
        }
    }
}

