using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.UI.Controls
{
    public abstract class Layout : Control
    {
        private bool needsReorder;
        private Vector2 maxSize = Vector2.zero;

        public bool AutoReorderChildren { get; set; }

        public Layout()
        {
            SizeMode = eSizeMode.AutoResize;
            AutoReorderChildren = true;
        }

        protected override void OnDraw()
        {
            if (needsReorder)
            {
                if (OnReorderChildren())
                {
                    maxSize = FindMaxSize();
                    needsReorder = false;
                }
            }
            foreach (var c in Children)
                c.Draw();
        }

        protected override Vector2 CalculateSize()
        {
            switch (SizeMode)
            {
                case eSizeMode.Manual:
                    return Size;
                default:
                    return maxSize;
            }                
        }

        private Vector2 FindMaxSize()
        {
            Vector2 b = Vector2.zero;
            foreach (var c in Children)
            {
                if (c.Bounds.xMax > b.x)
                    b.x = c.Bounds.xMax;
                if (c.Bounds.yMax > b.y)
                    b.y = c.Bounds.yMax;
            }
            return b;
        }

        protected override void OnControlAdded(Control control)
        {
            base.OnControlAdded(control);
            if (AutoReorderChildren)
                ReorderChildren();
        }

        protected override void OnControlRemoved(Control control)
        {
            base.OnControlRemoved(control);
            if (AutoReorderChildren)
                ReorderChildren();
        }

        public void ReorderChildren()
        {
            needsReorder = true;
        }

        protected abstract bool OnReorderChildren();
    }
}
