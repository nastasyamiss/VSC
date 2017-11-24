using System;
using System.IO;
/*using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/


namespace VSC
{
    class Start
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public  { get; set; }

        public void FolderOpen()
        {
            string path = "start.txt";
            string dir_path, filename="";
            long size;
            DateTime create, modif;
            Console.WriteLine("folder");
            dir_path = Console.ReadLine();

            DirectoryInfo DI = new DirectoryInfo(dir_path);
            FileInfo[] fi = DI.GetFiles();

           
            StreamWriter sw = new StreamWriter(path);
            

            if (!DI.Exists)      //Если не существует каталог/папка
            {
                Console.WriteLine("The folder didn't found");
            }
            else 
            {
                
                // Print out the names of the files in the current directory.
                foreach (FileInfo fiTemp in fi)
                {
   
                    filename = fiTemp.Name;
                    
                    size = fiTemp.Length;
                    create = fiTemp.CreationTime;
                    modif = fiTemp.LastWriteTime;
                    sw.Write(filename + ' '+size +' '+create+' '+modif+'\n');
                    Console.WriteLine("file: "+ filename);
                    Console.WriteLine("size: "+ size);
                    Console.WriteLine("created: " + create);
                    Console.WriteLine("modified: " + modif);
                    Console.WriteLine();
                   
                }
            }

            sw.Close();
            Console.ReadKey();
            Compare st2 = new Compare();
            st2.FileOpen();
        }
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            /*string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                Console.WriteLine("file: ", fileName);
                DateTime fileCreatedDate = File.GetCreationTime(fileName);
                Console.WriteLine("created: " + fileCreatedDate);
                Console.WriteLine("size: " + fileName.Length);
                Console.WriteLine();
            }*/
            
           

            // Recurse into subdirectories of this directory.
            /*string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);*/
        }
       
        }
}
