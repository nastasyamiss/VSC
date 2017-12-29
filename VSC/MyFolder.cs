using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace VSC
{
    class MyFolder
    {
        public List<MyFile> filelist = new List<MyFile>();
        public string Path { get; set; }

        public void InitList(params string[] removes)
        {
            DirectoryInfo DI = new DirectoryInfo(Path);
            FileInfo[] DIFiles = DI.GetFiles();

            foreach (FileInfo file in DIFiles)
            {
                if (!removes.Contains(file.Name))
                    filelist.Add(new MyFile()
                {

                    Name = file.Name,
                    Size = file.Length,
                    Create = file.CreationTime,
                    Modified = file.LastWriteTime,
                    Label = ""
                });                

            }
        }
    }
}
