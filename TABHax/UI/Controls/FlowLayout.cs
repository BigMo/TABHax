using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.UI.Controls
{
    public class FlowLayout : Layout
    {
        public FlowLayout()
        {
            SizeMode = eSizeMode.Manual;
            AutoReorderChildren = false;
        }

        protected override bool OnReorderChildren()
        {
            float x = 0, y = 0, largestY = 0;
            foreach (var c in Children)
            {
                if (c.RequiresResize)
                    return false;

                var size = c.Size;

                //Put into current line
                if (x + size.x < Size.x)
                {
                    c.Position = Position + new Vector2(x, y);
                    x += size.x;
                }
                else //Advance into next line
                {
                    y += largestY;
                    x = 0f;
                    c.Position = Position + new Vector2(x, y);
                }
                //Apply largest height
                if (size.y > largestY)
                    largestY = size.y;

                Console.WriteLine("Placed \"{0}\" at {1}", c.Text ?? c.GetType().Name, c.Position);
            }
            return true;
        }
    }
}
