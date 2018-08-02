using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.UI.Controls
{
    public class LinearLayout : Layout
    {
        protected override bool OnReorderChildren()
        {
            Vector2 pos = Position;
            foreach (var c in Children)
            {
                if (c.RequiresResize)
                    return false;

                c.Position = pos;
                pos.y += c.Size.y;
            }
            return true;
        }
    }
}
