using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.UI.Controls
{
    public class Window : Control
    {
        private static int WINDOW_COUNT = 0;

        public int ID { get; private set; }
        public Vector2 DragOffset { get; set; }
        public Vector2 DragSize { get; set; }
        public Rect DragBounds { get { return new Rect(DragOffset, DragSize); } set { DragOffset = value.position; DragSize = value.size; } }

        public Window()
        {
            SizeMode = eSizeMode.Manual;
            ID = WINDOW_COUNT++;
            DragOffset = Vector2.zero;
            DragSize = Size;
            Text = string.Format("{0} #{1}", GetType().Name, ID);
        }

        protected override void SetParent(Control newParent)
        {
            return;
        }

        protected override Vector2 CalculateSize()
        {
            return Size != null ? Size : Vector2.one;
        }

        protected override void OnDraw()
        {
            Bounds = GUI.Window(ID, Bounds, DrawWindow, Text ?? "");
        }

        private void DrawWindow(int id)
        {
            foreach (var c in Children)
                c.Draw();

            GUI.DragWindow(DragBounds);
        }
    }
}
