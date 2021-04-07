using UnityEngine;

namespace Ronin.Utils
{
    public class RoninBound3D
    {

        public float xMin = float.MaxValue, xMax = float.MinValue;
        public float yMin = float.MaxValue, yMax = float.MinValue;
        public float zMin = float.MaxValue, zMax = float.MinValue;


        public void UpdateX(float value)
        {
            xMin = Mathf.Min(xMin, value);
            xMax = Mathf.Max(xMax, value);
        }


        public void UpdateY(float value)
        {
            yMin = Mathf.Min(yMin, value);
            yMax = Mathf.Max(yMax, value);
        }


        public void UpdateZ(float value)
        {
            zMin = Mathf.Min(zMin, value);
            zMax = Mathf.Max(zMax, value);
        }


        public void Update(Vector3 value)
        {
            UpdateX(value.x);
            UpdateY(value.y);
            UpdateZ(value.z);
        }


        public float xCenter { get { return (xMin + xMax) / 2; } }
        public float yCenter { get { return (yMin + yMax) / 2; } }
        public float zCenter { get { return (zMin + zMax) / 2; } }


        public float xSize { get { return (xMax - xMin); } }
        public float ySize { get { return (yMax - yMin); } }
        public float zSize { get { return (zMax - zMin); } }


        public RoninBound3D()
        {

        }

        public RoninBound3D(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            this.zMin = zMin;
            this.zMax = zMax;
        }


        public override string ToString()
        {
            return string.Format("[x:[{0}, {1}], y:[{2}, {3}], z:[{4}, {5}]]",xMin, xMax, yMin, yMax, zMin, zMax);
        }

    }
}
