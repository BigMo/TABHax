using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABHax.Dumping
{
    [Serializable]
    public class KeyValue
    {
        public string Key, Value;

        public static KeyValue Create(string key, string value)
        {
            return new KeyValue()
            {
                Key = key,
                Value = value
            };
        }
    }
}
