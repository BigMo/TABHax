using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.UI.Controls
{
    public class Label : Control
    {
        public Label()
        {
            SizeMode = eSizeMode.AutoSize;
        }

        protected override void OnDraw()
        {
            if (string.IsNullOrEmpty(Text))
                return;

            GUI.Label(Bounds, Text ?? "<null>");
        }

        protected override Vector2 CalculateSize()
        {
            if (string.IsNullOrEmpty(Text))
                return Vector2.zero;

            return GUI.skin.label.CalcSize(new GUIContent(Text));
        }
    }
}
