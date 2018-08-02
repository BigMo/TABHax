using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.UI.Controls
{
    public abstract class Control
    {
        public enum eSizeMode { Manual, AutoSize, AutoResize };
        public eSizeMode SizeMode
        {
            get { return sizeMode; }
            set {
                sizeMode = value;
                if (sizeMode != eSizeMode.Manual)
                    needsResize=true;
            }
        }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public string Text { get; set; }
        public Control Parent { get { return parent; } set { SetParent(value); } }
        public Control[] Children { get { return children.ToArray(); } }
        public Vector2 AbsolutePosition
        {
            get { return parent != null ? parent.AbsolutePosition + Position : Position; }
            set { if (parent != null) Position = value - parent.AbsolutePosition; else Position = value; }
        }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Margin { get; set; }
        public Rect Bounds
        {
            get { return new Rect(Position, Size); }
            set { Position = value.position; Size = value.size; }
        }
        public Rect AbsoluteBounds
        {
            get { return new Rect(AbsolutePosition, Size); }
            set { AbsolutePosition = value.position; Size = value.size; }
        }
        public bool RequiresResize { get { return needsResize; } }

        private eSizeMode sizeMode;
        private Control parent;
        private List<Control> children;
        private bool needsResize;

        public Control()
        {
            SizeMode = eSizeMode.Manual;
            ForeColor = Color.white;
            BackColor = Color.black;
            Text = GetType().Name;
            parent = null;
            children = new List<Control>();
            Position = Vector2.zero;
            Size = Vector2.zero;
            Margin = new Vector2(4, 4);
        }

        protected virtual void SetParent(Control newParent)
        {
            if ((parent != null && newParent == parent) || newParent == this)
                return;

            if (parent != null)
                parent.RemoveChild(this);

            parent = newParent;

            if (newParent != null)
                newParent.AddChild(this);
        }
        public void AddChild(Control control)
        {
            if (children.Contains(control))
                return;

            children.Add(control);
            control.SetParent(this);
            OnControlAdded(control);
        }
        protected virtual void OnControlAdded(Control control) { }
        public void RemoveChild(Control control)
        {
            if (!children.Contains(control))
                return;

            control.SetParent(null);
            children.Remove(control);
            OnControlRemoved(control);
        }
        protected virtual void OnControlRemoved(Control control) { }

        public void Draw()
        {
            if (needsResize)
            {
                Size = CalculateSize();
                if (Size != Vector2.zero)
                    needsResize = false;
            }
            OnDraw();
            if (Input.GetKey(KeyCode.KeypadMinus))
                ZatsRenderer.DrawString(Position + Size, ToString(), false);
        }

        protected abstract void OnDraw();
        protected abstract Vector2 CalculateSize();

        public override string ToString()
        {
            var str = string.Format("[{0}] \"{1}\"\nPos: {2} / Size: {3}\n{4} children",
                GetType().Name, Text ?? "null", AbsolutePosition, Size, Children.Count());

            foreach (var c in Children)
                str += "\n- " + c.ToString();

            return str;
        }
    }
}
