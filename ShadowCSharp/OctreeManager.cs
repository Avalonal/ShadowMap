using Assets._01_Basic_ShadowMap.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ShadowCSharp
{
    class OctreeManager
    {
        private int _depth;
        private float _step;
        private AABBManager _aabbManager;
        private Vector3 basePoint;
        private Octree _root;
        private List<int> stk;
        private List<DebugShadowData> _shadowDatas;

        public OctreeManager(AABBManager aabbManager, int depth)
        {
            _depth = depth;
            _aabbManager = aabbManager;
            stk = new List<int>();
            Initializer();
        }

        private void Initializer()
        {
            _step = GetSizeByDepth(_depth);
            basePoint = _aabbManager.BasePoint;
            _shadowDatas = new List<DebugShadowData>();
        }

        private float GetSizeByDepth(int depth)
        {
            return _aabbManager.GetAABBLenth()/(long)(1 << depth);
        }

        public Octree BuildTree()
        {
            if (_root == null)
                _root = Build(_depth);
            return _root;
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
                _shadowDatas.Add(new DebugShadowData(GetWorldPositionByStack(), GetSizeByDepth(stk.Count)));
                return;
            }

            for (int i = 0; i < 8; ++i)
            {
                stk.Add(i);
                CheckShadow(root.SubTree[i]);
                stk.RemoveAt(stk.Count-1);
            }
        }

        private Octree Build(int tmpDep)
        {
            Octree root = new Octree();
            if (tmpDep == 0)
            {
                Vector3 pos = GetWorldPositionByStack();
                Debug.Log(pos);
                root.InShadow = (CommonValues.GetShadowState(pos) < 0.3f);
                return root;
            }

            for (int i = 0; i < 8; ++i)
            {
                stk.Add(i);
                root.SubTree.Add(Build(tmpDep - 1));
                stk.RemoveAt(stk.Count-1);
            }

            root.CheckMerge();
            return root;
        }
        
        private Vector3 GetWorldPositionByStack()
        {
            List<int> pos = new List<int>{0,0,0};
            foreach (var tp in stk)
            {
                for (int i = 0; i < 3; ++i)
                {
                    pos[i] = (int) (pos[i] << 1) + (int)((tp >> (2-i))&1);
                }
            }
            Debug.LogFormat("x={0},y={1},z={2}",pos[0],pos[1],pos[2]);
            return GetWorldPositionByAABBPosition(pos);
        }

        private Vector3 GetWorldPositionByAABBPosition(List<int> pos)
        {
            Vector3 worldPos = new Vector3();
            for (int i = 0; i < 3; ++i)
            {
                worldPos[i] = basePoint[i] + pos[i] * _step;
            }
            return worldPos;
        }
    }
}
