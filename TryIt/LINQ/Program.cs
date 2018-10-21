using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace TryIt.LINQ
{
    public class Program
    {
        public static void LinQMain(string[] args)
        {
            string dir = "C:\\Windows";
            ShowTopFilesBySize(dir);
            Console.WriteLine("****");
            ShowTopeFilesBySizeLinq(dir);
        }

        private static void ShowTopeFilesBySizeLinq(string dir)
        {
            var query = from file in new DirectoryInfo(dir).GetFiles()
                        orderby file.Length descending
                        select file;
            foreach(var file in query.Take(5))
            {
                Console.WriteLine($"{file.Name,-20} : {file.Length,10:N0}");
            }
        }

        private static void ShowTopFilesBySize(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            Array.Sort(files, new FileInfoComparer());
            for(int i=0;i<5;i++)
            {
                var file = files[i];
                Console.WriteLine($"{file.Name, -20} : {file.Length, 10:N0}");
            }
        }
    }

    public class FileInfoComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            return y.Length.CompareTo(x.Length);
        }
    }
}
