using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VSC
{
    [Serializable]
    public class MyFile
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime Create { get; set; }
        public DateTime Modified { get; set; }
        public string Label { get; set; }

    }
}
