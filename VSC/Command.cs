using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VSC
{
    class Command
    {
        public static List<MyFolder> FolderList = new List<MyFolder>();
        public static MyFolder ActiveDir = new MyFolder();

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
            foreach (FileInfo file in di.GetFiles())
            {
                NewFileNames.Add(new MyFile() {
                    Name = file.Name,
                    Size = file.Length,
                    Create = file.CreationTime,
                    Modified = file.LastWriteTime,
                });
            }

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
                    Console.WriteLine("size: " + newfile.Length);
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
                    if (OldFileNames[iold].Label == "<-- removed") //если файл уже убрали из версионного контроля нет смысла проверять дальше
                    {
                        Print(file.Name, "<-- removed", true);
                        continue;
                    }
                    else if (NewFileNames[inew].Size != OldFileNames[iold].Size) //изменился ли размер файла
                    {
                        Print(file.Name, "", true, "<-- " + NewFileNames[inew].Size);
                    }
                    else if (NewFileNames[inew].Modified != OldFileNames[iold].Modified) // или просто файл меняли, но размер не менялся
                    {
                        Print(file.Name, "", true, "", "<-- " + NewFileNames[inew].Modified);

                    }
                    else if (NewFileNames[inew].Create != OldFileNames[iold].Create) //или удалили и потом обратно создали??
                    {
                        Print(file.Name, "", true, "", "", "<-- " + NewFileNames[inew].Create);
                    }
                    else Print(file.Name, OldFileNames[iold].Label);
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
                    Console.WriteLine("Activedirectory: ", ActiveDir.Path);
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
    }
}
