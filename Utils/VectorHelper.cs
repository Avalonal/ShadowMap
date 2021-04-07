using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ronin.Utils
{
    public static class VectorHelper
    {
        public static string DebugStr(this Vector3 v)
        {
            return string.Format("[{0}, {1}, {2}]", v.x, v.y, v.z);
        }


    }
}
