using System.Collections;
using Assets._01_Basic_ShadowMap.Helper;
using Assets.ShadowCSharp;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Random = UnityEngine.Random;

class Controller : MonoBehaviour
{
    public GameObject sceneAABB;
    public Light sceneLight;

    public float bias = 0;
    public float pixelWidth = 0;
    public float pixelHeight = 0;
    public int depth = 1;

    public bool debugToggle = false;

    public AABBManager aabbManager;
    public List<Color> colors = new List<Color> { Color.green, Color.blue, Color.red, Color.yellow, Color.black, Color.cyan };
    public List<int> hashCode = new List<int> {17, 37, 71, 97, 113, 193, 233, 349};

    private OctreeManager octreeManager;
    private List<DebugShadowData> list;
    private Octree root;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        InitVariable();
        if (sceneLight != null)
        {
            var lightController = sceneLight.GetComponent<BasicShadowMap>();
            lightController.Init();
        }
        InitSystem();
        StartCoroutine(octreeManager.TrueBuildTree());
        GC.Collect();
        Execute();
    }

    private void Execute()
    {
        ThreadStart childRef = new ThreadStart(OcTreeExecute);
        Thread childThread = new Thread(childRef);
        childThread.Start();
    }

    private void OcTreeExecute()
    {
        if (debugToggle)
            GetDebugList();
    }

    private void InitVariable()
    {
        CommonValues.bias = bias;
        CommonValues.pixelHeight = pixelHeight;
        CommonValues.pixelWidth = pixelWidth;
    }

    private void InitSystem()
    {
        CommonValues.Init();
        aabbManager = new AABBManager(sceneAABB);
        octreeManager = new OctreeManager(aabbManager, depth, hashCode);
    }

    private void GetDebugList()
    {
        list = octreeManager.GetDebugShadowDatas();
        Debug.Log("Get Debug List Finished!");
    }

    void OnDrawGizmos()
    {
        if(list==null) return;
        foreach (var item in list)
        {
            var pos = item.pos;
            var size = item.size;
            var depth = item.depth;
            var strength = CommonValues.GetShadowState(pos);
            if(strength>0.3) continue;
            Gizmos.color = GetColorBySize(depth);
            //Gizmos.DrawSphere(pos,size*0.2f);
            Gizmos.DrawWireCube(pos, size * Vector3.one);
        }
    }

    private Color GetColorBySize(int dep)
    {
        return colors[(depth-dep) % colors.Count];
    }
}

