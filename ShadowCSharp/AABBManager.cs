using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.ShadowCSharp
{
    class AABBManager
    {
        private List<Vector3> BoundPoints;
        private Vector3 minVector = new Vector3();
        private Vector3 maxVector = new Vector3();

        public AABBManager(GameObject aabb)
        {
            if (aabb == null)
            {
                Debug.LogError("Can't find AABB.");
                return;
            }

            BoundPoints = BoundVertexsDetector.GetSceneBoundVertexs(aabb);
            InitVariable();

        }

        public float GetAABBLenth()
        {
            return maxVector[0] - minVector[0];
        }

        public Vector3 BasePoint
        {
            get { return minVector; }
        }

        public void DoActionForEachPoint(Action<Vector3> action,float step)
        {
            for (float x = minVector.x; x <= maxVector.x; x += step)
            {
                for (float y = minVector.y; y <= maxVector.y; y += step)
                {
                    for (float z = minVector.z; z <= maxVector.z; z += step)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        action(pos);
                    }
                }
            }
        }

        private void InitVariable()
        {
            foreach (var point in BoundPoints)
            {
                if (minVector == Vector3.zero)
                {
                    minVector = point;
                }

                if (maxVector == Vector3.zero)
                {
                    maxVector = point;
                }

                for (int i = 0; i < 3; ++i)
                {
                    minVector[i] = Math.Min(minVector[i], point[i]);
                    maxVector[i] = Math.Max(maxVector[i], point[i]);
                }
            }
        }
    }
}
