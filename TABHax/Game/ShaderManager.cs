using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Game
{
    public static class ShaderManager
    {
        private static Material t000, t005, t010, t025, t050, t075,
            tred, tgreen;
        public static Material TransparentMat000 { get { return t000 ?? (t000 = CreateTransparentMaterial(0f)); } }
        public static Material TransparentMat005 { get { return t005 ?? (t005 = CreateTransparentMaterial(0.05f)); } }
        public static Material TransparentMat010 { get { return t010 ?? (t010 = CreateTransparentMaterial(0.1f)); } }
        public static Material TransparentMat025 { get { return t025 ?? (t025 = CreateTransparentMaterial(0.25f)); } }
        public static Material TransparentMat050 { get { return t050 ?? (t050 = CreateTransparentMaterial(0.5f)); } }
        public static Material TransparentMat075 { get { return t075 ?? (t075 = CreateTransparentMaterial(0.75f)); } }

        public static Material MatRed { get { return tred ?? (tred = CreateTransparentMaterial(1f, Color.red)); } }
        public static Material MatGreen { get { return tgreen ?? (tgreen = CreateTransparentMaterial(1f, Color.green)); } }

        private static Texture2D CreateTransparentTexture(float a)
        {
            var tex = new Texture2D(4, 4, TextureFormat.ARGB32, false);
            var color = new Color(0, 0, 0, a);
            var colors = new Color[tex.width * tex.height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;

            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }

        private static Material CreateTransparentMaterial(float a)
        {
            return CreateTransparentMaterial(a, Color.white);
        }
        private static Material CreateTransparentMaterial(float a, Color color)
        {
            var shader = Shader.Find("UI/Default");
            var mat = new Material(shader);
            mat.color = new Color(color.r, color.g, color.b ,a);
            mat.SetColor("_Tint", new Color(color.r, color.g, color.b, a));
            mat.SetColor("Tint", new Color(color.r, color.g, color.b, a));
            return mat;
        }
    }
}
