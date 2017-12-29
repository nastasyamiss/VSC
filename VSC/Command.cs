using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VSC
{
    class Command
    {
        public static List<MyFolder> FolderList = new List<MyFolder>();
        public static MyFolder ActiveDir = new MyFolder();
        
        List<string> temp = new List<string>();
        public void FileOpen(string way)
        {
            string line = "";
            using (StreamReader sr = new StreamReader("1.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                        ActiveDir = new MyFolder() { Path = line };
                        FolderList.Add(ActiveDir);   
                }
                sr.Close();
            }
            var binFormat = new BinaryFormatter();
            using (var fs = new FileStream(way, FileMode.Open))
            {
                while (fs.Length != fs.Position)
                {
                    foreach (MyFolder folder in FolderList)
                    {
                    
                        ActiveDir = folder;
                        ActiveDir.filelist = ((List<MyFile>)binFormat.Deserialize(fs));

                        //Console.WriteLine("Имя: {0} \t Возраст: {1}", dsUser.Name, dsUser.Age);
                    }
                }
            }
            /*XmlSerializer ser = new XmlSerializer(typeof(List<MyFile>));
            FileStream file = new FileStream(way, FileMode.Open, FileAccess.Read, FileShare.None);

            while (file.Length != file.Position)
            {

                //ser.Serialize(file, ActiveDir.filelist);

                ActiveDir.filelist = (List<MyFile>)ser.Deserialize(file);
                //file.Close();
            }*/
            
           
            /*formatter.Serialize(fs, people);
            }
 
            using (FileStream fs = new FileStream("people.xml", FileMode.OpenOrCreate))
            {
                Person[] newpeople = (Person[])formatter.Deserialize(fs);*/

        }
        public void Init(string dir_path)
        {
            if (Directory.Exists(dir_path))
            {
                bool flag = true;
                foreach (MyFolder folder in FolderList)
                {
                    if (dir_path == folder.Path) { flag = false; Console.WriteLine("Директория уже отслеживается"); return; }
                }
                if (flag == true)
                {
                    ActiveDir = new MyFolder() { Path = dir_path };
                    ActiveDir.InitList();
                    FolderList.Add(ActiveDir);
                    Console.WriteLine("Директория добавлена");
                }
            }
            else
                Console.WriteLine("The folder didn't found");

        }
        
        public string BytesToString(long byteCount)
        {
            string[] suf = {"B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
        private void Print(string myfile, string metka = "", bool color = false, string msize = "", string mmodif = "", string mcraete = "")
        {
            foreach (MyFile file1 in ActiveDir.filelist)
            {
                if (file1.Name == myfile)
                {
                    if (color == true)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("file: {0} {1}", file1.Name, metka);
                    Console.WriteLine("size: {0} {1}", BytesToString(file1.Size), msize);
                    Console.WriteLine("created: {0} {1}", file1.Create, mcraete);
                    Console.WriteLine("modified: {0} {1}", file1.Modified, mmodif);
                    Console.WriteLine();
                    Console.ResetColor();
                }
            }
        }
        public void Status()
        {
            if (FolderList.Count == 0)
            {
                Console.WriteLine("Ни одна папка не проинициализирована");
                return;
            }
            List<MyFile> OldFileNames = new List<MyFile>(); //старое состояние папки
            List<MyFile> AllFiles = new List<MyFile>(); //объединенное состояние
            List<MyFile> NewFileNames = new List<MyFile>(); //новое состояние папки

            DirectoryInfo di = new DirectoryInfo(ActiveDir.Path);
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    NewFileNames.Add(new MyFile()
                    {
                        Name = file.Name,
                        Size = file.Length,
                        Create = file.CreationTime,
                        Modified = file.LastWriteTime,
                    });
                }
            }
            else
                Console.WriteLine("Такой папки не существут");

            foreach (MyFile file in ActiveDir.filelist)
            {
                OldFileNames.Add(new MyFile() {
                    Name = file.Name,
                    Size = file.Size,
                    Create = file.Create,
                    Modified = file.Modified,
                    Label=file.Label
                });
            }
            AllFiles = OldFileNames.Union(NewFileNames).ToList();
            AllFiles = AllFiles.GroupBy(fi => fi.Name).Select(g => g.First()).ToList(); //убираем повторения
            
            foreach (MyFile file in AllFiles)
            {
                int iold = -1;
                int inew = -1;
                for (int i = 0; i < OldFileNames.Count; i++)
                    if (file.Name == OldFileNames[i].Name)
                        iold = i;
                for (int i = 0; i < NewFileNames.Count; i++)
                    if (file.Name == NewFileNames[i].Name)
                        inew = i;

                if (iold == -1 && inew == -1)
                    Console.WriteLine("Такое не должно быть(по идее)");
                else if (iold == -1 && inew != -1) //если файл есть только в списке NewFileNames, в папке появился новый файл
                {
                    FileInfo newfile = new FileInfo(ActiveDir.Path + @"\" + NewFileNames[inew].Name);
                    Console.ForegroundColor = ConsoleColor.Green;
                    
                    Console.WriteLine("file: " + newfile.Name + " <-- new");
                    Console.WriteLine("size: " + BytesToString(newfile.Length));
                    Console.WriteLine("created: " + newfile.CreationTime);
                    Console.WriteLine("modified: " + newfile.LastWriteTime);
                    Console.WriteLine();
                    Console.ResetColor();
                    continue;
                }
                else if (iold != -1 && inew == -1) //файл удален или перемещен из папки
                {
                    Print(file.Name, "<-- deleted", true); //true - red colour
                    continue;
                }
                else if ((iold != -1 && inew != -1))
                {
                    string m = "";
                    string size = "";
                    string c = "";

                    if (OldFileNames[iold].Label == "<-- removed") //если файл уже убрали из версионного контроля нет смысла проверять дальше
                    {
                        Print(file.Name, "<-- removed", true);
                        continue;
                    }
                    else
                    {
                        if (NewFileNames[inew].Size != OldFileNames[iold].Size) //изменился ли размер файла
                        {
                            size = "<-- " + BytesToString(NewFileNames[inew].Size);
                            //Print(file.Name, "", true,  + size);
                        }
                        if (NewFileNames[inew].Modified != OldFileNames[iold].Modified) // или просто файл меняли, но размер не менялся
                        {
                            m = "<-- " + NewFileNames[inew].Modified;

                        }
                        if (NewFileNames[inew].Create != OldFileNames[iold].Create) //или удалили и потом обратно создали??
                        {
                            c = "<-- " + NewFileNames[inew].Create;
                        }
                        if (size != "" || m != "" || c != "")
                            Print(file.Name, "", true, size, m, c);


                        else Print(file.Name, OldFileNames[iold].Label);
                    }
                }
               
            }
        }

        public void AddFile(string name)
        {

            if (FolderList.Count == 0)
            {
                Console.WriteLine("Ни одна папка не проинициализирована");
                return;
            }
            if (!File.Exists(ActiveDir.Path + @"\" + name))
            {
                Console.WriteLine("Файл не существует");
                return;
            }
            foreach (MyFile file in ActiveDir.filelist)
            {
                if (name == file.Name)
                {
                    Console.WriteLine("Файл уже под версионным контролем");
                    return;
                }

            }
            FileInfo newfile = new FileInfo(ActiveDir.Path + @"\" + name);
            ActiveDir.filelist.Add(new MyFile()
            {
                Name = newfile.Name,
                Size = newfile.Length,
                Create = newfile.CreationTime,
                Modified = newfile.LastWriteTime,
                Label = "<-- added"
            });
            Console.WriteLine("Файл добавлен");
        }

        public void RemoveFile(string name)
        {

            if (FolderList.Count == 0)
            {
                Console.WriteLine("Ни одна папка не проинициализирована");
                return;
            }
            if (!File.Exists(ActiveDir.Path + @"\" + name))
            {
                Console.WriteLine("Файл не существует");
                return;
            }
            foreach (MyFile file in ActiveDir.filelist)
            {
                if (name == file.Name)
                {
                    if (file.Label == "<-- removed")
                    {
                        Console.WriteLine("Файл уже убран из версионного контроля");
                        return;
                    }
                    else
                    {
                        file.Label = "<-- removed";
                        Console.WriteLine("Файл убран из версионного контроля");
                        return;
                    }
                }

            }

        }

        public void Apply()
        {
            List<string> list = new List<string>();
            foreach (MyFile file in ActiveDir.filelist)
            {
                if (file.Label == "<-- removed")
                {
                    list.Add(file.Name);
                }
            }

            ActiveDir.filelist.Clear();
            ActiveDir.InitList(list.ToArray());
            Console.WriteLine("Все изменения сохранены");
            /*int i = 0;
            foreach (MyFile file in ActiveDir.filelist)
            {
                i++;
                foreach(string s in list)
                {
                    if (file.Name==s)
                    {   
                        ActiveDir.filelist.RemoveAt(i);
                    }
                }
            }*/
        }
        public void Listbranch()
        {

            if (FolderList.Count == 0)
            {
                Console.WriteLine("Ни одна папка не проинициализирована");
                return;
            }
            int i = 1;
            foreach (MyFolder folder in FolderList)
            {
                Console.WriteLine(i++ + ") " + folder.Path);

            }
        }
        public void Checkout(string str)
        {

            if (FolderList.Count == 0)
            {
                Console.WriteLine("Ни одна папка не проинициализирована");
                return;
            }
            if (int.TryParse(str, out int i))
            {
                if (i < FolderList.Count && i >= 0)
                {
                    ActiveDir = FolderList[i - 1];
                    Console.WriteLine("Activedirectory: {0}", ActiveDir.Path);
                    return;
                }
            }
            else if (Directory.Exists(str))
            {
                int number = 1;
                foreach (MyFolder folder in FolderList)
                {
                    if (str == folder.Path)
                    {
                        ActiveDir = FolderList[number];
                        Console.WriteLine("Activedirectory: ", ActiveDir.Path);
                        return;
                    }
                    number++;
                }
                Console.WriteLine("Директория с таким именем не отслеживается");
            }
            else Console.WriteLine("Директория не найдена");
        }
        public void CloseProgram(string way)
        {
            /*if (!File.Exists("1.txt"))
            {
                StreamWriter sw = File.CreateText("1.txt");          
            }*/
            using (StreamWriter sw = new StreamWriter("1.txt"))
            {
                foreach (MyFolder folder in FolderList)
                {
                    sw.WriteLine(folder.Path);
                }
                sw.Close();
            }
            var binFormat = new BinaryFormatter();
            using (var fs = new FileStream(way, FileMode.Create))
            {
                foreach (MyFolder folder in FolderList)
                {
                    ActiveDir = folder;
                    binFormat.Serialize(fs, ActiveDir.filelist);
                }
            }
            /*FileStream file = new FileStream(way, FileMode.Create, FileAccess.Write, FileShare.None);
            XmlSerializer ser = new XmlSerializer(typeof(List<MyFile>));

            foreach (MyFolder folder in FolderList)
                {
                    ActiveDir = folder;   
                    ser.Serialize(file, ActiveDir.filelist);
                }

            file.Close();*/
            Environment.Exit(0);
        }
    }
}
