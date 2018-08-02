using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Dumping
{
    [Serializable]
    public class DComponent
    {
        public string Name;
        public string Tag;
        public KeyValue[] Fields;
        public KeyValue[] Properties;

        internal static DComponent Copy(Component component)
        {
            var t = component.GetType();
            var fields = t.GetFields();
            var props = t.GetProperties();
            var dfields = new List<KeyValue>();
            var dprops = new List<KeyValue>();

            foreach (var f in fields)
                try
                {
                    var val = f.GetValue(component);
                    dfields.Add(KeyValue.Create(f.Name, val != null ? val.ToString() : "null"));
                }
                catch { }
            foreach (var p in props)
                try
                {
                    var val = p.GetValue(component, null);
                    dfields.Add(KeyValue.Create(p.Name, val != null ? val.ToString() : "null"));
                }
                catch { }

            return new DComponent()
                {
                    Name = component.name,
                    Fields = dfields.ToArray(),
                    Properties = dprops.ToArray()
            };
                }
    }
}
