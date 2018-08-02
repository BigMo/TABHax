using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Dumping
{
    [Serializable]
    public class DQuaternion
    {
        public float X, Y, Z, W;

        public static DQuaternion Copy(Quaternion quad)
        {
            return new DQuaternion() { X = quad.x, Y = quad.y, Z = quad.z, W = quad.w };
        }
    }
}
