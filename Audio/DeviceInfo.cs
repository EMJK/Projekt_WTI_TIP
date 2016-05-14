using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio
{
    public class DeviceInfo
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public override string ToString()
        {
            return $"[{ID}] {Name}";
        }
    }
}
