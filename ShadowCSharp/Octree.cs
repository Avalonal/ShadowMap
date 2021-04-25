using System.Collections.Generic;
using UnityEngine;

namespace Assets.ShadowCSharp
{
    class Octree
    {
        public Octree()
        {
            SubTree = new List<Octree>(8);
            InShadow = false;
            Ip = -1;
        }

        public Octree(bool shadow)
        {
            InShadow = shadow;
            SubTree = null;
            Ip = -1;
        }

        public bool InShadow { get; set; }

        public List<Octree> SubTree { get; set; }

        public int Ip { get; set; }
    }
}

/*
1、树本身应该只有结构信息，与场景实际位置分离；=》保证复用子树
2、


*/