using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VSC
{
    class Compare
    {
        public void FileOpen()
        {
            StreamReader sw = new StreamReader("start.txt");
            string s = "";
            while ((s = sw.ReadLine()) != null)
            {
                Console.WriteLine(s);
            }
            Console.ReadKey();
        }
    }
}
