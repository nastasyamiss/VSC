using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace VSC
{
    class Program
    {
        static void Main(string[] args)
        {
            Command cmd = new Command();
            const string filepath = "start.dat";
            cmd.FileOpen(filepath);
            do
            {
                Console.WriteLine("Введите команду:");
                string comand = Console.ReadLine();
                string[] array = new string[2];
                array = comand.Split(new[] { ' ' }, 2);
                array[0] = array[0].ToLower();
                if (array.Length == 2)
                {
                    switch (array[0])
                    {
                        case "init": cmd.Init(array[1]); break;
                        case "add": cmd.AddFile(array[1]); break;
                        case "remove": cmd.RemoveFile(array[1]); break;
                        case "checkout": cmd.Checkout(array[1]); break;
                        default: break;
                    }
                }
                else if (array.Length == 1)
                {
                    switch (array[0])
                    {
                        case "status": cmd.Status(); break;
                        case "listbranch": cmd.Listbranch(); break;
                        case "apply": cmd.Apply(); break;
                        case "exit": cmd.CloseProgram(filepath); break;
                        default: break;
                    }
                }
                else Console.WriteLine("Введена неверная команда");

            } while (true);

        }
    }
}
