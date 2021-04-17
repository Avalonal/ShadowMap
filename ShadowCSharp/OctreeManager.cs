using System;
using System.Collections;
using Assets._01_Basic_ShadowMap.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ShadowCSharp
{
    class OctreeManager
    {
        private int _depth;
        private AABBManager _aabbManager;
        private Vector3 basePoint;
        private Octree _root;
        private List<int> stk;
        private List<DebugShadowData> _shadowDatas;
        private List<int> _hashCode;
        private Dictionary<int, Octree> _dic;
        private List<int> _primeList;
        private const int mod = (int) 1e9 + 7;

        private Octree inshadowTree = new Octree(true);
        private Octree outshadowTree = new Octree(false);

        public OctreeManager(AABBManager aabbManager, int depth, List<int> hashCode)
        {
            _depth = depth;
            _aabbManager = aabbManager;
            _hashCode = hashCode;

            stk = new List<int>();
   
            InitHashSystem();
            Initializer();
        }

        private void InitHashSystem()
        {
            int N = (int) 1e5;
            _dic = new Dictionary<int, Octree>();
            _primeList = new List<int>();
            bool[] vis = new bool[N];
            vis[0] = vis[1] = false;
            for (int i = 2; i < N; ++i)
            {
                if (!vis[i])
                {
                    _primeList.Add(i);
                }

                for (int j = 1; j < _primeList.Count && i * _primeList[j] < N; ++j)
                {
                    vis[i * _primeList[j]] = true;
                    if (i % _primeList[j] == 0) break;
                }
            }
        }

        private void Initializer()
        {
            basePoint = _aabbManager.BasePoint;
            _shadowDatas = new List<DebugShadowData>();
        }

        private float GetSizeByDepth(int depth)
        {
            return _aabbManager.GetAABBLenth() / (long) (1 << depth);
        }

        public void BuildTree()
        {
            if (_root == null)
            {
                Build(_depth, out _root);
            }
            Debug.Log("Dic size = " + _dic.Count);
        }

        public IEnumerator TrueBuildTree()
        {
            BuildTree();
            Debug.Log("Build Tree Finished!");
            yield return 0;
        }

        public List<DebugShadowData> GetDebugShadowDatas()
        {
            stk.Clear();
            CheckShadow(_root);
            return _shadowDatas;
        }

        private void CheckShadow(Octree root)
        {
            if (root.SubTree == null || root.SubTree.Count <= 0)
            {
                var pos = GetWorldPositionByStack();
                var size = GetSizeByDepth(stk.Count);
                _shadowDatas.Add(new DebugShadowData(pos, size, stk.Count));
                return;
            }

            for (int i = 0; i < 8; ++i)
            {
                stk.Add(i);
                CheckShadow(root.SubTree[i]);
                stk.RemoveAt(stk.Count - 1);
            }
        }

        private KeyValuePair<int, int> Build(int tmpDep, out Octree root)
        {
            root = new Octree();
            if (tmpDep == 0)
            {
                Vector3 pos = GetWorldPositionByStack();
                //Debug.Log(pos);
                root.SubTree.Clear();
                root.InShadow = (CommonValues.GetShadowState(pos) < 0.3f);
                if (root.InShadow) root = inshadowTree;
                else root = outshadowTree;

                //return root;
                return new KeyValuePair<int, int>(0,root.InShadow?1 : 0);
            }

            bool flag = true;

            int sz = 1;
            int hashval = 0;

            for (int i = 0; i < 8; ++i)
            {
                stk.Add(i);
                var pos = GetWorldPositionByStack();
                Octree subTree;
                var subVal = Build(tmpDep - 1, out subTree);
                root.SubTree.Add(subTree);
                bool isCull = false;
                if (subTree.InShadow)
                {
                    isCull = Physics.CheckSphere(pos, GetSizeByDepth(stk.Count) * 0.4f);
                }

                hashval = (hashval + _hashCode[i] * _primeList[subVal.Key] % mod * subVal.Value % mod) % mod;
                sz += subVal.Key;

                stk.RemoveAt(stk.Count - 1);
                if (flag)
                {
                    if (root.SubTree[i].SubTree != null && root.SubTree[i].SubTree.Count > 0) flag = false;
                    if (i == 0)
                        root.InShadow = root.SubTree[i].InShadow;
                    else if (isCull) continue;
                    else if (root.InShadow != root.SubTree[i].InShadow)
                        flag = false;
                }
            }

            if (flag)
            {
                root.SubTree.Clear();

                if (root.InShadow) root = inshadowTree;
                else root = outshadowTree;

                return new KeyValuePair<int, int>(0, root.InShadow ? 1 : 0);
            }

            if (_dic.ContainsKey(hashval)) root = _dic[hashval];
            else _dic.Add(hashval,root);

            return new KeyValuePair<int, int>(sz,hashval);
        }

        private Vector3 GetWorldPositionByStack()
        {
            List<int> pos = new List<int> {0, 0, 0};
            foreach (var tp in stk)
            {
                for (int i = 0; i < 3; ++i)
                {
                    pos[i] = (int) (pos[i] << 1) + (int) ((tp >> (2 - i)) & 1);
                }
            }

            //Debug.LogFormat("x={0},y={1},z={2}",pos[0],pos[1],pos[2]);
            return GetWorldPositionByAABBPosition(pos);
        }

        private Vector3 GetWorldPositionByAABBPosition(List<int> pos)
        {
            Vector3 worldPos = new Vector3();
            for (int i = 0; i < 3; ++i)
            {
                worldPos[i] = basePoint[i] + (pos[i] + 0.5f) * GetSizeByDepth(stk.Count);
            }

            return worldPos;
        }
    }
}