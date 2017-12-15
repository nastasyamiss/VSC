using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VSC
{
    class MyFolder
    {
        public List<MyFile> filelist = new List<MyFile>();
        public string Path { get; set; }

        public void InitList()
        {
            DirectoryInfo DI = new DirectoryInfo(Path);
            FileInfo[] DIFiles = DI.GetFiles();


            foreach (FileInfo file in DIFiles)
            {
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
