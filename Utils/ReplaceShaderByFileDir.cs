/*
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ReplaceShaderByFileDir : EditorWindow
{
    //Shader shader;
    public Material material;
    bool isShowReplaceGo = false;  //是否显示被替换的物体
    string tipMsg = null;
    MessageType tipMsgType = MessageType.Info;
    List<GameObject> replaceGoList = new List<GameObject>();
    int matCount = 0;   //材质的数量
    Vector2 scrollPos = Vector2.zero;

    [MenuItem("Editor/替换场景中的material")]
    public static void OpenWindow()
    {
        ReplaceShaderByFileDir window = (ReplaceShaderByFileDir)EditorWindow.GetWindow<ReplaceShaderByFileDir>(false, "替换场景中的shader");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("替换mat ：");
        material = (Material)EditorGUILayout.ObjectField(material, typeof(Material), true);

        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("批量替换", GUILayout.Height(30)))
        {
            Replace();
        }

        if (GUILayout.Button("重置", GUILayout.Height(30)))
        {
            Reset();
        }
        GUILayout.EndHorizontal();

        //提示信息
        if (!string.IsNullOrEmpty(tipMsg))
        {
            EditorGUILayout.HelpBox(tipMsg, tipMsgType);
        }

        isShowReplaceGo = GUILayout.Toggle(isShowReplaceGo, "显示被替换的GameObject");
        if (isShowReplaceGo)
        {
            if (replaceGoList.Count > 0)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 200));
                foreach (var go in replaceGoList)
                {
                    EditorGUILayout.ObjectField(go, typeof(GameObject), true);
                }
                GUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("替换个数为0");
            }
        }
    }

    void Replace()
    {
        replaceGoList.Clear();

        /*Dictionary<GameObject, Material[]> matDict =#1# GetAllScenceMaterial();

        /*List<Material> replaceMatList = new List<Material>();
        foreach (var item in matDict)
        {
            GameObject tempGo = item.Key;
            Material[] mats = item.Value;

            int length = mats.Length;
            for (int i = 0; i < length; i++)
            {
                var mat = mats[i];
                if (mat != null)
                {
                    if (!mat.Equals(material))
                    {
                        replaceGoList.Add(tempGo);
                        if (!replaceMatList.Contains(mat))
                            replaceMatList.Add(mat);
                    }
                }
            }
        }

        int replaceMatCount = replaceMatList.Count;   //替换Material的数量
        for (int i = 0; i < replaceMatCount; i++)
        {
            UpdateProgress(i, replaceMatCount, "替换中...");
            replaceMatList[i] = material;
            EditorUtility.SetDirty(replaceMatList[i]);
        }#1#

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        /*tipMsg = "替换成功！替换了" + replaceMatCount + "个Material," + replaceGoList.Count + "个GameObject";
        tipMsgType = MessageType.Info;#1#

        EditorUtility.ClearProgressBar();
    }

    void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    void Reset()
    {
        tipMsg = null;
        material = null;
        matCount = 0;
        replaceGoList.Clear();
        isShowReplaceGo = false;
    }

    /// <summary>
    /// 获取所有场景中的Material
    /// </summary>
    /// <returns></returns>
    void GetAllScenceMaterial()
    {
        Dictionary<GameObject, Material[]> dict = new Dictionary<GameObject, Material[]>();
        List<GameObject> gos = GetAllSceneGameObject();
        foreach (var go in gos)
        {
            Renderer render = go.GetComponent<Renderer>();
            if (render != null)
            {
                Material[] mats = render.sharedMaterials;
                if (mats != null && mats.Length > 0)
                {
                    for (int i = 0; i < mats.Length; ++i)
                        mats[i] = material;
                }
            }
        }
    }


    /// <summary>
    /// 获取所有场景中的物体
    /// </summary>
    /// <returns></returns>
    List<GameObject> GetAllSceneGameObject()
    {
        List<GameObject> list = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootGos = scene.GetRootGameObjects();
        foreach (var go in rootGos)
        {
            Transform[] childs = go.transform.GetComponentsInChildren<Transform>(true);
            foreach (var child in childs)
            {
                list.Add(child.gameObject);
            }
        }
        return list;
    }



}
*/
