using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Dumping
{
    [Serializable]
    public class DVector3
    {
        public float X, Y, Z;

        public static DVector3 Copy(Vector3 vec)
        {
            return new DVector3() { X = vec.x, Y = vec.y, Z = vec.z };
        }
    }
}
