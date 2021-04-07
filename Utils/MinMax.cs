using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ronin.Utils
{
    public class MinMax
    {
        public float min;
        public float max;


        public MinMax()
        {
            min = float.MaxValue;
            max = float.MinValue;
        }

        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }


        public void Update(float value)
        {
            UpdateMin(value);
            UpdateMax(value);
        }


        public void UpdateMin(float value)
        {
            min = Mathf.Min(min, value);
        }


        public void UpdateMax(float value)
        {
            max = Mathf.Max(max, value);
        }



        public override string ToString()
        {
            return string.Format("[{0} -> {1}]", min, max);
        }
    }

}
