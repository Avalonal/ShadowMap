using System.Collections;
using Assets._01_Basic_ShadowMap.Helper;
using Assets.ShadowCSharp;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

class Controller : MonoBehaviour
{
    public GameObject sceneAABB;
    public Light sceneLight;

    public float bias = 0;
    public float pixelWidth = 0;
    public float pixelHeight = 0;
    public int depth = 1;

    public bool physicsTest = true;
    public bool debugToggle = false;

    public AABBManager aabbManager;
    public List<Color> colors = new List<Color> { Color.green, Color.blue, Color.red, Color.yellow, Color.black, Color.cyan };

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

            Execute();
        }
        InitSystem();

        if(debugToggle)
            StartCoroutine(GetDebugList());
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
        octreeManager = new OctreeManager(aabbManager, depth,physicsTest);
        octreeManager.BuildTree();
        StartCoroutine(octreeManager.TrueBuildTree());
    }

    IEnumerator GetDebugList()
    {
        list = octreeManager.GetDebugShadowDatas();
        Debug.Log("Get Debug List Finished!");
        yield return 0;
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

