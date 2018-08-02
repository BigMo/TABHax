using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Dumping
{
    [Serializable]
    public class DTransform
    {
        public DVector3 Position, Scale;
        public DQuaternion Rotation;

        public static DTransform Copy(Transform t) {
            return new DTransform()
            {
                Position = DVector3.Copy(t.position),
                Scale = DVector3.Copy(t.localScale),
                Rotation = DQuaternion.Copy(t.rotation)
            };
        }
    }
}
