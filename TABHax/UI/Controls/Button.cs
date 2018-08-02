using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.UI.Controls
{
    public class Button : Control
    {
        public bool CurrentlyPressed { get; protected set; }
        public event EventHandler Click { add { onClick += value; } remove { onClick -= value; } }
        private EventHandler onClick;

        protected override Vector2 CalculateSize()
        {
            return GUI.skin.button.CalcSize(new GUIContent(Text ?? ""));
        }

        protected override void OnDraw()
        {
            bool was = CurrentlyPressed;
            CurrentlyPressed = GUI.Button(Bounds, Text ?? "<null>");
            if (CurrentlyPressed && !was)
                onClick?.Invoke(this, new EventArgs());
        }
    }
}
