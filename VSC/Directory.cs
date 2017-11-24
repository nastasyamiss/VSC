using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VSC
{
    class Directory
    {
        List<string> filelist = new List<string>();

        public void main_func()
        {
            Console.WriteLine("Введите команду:");
            string comand = Console.ReadLine();
            
            switch (comand)
            {
                case "init": init(); break;
                case "listbranch": listbranch(); break;
                default: break;
            }
            
        }
        private void init()
        {
            Console.WriteLine("Введите название папки:");
            string dir_path = Console.ReadLine();
        }
        public void status(string name)
        {

        }
        public void add(string name)
        {

        }
        public void listbranch()
        {

        }
    }
}
