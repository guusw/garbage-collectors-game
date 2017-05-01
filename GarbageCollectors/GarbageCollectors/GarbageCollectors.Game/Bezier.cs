using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;

namespace GarbageCollectors
{
    public static class Bezier
    {
        // Quickly stolen from http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
        public static float Compute(float t, float p0, float p1, float p2, float p3)
        {
            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            float r = uuu * p0; //first term
            r += 3 * uu * t * p1; //second term
            r += 3 * u * tt * p2; //third term
            r += ttt * p3; //fourth term

            return r;
        }
    }
}
