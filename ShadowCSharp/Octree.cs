using System.Collections.Generic;
using UnityEngine;

namespace Assets.ShadowCSharp
{
    class Octree
    {
        private List<Octree> _subtree;
        private bool _inshadow;
        private bool _isCull;

        public Octree()
        {
            _subtree = new List<Octree>(8);
            _inshadow = false;
            _isCull = false;
        }

        public bool InShadow
        {
            get { return _inshadow; }
            set { _inshadow = value; }
        }

        public List<Octree> SubTree
        {
            get { return _subtree; }
            set { _subtree = value; }
        }

        public bool IsCull
        {
            get { return _isCull; }
            set { _isCull = value; }
        }

    }
}

/*
1、树本身应该只有结构信息，与场景实际位置分离；=》保证复用子树
2、


*/