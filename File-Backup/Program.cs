using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace File_Backup
{
    public class JSON
    {
        public List<string> sourse { get; set; }
        public string destination { get; set; }
    }
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите путь в файлу json");
            string path = Console.ReadLine();
            //string path = @"C:\Users\tat74\source\repos\File-Backup\paths.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("Файла не существует");
                Console.ReadKey();
                return;
            }
            JSON json = JsonConvert.DeserializeObject<JSON>(File.ReadAllText(path, Encoding.Default));

            string temp_path = json.destination + @"\Temp";

            CreateTempDirectory(temp_path);

            BackupToTemp(json.sourse, temp_path);

            return;
        }
        static void CreateTempDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }

        static void CopyDir(string sourse, string destination)
        {
            string[] files = System.IO.Directory.GetFiles(sourse);

            foreach (string s in files)
            {
                try
                {
                    string fileName = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(destination, fileName);
                    System.IO.File.Copy(s, destFile);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("{0}: {1}, {2}", e.GetType().Name, e.Message, s);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("У приложения не разрешения к {0}", s);
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("{0}", e.Message);
                }
            }
            string[] directories = System.IO.Directory.GetDirectories(sourse);
            foreach (string s in directories)
            {
                string new_path = destination  + s.Substring(s.LastIndexOf('\\'));
                Directory.CreateDirectory(new_path);
                CopyDir(s, new_path);
            }
        }
        static void BackupToTemp(List<string> sourse, string temp)
        {
            foreach (string path in sourse)
            {
                try
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        CopyDir(path, temp);
                    }
                    else
                    {
                        throw new DirectoryNotFoundException();
                    }
                }
                catch(DirectoryNotFoundException)
                {
                    Console.WriteLine("Путь {0} не существует", path);
                }
                
            }
        }
    }
}
