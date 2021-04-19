using System.Collections.Generic;
using UnityEngine;

namespace Assets.ShadowCSharp
{
    class Octree
    {
        private List<Octree> _subtree;
        private bool _inshadow;
        private int _ip;

        public Octree()
        {
            _subtree = new List<Octree>(8);
            _inshadow = false;
            _ip = -1;
        }

        public Octree(bool shadow,int ip = -1)
        {
            _inshadow = shadow;
            _subtree = null;
            _ip = ip;
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

        public int Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
    }
}

/*
1、树本身应该只有结构信息，与场景实际位置分离；=》保证复用子树
2、


*/