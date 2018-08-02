using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Dumping
{
    [Serializable]
    public class DGameObject
    {
        public DGameObject[] Children;
        public DComponent[] Components;
        public string Name;
        public DTransform Transform;

        public static DGameObject Copy(GameObject go)
        {
            var children = new List<DGameObject>();
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var c = go.transform.GetChild(i);
                if (go.transform != c)
                    children.Add(Copy(go.transform.GetChild(i).gameObject));
            }

            var ocomponents = go.GetComponents<Component>();
            var components = new DComponent[ocomponents.Length];
            for (int i = 0; i < ocomponents.Length; i++)
                components[i] = DComponent.Copy(ocomponents[i]);

            return new DGameObject()
            {
                Children = children.ToArray(),
                Components = components,
                Name = go.name,
                Transform = DTransform.Copy(go.transform)
            };
        }
    }
}
