using System.Collections;
using Assets._01_Basic_ShadowMap.Helper;
using Assets.ShadowCSharp;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;
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
    public bool loadFromLocal = false;

    public AABBManager aabbManager;
    public List<Color> colors = new List<Color> { Color.green, Color.blue, Color.red, Color.yellow, Color.black, Color.cyan };
    public List<int> hashCode = new List<int> {17, 37, 71, 97, 113, 193, 233, 349};

    private OctreeManager octreeManager;
    private List<DebugShadowData> list;
    private Octree root;

    public Texture2D octreeTexture;
    public Material octreeMaterial;
    public Texture2D tex;

    void Start()
    {
        Init();
        //CheckTexture();
    }

    private void Init()
    {
        InitVariable();

        int bufferSize = 0;
        List<Color32> list;
        aabbManager = new AABBManager(sceneAABB);


        if (loadFromLocal)
        {
            tex = LoadLocalTexture("OctreeInShader");
            list = new List<Color32>();
            for (int i = 0; i < tex.width * tex.height; ++i)
            {
                Color32 col;
                OctreeManager.GetPixel(i, tex, out col);
                if(CommonValues.ColorEqual(col,OctreeManager.fillingColor)) break;
                list.Add(col);
            }

            bufferSize = list.Count;
            Debug.Log("Buffer Size = " + bufferSize);
        }
        else
        {
            if (sceneLight != null)
            {
                var lightController = sceneLight.GetComponent<BasicShadowMap>();
                lightController.Init();
            }
            InitSystem();
            StartCoroutine(octreeManager.TrueBuildTree());
            GC.Collect();
            Execute();
            GC.Collect();
            tex = octreeManager.SerializeOctree(out bufferSize, out list);
            CommonValues.SaveTexture2DToLacalPng(tex, "OctreeInShader");
        }
        
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        octreeTexture = tex;
        octreeMaterial.SetVector("_AABBMin", aabbManager.BasePoint);
        octreeMaterial.SetInt("_TreeDepth",depth);
        var cell = aabbManager.GetAABBLenth() / (long) (1 << depth);
        octreeMaterial.SetFloat("_AABBCell", cell);

        GenerateBufferForShader(octreeMaterial,tex,bufferSize,list);

        StringBuilder st = new StringBuilder();
        st.AppendFormat("AABB min = {0}\n", octreeMaterial.GetVector("_AABBMin"));
        st.AppendFormat("depth = {0}\n", octreeMaterial.GetInt("_TreeDepth"));
        st.AppendFormat("cell = {0}\n", octreeMaterial.GetFloat("_AABBCell"));

        Debug.Log(st.ToString());
    }

    private void GenerateBufferForShader(Material mat, Texture2D tex,int lenth,List<Color32> colors)
    {
        int bytesInUint = 4;
        int UintInCustomAttribute = 1;
        int structSize = bytesInUint * UintInCustomAttribute;
        ComputeBuffer cb = new ComputeBuffer(lenth, structSize);
        uint[] values = new uint[lenth];
        for (int i = 0; i < lenth; ++i)
        {
            uint x = colors[i].r;
            uint y = colors[i].g;
            uint z = colors[i].b;
            uint w = colors[i].a;
            values[i] = 16777216 * x + 65536 * y + 256 * z + w;
        }
        cb.SetData(values);
        mat.SetBuffer("_OctreeBuffer",cb);
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
        octreeManager = new OctreeManager(aabbManager, depth, hashCode);
    }

    private void GetDebugList()
    {
        list = octreeManager.GetDebugShadowDatas();
        Debug.Log("Get Debug List Finished!");
    }

    void OnDrawGizmos()
    {
        if(list==null||!debugToggle) return;
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

    private Texture2D LoadLocalTexture(string pngName)
    {
        string path = Application.dataPath + "/Resources/" + pngName + ".png";
        var tex = Resources.Load(pngName) as Texture2D;
        if (tex == null)
        {
            Debug.LogError("Can't find " + pngName);
        }
        return tex;
    }

    private void CheckTexture()
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < tex.width * tex.height; ++i)
        {
            Color32 col;
            OctreeManager.GetPixel(i, tex, out col);
            if (CommonValues.ColorEqual(col, OctreeManager.fillingColor)) break;
            str.AppendFormat(i + " => " + col + "\n");
        }
        Debug.Log(str.ToString());
        CommonValues.SaveTexture2DToLacalPng(tex,"check");
    }
}

//0 => RGBA(128, 0, 0, 1)
//1 => RGBA(0, 0, 0, 0)
//2 => RGBA(0, 0, 0, 0)
//3 => RGBA(0, 0, 0, 0)
//4 => RGBA(0, 0, 0, 0)
//5 => RGBA(64, 0, 0, 51)
//6 => RGBA(64, 0, 0, 2)
//7 => RGBA(0, 0, 0, 0)
//8 => RGBA(0, 0, 0, 0)




