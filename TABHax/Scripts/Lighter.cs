using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Scripts
{
    public class Lighter : MonoBehaviour
    {
        public Color Color { get { return light.color; }set { light.enabled = true; light.color = value; } }

        private Light light;
        private Player player;

        void Start()
        {
            player = gameObject.GetComponent<Player>();
            light = player.m_head.gameObject.AddComponent<Light>();
            light.transform.localPosition = Vector3.up * 0.5f;
            light.color = Color.red;
            light.range = 10;
            light.intensity = 2;
            light.type = LightType.Point;
        }

        public void Disable()
        {
            light.enabled = false;
        }

        public void Enable()
        {
            light.enabled = true;
        }
    }
}
