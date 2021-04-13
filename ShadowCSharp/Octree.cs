using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.ShadowCSharp
{
    class Octree
    {
        private List<Octree> _subtree;
        private bool _inshadow;
        public Octree()
        {
            _subtree = new List<Octree>(8);
            _inshadow = false;
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

        public bool CheckMerge()
        {
            if (_subtree == null || _subtree.Count <= 0)
                return true;
            
            for (int i = 0; i < 8; ++i)
            {
                bool subShadow = _subtree[i].InShadow;
                if (!_subtree[i].CheckMerge())
                    return false;
                if (i == 0) _inshadow = subShadow;
                else if (subShadow != _inshadow) return false;
            }
            _subtree.Clear();
            return true;
        }
    }
}

/*
1、树本身应该只有结构信息，与场景实际位置分离；=》保证复用子树
2、


*/