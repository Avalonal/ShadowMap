using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.ShadowCSharp
{
    struct DebugShadowData
    {
        public Vector3 pos;
        public float size;

        public DebugShadowData(Vector3 p, float s)
        {
            pos = p;
            size = s;
        }
    }
}
